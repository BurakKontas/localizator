using Localizator.Shared.Base;
using Localizator.Shared.Mediator.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Localizator.Namespace.Infrastructure.Persistence.Context;

public class NamespaceDbContext(DbContextOptions<NamespaceDbContext> options, IMediator mediator) : BaseDbContext<NamespaceDbContext>(options, mediator)
{
    public DbSet<Domain.Namespace.Namespace> Namespaces { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(NamespaceDbContext).Assembly
        );
    }
}

// dotnet ef migrations add InitialCreate --project Localizator.Namespace\Localizator.Namespace.Infrastructure  --startup-project Localizator.API -c NamespaceDbContext -o .\Localizator.Namespace\Localizator.Namespace.Infrastructure\Persistence\Migrations