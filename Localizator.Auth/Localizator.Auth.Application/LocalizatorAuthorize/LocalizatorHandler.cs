using Localizator.Auth.Domain.Configuration.Mode;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Localizator.Shared.Resources;
using Localizator.Shared.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text.Json;

namespace Localizator.Auth.Application.LocalizatorAuthorize;

public sealed class LocalizatorHandler(IAuthStrategy authStrategy) : AuthorizationHandler<LocalizatorRequirement>
{
    private readonly IAuthStrategy _authStrategy = authStrategy;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, LocalizatorRequirement requirement)
    {
        if (context.Resource is HttpContext httpContext)
        {
            Result<int> result = await _authStrategy.AuthenticateAsync(httpContext);
            
            if(result.IsSuccess)
            {
                context.Succeed(requirement);
            }
            else
            {
                string reason = result.Message ?? "Authentication failed";
                httpContext.Items.TryAdd("AuthResult", result);
                context.Fail(new AuthorizationFailureReason(this, reason));
            }
        }
        else
        {
            context.Fail(new AuthorizationFailureReason(this, Errors.ContextResourceIsNotHttpContext));
        }
    }
}
