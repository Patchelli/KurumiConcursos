using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace KurumiConcursos.Api.Middlewares;

public sealed class GlobalExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled request error");
            var (status, title) = ex switch
            {
                ValidationException => (400, "Validation failed"),
                KeyNotFoundException => (404, "Resource not found"),
                InvalidOperationException => (409, "Operation conflict"),
                ArgumentException => (400, "Invalid request"), _ => (500, "Unexpected error")
            };
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status, Title = title, Detail = status == 500 ? "An unexpected error occurred." : ex.Message
            });
        }
    }
}