using Localizator.Namespace.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Localizator.Namespace.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNamespaceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NamespaceDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("NamespaceDatabase"))
        );

        return services;
    }
}
