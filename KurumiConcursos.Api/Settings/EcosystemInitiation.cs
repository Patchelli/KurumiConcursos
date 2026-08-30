using System.Globalization;
using KurumiConcursos.Api.Middlewares;
using KurumiConcursos.Api.Settings.Constants;
using KurumiConcursos.Domain.Providers;
using Microsoft.AspNetCore.Localization;

namespace KurumiConcursos.Api.Settings;

public static class EcosystemInitiation
{
    public static void AddWebApplication(this WebApplication app, IConfiguration configuration)
    {
        var environmentConfiguration = configuration
            .GetSection(EnvironmentConfigurationOptions.SectionName)
            .Get<EnvironmentConfigurationOptions>()!;

        var supportedCultures = new[] { new CultureInfo("pt-BR") };
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("pt-BR"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });

        // Apply CORS before exception/redirect middleware so error responses
        // also carry the headers required by browsers.
        app.UseCors(CorsName.DefaultPolicy);

        if (environmentConfiguration.ActiveSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "swagger";
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "KurumiConcursos.Api v1");
            });
        }

        if (app.Environment.IsDevelopment())
            app.UseMiddleware<RequestTimingMiddleware>();

        if (!app.Environment.IsDevelopment())
        {
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseMiddleware<CaptureStatusCodeTooManyRequestsMiddleware>();
        app.UseWebSockets();
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapHealthChecks("/health");
        app.MapControllers();
    }
}