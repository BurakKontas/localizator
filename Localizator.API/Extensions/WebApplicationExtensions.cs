using Localizator.Auth.Application.Interfaces.Validators;
using Localizator.Auth.Domain.Interfaces.Strategy;
using Localizator.Auth.Infrastructure.Persistence;
using Localizator.Namespace.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Localizator.API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication AddLocalization(this WebApplication app)
    {
        var supportedCultures = new[]
        {
            new CultureInfo("en-US"),
            new CultureInfo("tr-TR")
        };

        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("en-US"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });

        return app;
    }

    public static async Task<WebApplication> Migrate(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var authDbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            var namespaceDbContext = scope.ServiceProvider.GetRequiredService<NamespaceDbContext>();
            await authDbContext.Database.MigrateAsync();
            await namespaceDbContext.Database.MigrateAsync();

            var optionsFactory = scope.ServiceProvider.GetRequiredService<IAuthOptionsProvider>();
            var validator = scope.ServiceProvider.GetRequiredService<IAuthOptionsValidatorResolver>();

            var options = optionsFactory.Get();
            validator.Validate(options);
        }

        return app;
    }
}
