using Hangfire;
using Hangfire.PostgreSql;
using KurumiConcursos.Api.IoC;
using KurumiConcursos.Api.Settings;
using KurumiConcursos.Api.Settings.Handlers;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.Providers;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
builder.Services.AddInversionOfControlHandler();
builder.Services.AddSettingsControl(configuration);
builder.Services.AddHangfire((provider, options) => options.UsePostgreSqlStorage(storage =>
    storage.UseNpgsqlConnection(provider.GetRequiredService<ConnectionStringOptions>().DefaultConnection)));
builder.Services.AddHangfireServer();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
var app = builder.Build();
await app.MigrateDatabaseAsync();
app.Services.GetRequiredService<IRecurringJobManager>().AddOrUpdate<ITimeCapsuleCommandService>(
    "time-capsule-delivery", service => service.EvaluateDateTriggersAsync(), Cron.Minutely);
app.Services.GetRequiredService<IBackgroundJobClient>()
    .Enqueue<ITimeCapsuleCommandService>(service => service.EvaluateNonDateTriggersAsync());
app.AddWebApplication(configuration);
app.Run();

public partial class Program;