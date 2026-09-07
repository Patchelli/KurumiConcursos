using System.Globalization;
using System.Text.Json;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.DataTransferObjects.PciDtos.Response;
using KurumiConcursos.Infra.Interfaces.MapperContracts;

namespace KurumiConcursos.Infra.Mappers;

public sealed class PciContestMapper : IPciContestMapper
{
    private static readonly JsonSerializerOptions RpcOptions = new(JsonSerializerDefaults.Web);

    private static readonly JsonSerializerOptions ContestOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public IReadOnlyList<ContestOpportunity> ResponseToDomain(string response)
    {
        var envelope = ReadEnvelope(response);
        if (envelope.Error is not null || envelope.Result is null || envelope.Result.IsError)
            throw new JsonException("PCI MCP returned an error.");

        var text = envelope.Result.Content?.FirstOrDefault(item => item.Type == "text")?.Text;
        if (string.IsNullOrWhiteSpace(text))
            throw new JsonException("PCI MCP returned no contest list.");

        var feed = JsonSerializer.Deserialize<PciContestListResponse>(text, ContestOptions);
        if (feed?.Data is null)
            throw new JsonException("PCI MCP returned an invalid contest list.");

        return feed.Data.Select(ResponseToDomain).ToArray();
    }

    private static ContestOpportunity ResponseToDomain(PciContestResponse response) => new(
        response.Id,
        response.Titulo ?? string.Empty,
        response.Cargos is { Count: > 0 } ? string.Join(", ", response.Cargos) : response.CargosResumo ?? string.Empty,
        response.Formacao ?? string.Empty,
        response.Regiao ?? string.Empty,
        response.Uf?.ToUpperInvariant() ?? string.Empty,
        response.VagasSalario ?? string.Empty,
        response.Datas?.Aberto ?? false,
        ParseDate(response.Datas?.Inicio),
        ParseDate(response.Datas?.Fim),
        SafeUrl(response.Noticia?.Link));

    private static DateOnly? ParseDate(string? value) =>
        DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date
            : null;

    private static string? SafeUrl(string? value) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme == "https" &&
        uri.Host == "www.pciconcursos.com.br"
            ? value
            : null;

    private static PciRpcResponse ReadEnvelope(string response)
    {
        if (response.TrimStart().StartsWith('{'))
            return JsonSerializer.Deserialize<PciRpcResponse>(response, RpcOptions) ??
                   throw new JsonException("Empty PCI response.");

        // SSE can include progress notifications before the response to our request.
        foreach (var block in response.Replace("\r\n", "\n").Split("\n\n", StringSplitOptions.RemoveEmptyEntries))
        {
            var data = string.Join("\n",
                block.Split('\n').Where(line => line.StartsWith("data:")).Select(line => line[5..].TrimStart()));
            if (string.IsNullOrWhiteSpace(data)) continue;
            var envelope = JsonSerializer.Deserialize<PciRpcResponse>(data, RpcOptions);
            if (envelope?.Id == 1) return envelope;
        }

        throw new JsonException("PCI MCP returned no matching response.");
    }
}