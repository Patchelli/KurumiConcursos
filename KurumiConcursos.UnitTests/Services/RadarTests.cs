using System.Linq.Expressions;
using System.Net;
using System.Text.Json;
using KurumiConcursos.ApplicationService.DataTransferObjects.RadarDtos.Response;
using KurumiConcursos.ApplicationService.Mappers;
using KurumiConcursos.ApplicationService.Services.RadarServices;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.NotificationHandler;
using KurumiConcursos.Domain.Providers;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.Mappers;
using KurumiConcursos.Infra.Services.PciContestServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace KurumiConcursos.UnitTests.Services;

public sealed class RadarTests
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(TimeZoneInfo
        .ConvertTimeBySystemTimeZoneId(DateTimeOffset.UtcNow, "America/Sao_Paulo").DateTime);

    private static object Contest(int id, string state = "SP", string region = "SUDESTE",
        string education = "Médio / Superior", string role = "ANALISTA DE SAÚDE", int days = 3, bool open = true,
        int startDays = -5, string url = "https://www.pciconcursos.com.br/noticias/teste") => new
    {
        id, titulo = $"Concurso {id}", cargos = new[] { role }, formacao = education, uf = state, regiao = region,
        vagas_salario = "2 vagas até R$ 5.000,00",
        datas = new
        {
            aberto = open, inicio = Today.AddDays(startDays).ToString("yyyy-MM-dd"),
            fim = Today.AddDays(days).ToString("yyyy-MM-dd")
        },
        noticia = new { link = url }
    };

    private static IReadOnlyList<ContestOpportunity> Feed(params object[] items) =>
        new PciContestMapper().ResponseToDomain(JsonSerializer.Serialize(new
        {
            result = new { content = new[] { new { type = "text", text = RawFeed(items).GetRawText() } } }
        }));

    private static JsonElement RawFeed(params object[] items) =>
        JsonSerializer.SerializeToElement(new { data = items });

    [Fact]
    public void FiltersStateEducationAndAccentInsensitiveRoleWhileIncludingNational()
    {
        var feed = Feed(Contest(1), Contest(2, state: "RJ"), Contest(3, state: "BR", region: "NACIONAL"),
            Contest(4, education: "Fundamental"), Contest(5, role: "PROFESSOR"));
        var result = RadarContestFilter.Filter(feed, new(State: "SP", Education: "superior", Role: "saude"));
        Assert.Equal(new[] { 1, 3 }, result.Select(x => x.Id));
    }

    [Fact]
    public void ExcludesNationalAndOtherRegionsWhenRequested()
    {
        var feed = Feed(Contest(1), Contest(2, "PR", "SUL"), Contest(3, "BR", "NACIONAL"));
        var result = RadarContestFilter.Filter(feed, new(Region: "sul", IncludeNational: false));
        Assert.Equal(2, Assert.Single(result).Id);
    }

    [Fact]
    public void ExcludesClosedExpiredAndFutureContestsAndSortsByDeadline()
    {
        var feed = Feed(Contest(1, days: 6), Contest(2, days: -1), Contest(3, open: false),
            Contest(4, startDays: 2), Contest(5, days: 0), Contest(5, days: 0));
        var result = RadarContestFilter.Filter(feed, new());
        Assert.Equal(new[] { 5, 1 }, result.Select(x => x.Id));
        Assert.Equal(Today, result[0].RegistrationEnd);
    }

    [Fact]
    public void DoesNotExposeUnsafeLinks()
    {
        var result = RadarContestFilter.Filter(Feed(Contest(1, url: "javascript:alert(1)")), new());
        Assert.Null(Assert.Single(result).Url);
    }

    [Fact]
    public void PreferencesRoundTripAndDefaultToAllBrazil()
    {
        var mapper = new RadarMapper();
        var user = new User();
        Assert.Equal(new RadarPreferencesResponse(), mapper.DomainToPreferencesResponse(user));
        mapper.DtoUpdateToDomain(user,
            new() { State = "SP", Education = "superior", Role = " analista ", IncludeNational = false });
        Assert.Equal(
            new RadarPreferencesResponse(State: "SP", Education: "superior", Role: "analista", IncludeNational: false),
            mapper.DomainToPreferencesResponse(user));
    }

    [Fact]
    public async Task CommandOnlyUpdatesAuthenticatedUser()
    {
        var current = new User { Id = Guid.NewGuid() };
        var other = new User { Id = Guid.NewGuid() };
        var repository = new Mock<IUserRepository>();
        repository.Setup(x => x.FindByPredicateAsync(It.IsAny<Expression<Func<User, bool>>>(), null, false))
            .Returns((Expression<Func<User, bool>> predicate,
                    Func<IQueryable<User>, IIncludableQueryable<User, object>>? _, bool _) =>
                Task.FromResult(new[] { current, other }.FirstOrDefault(predicate.Compile())));
        repository.Setup(x => x.UpdateAsync(current)).ReturnsAsync(IdentityResult.Success);
        var service = new RadarCommandService(repository.Object, new RadarMapper(), new NotificationHandler());
        Assert.True(await service.SavePreferencesAsync(new() { State = "SP" },
            new UserCredential { UserId = current.Id, Roles = [] }));
        Assert.NotNull(current.RadarPreferencesJson);
        Assert.Null(other.RadarPreferencesJson);
        repository.Verify(x => x.UpdateAsync(current), Times.Once);
    }

    [Fact]
    public async Task InvalidPreferencesNeverReachPersistence()
    {
        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        var notifications = new NotificationHandler();
        var service = new RadarCommandService(repository.Object, new RadarMapper(), notifications);
        Assert.False(await service.SavePreferencesAsync(new() { State = "XX" }, new UserCredential { Roles = [] }));
        Assert.True(notifications.HasNotification());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ParsesJsonAndSse(bool sse)
    {
        var envelope = JsonSerializer.Serialize(new
        {
            jsonrpc = "2.0", id = 1,
            result = new { content = new[] { new { type = "text", text = RawFeed(Contest(1)).GetRawText() } } }
        });
        var parsed = new PciContestMapper().ResponseToDomain(sse ? $"event: message\ndata: {envelope}\n\n" : envelope);
        Assert.Single(parsed);
    }

    [Fact]
    public void RejectsMcpErrorsInsteadOfReturningAnEmptyList()
    {
        Assert.Throws<JsonException>(() => new PciContestMapper().ResponseToDomain("{\"error\":{\"code\":-1}}"));
        Assert.Throws<JsonException>(() => new PciContestMapper().ResponseToDomain("{\"result\":{\"isError\":true}}"));
    }

    [Fact]
    public async Task CachesSourceBetweenQueries()
    {
        var envelope = JsonSerializer.Serialize(new
            { result = new { content = new[] { new { type = "text", text = RawFeed(Contest(1)).GetRawText() } } } });
        var handler = new FakePciHandler(envelope);
        using var http = new HttpClient(handler) { BaseAddress = new Uri("https://mcp.pciconcursos.com.br/mcp") };
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var client = new PciContestQueryService(http, new PciContestMapper(), cache, new PciConcursosOptions());
        var first = await client.GetContestsAsync(default);
        var second = await client.GetContestsAsync(default);
        Assert.Equal(1, handler.Calls);
        Assert.Equal(first.UpdatedAt, second.UpdatedAt);
    }

    private sealed class FakePciHandler(string envelope) : HttpMessageHandler
    {
        public int Calls { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                { Content = new StringContent(envelope) });
        }
    }
}