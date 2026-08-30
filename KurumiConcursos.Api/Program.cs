using KurumiConcursos.Api.IoC;
using KurumiConcursos.Api.Settings;
using KurumiConcursos.Api.Settings.Handlers;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
builder.Services.AddInversionOfControlHandler();
builder.Services.AddSettingsControl(configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
var app = builder.Build();
await app.MigrateDatabaseAsync();
app.AddWebApplication(configuration);
app.Run();

public partial class Program;