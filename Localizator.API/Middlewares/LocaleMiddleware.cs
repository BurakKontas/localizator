using Localizator.Shared.Providers;
using Localizator.Shared.Resources;
using Localizator.Shared.Result;
using System.Globalization;

namespace Localizator.API.Middlewares;

public class LocaleMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var headerLocale = context.Request.Headers["X-Locale"].FirstOrDefault();

        var locale = LocaleProvider.Normalize(headerLocale);
        var culture = LocaleProvider.GetCulture(locale);

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        Errors.Culture = culture;
        Messages.Culture = culture;

        context.Response.Headers["Content-Language"] = locale;

        await _next(context);
    }
}
