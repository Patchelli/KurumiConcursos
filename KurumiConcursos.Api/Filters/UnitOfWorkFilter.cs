using KurumiConcursos.Domain.Interface;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KurumiConcursos.Api.Filters;

public sealed class UnitOfWorkFilter(
    IUnitOfWork unitOfWork,
    INotificationHandler notificationHandler)
    : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        if (HttpMethods.IsGet(context.HttpContext.Request.Method))
        {
            await next();
            return;
        }

        unitOfWork.BeginTransaction();

        var executed = await next();
        var routeName = context.HttpContext.GetEndpoint()?.DisplayName;

        if (IsAuthenticatorRoute(routeName))
            await AuthenticationMethodAsync(executed);
        else
            await OthersMethodsAsync(executed);
    }

    private static bool IsAuthenticatorRoute(string? routeName)
    {
        const string transfer = "GenerateAccessToken";
        return routeName is not null &&
               routeName.Contains(transfer, StringComparison.InvariantCulture);
    }

    private async Task AuthenticationMethodAsync(ActionExecutedContext context)
    {
        if (context.Exception is null && context.ModelState.IsValid)
            await unitOfWork.CommitAsync();
        else
            unitOfWork.RollbackTransaction();
    }

    private async Task OthersMethodsAsync(ActionExecutedContext context)
    {
        if (SuccessFlow(context))
            await unitOfWork.CommitAsync();
        else
            unitOfWork.RollbackTransaction();
    }

    private bool SuccessFlow(ActionExecutedContext context) =>
        context.Exception is null &&
        context.ModelState.IsValid &&
        !notificationHandler.HasNotification();
}