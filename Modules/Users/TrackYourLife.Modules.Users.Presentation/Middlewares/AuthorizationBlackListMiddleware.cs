using Microsoft.AspNetCore.Authorization;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;

namespace TrackYourLife.Modules.Users.Presentation.Middlewares;

public class AuthorizationBlackListMiddleware(
    RequestDelegate next,
    IAuthorizationBlackListStorage authorizationBlackListStorage
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            await next(context);
            return;
        }

        var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        if (token != null && authorizationBlackListStorage.Contains(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            return;
        }

        await next(context);
    }
}
