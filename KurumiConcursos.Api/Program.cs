using KurumiConcursos.Api.IoC;
using KurumiConcursos.Api.Settings;
using KurumiConcursos.Api.Settings.Handlers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInversionOfControlHandler();
builder.Services.AddSettingsControl(builder.Configuration);
var app = builder.Build();
app.AddWebApplication();
await app.MigrateDatabaseAsync();
app.Run();

public partial class Program;