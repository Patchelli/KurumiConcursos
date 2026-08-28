using KurumiConcursos.Api.Middlewares;
using KurumiConcursos.Api.Settings.Constants;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class WebApplicationHandler
{
    public static void AddWebApplication(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        app.UseHttpsRedirection();
        app.UseCors(CorsName.DefaultPolicy);
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
    }
}