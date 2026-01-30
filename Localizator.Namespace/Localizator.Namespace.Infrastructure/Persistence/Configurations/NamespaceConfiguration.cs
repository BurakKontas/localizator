using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Localizator.Namespace.Infrastructure.Persistence.Configurations;

public class NamespaceConfiguration : IEntityTypeConfiguration<Domain.Namespace.Namespace>
{
    public void Configure(EntityTypeBuilder<Domain.Namespace.Namespace> builder)
    {
        builder.ToTable("Namespaces");
        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Name, nameBuilder =>
        {
            nameBuilder.Property(n => n.Value)
                .HasColumnName("Name")
                .HasMaxLength(100)
                .IsRequired();

            nameBuilder.HasIndex(n => n.Value)
                .IsUnique();
        });

        builder.OwnsOne(x => x.Slug, slugBuilder =>
        {
            slugBuilder.Property(s => s.Value)
                .HasColumnName("Slug")
                .HasMaxLength(50)
                .IsRequired();

            slugBuilder.HasIndex(n => n.Value)
                .IsUnique();
        });

        builder.OwnsOne(x => x.Status, statusBuilder =>
        {
            statusBuilder.Property(s => s.Value)
                .HasColumnName("Status")
                .HasMaxLength(20)
                .IsRequired();

            statusBuilder.HasIndex(n => n.Value)
                .IsUnique();
        });

        builder.OwnsOne(x => x.CurrentVersion, versionBuilder =>
        {
            versionBuilder.Property(v => v.Value)
                .HasColumnName("CurrentVersion")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.OwnsOne(x => x.LastPublishedBy, publisherBuilder =>
        {
            publisherBuilder.Property(p => p.Value)
                .HasColumnName("LastPublishedBy")
                .HasMaxLength(100);
        });

        builder.OwnsOne(x => x.LastPublishedAt, dateBuilder =>
        {
            dateBuilder.Property(d => d.Value)
                .HasColumnName("LastPublishedAt");
        });

        builder.OwnsMany(x => x.SupportedLanguages, languagesBuilder =>
        {
            languagesBuilder.ToTable("NamespaceSupportedLanguages");

            languagesBuilder.WithOwner()
                .HasForeignKey("NamespaceId");

            languagesBuilder.Property<int>("Id")
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();

            languagesBuilder.HasKey("Id");

            languagesBuilder.Property(l => l.LanguageCode)
                .HasColumnName("LanguageCode")
                .HasMaxLength(2)
                .IsRequired();

            languagesBuilder.HasIndex("NamespaceId");
            languagesBuilder.HasIndex("NamespaceId", "LanguageCode").IsUnique();
        });

        builder.OwnsMany(x => x.Permissions, userPermissionBuilder =>
        {
            userPermissionBuilder.ToTable("NamespaceUserPermissions");

            userPermissionBuilder.WithOwner()
                .HasForeignKey("NamespaceId");

            userPermissionBuilder.Property<int>("Id")
                .ValueGeneratedOnAdd();

            userPermissionBuilder.HasKey("Id");

            userPermissionBuilder.Property(p => p.User)
                .HasColumnName("User")
                .HasMaxLength(100)
                .IsRequired();

            userPermissionBuilder.HasIndex("NamespaceId", "User")
                .IsUnique();

            userPermissionBuilder.OwnsMany(p => p.Permissions, permissionBuilder =>
            {
                permissionBuilder.ToTable("NamespacePermissions");

                permissionBuilder.WithOwner()
                    .HasForeignKey("NamespaceUserPermissionId");

                permissionBuilder.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                permissionBuilder.HasKey("Id");

                permissionBuilder.Property(x => x.Permission)
                    .HasColumnName("Permission")
                    .HasMaxLength(50)
                    .IsRequired();

                permissionBuilder.HasIndex(
                    "NamespaceUserPermissionId",
                    "Permission"
                ).IsUnique();
            });
        });

        builder.Property(x => x.IsPublic)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.IsPublic);

        builder.Navigation(x => x.Permissions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}