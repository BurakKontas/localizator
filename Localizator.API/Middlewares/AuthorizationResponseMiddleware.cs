using Localizator.Shared.Exceptions;
using Localizator.Shared.Result;

namespace Localizator.API.Middlewares;

public class AuthorizationResponseMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Items.TryGetValue("AuthResult", out var value) && value is Result<int> result)
        {
            context.Items.Remove("AuthResult");
            throw new TechnicalException(title: "Authorization Error", message: result.Message, statusCode: result.Data);
        }
    }
}

