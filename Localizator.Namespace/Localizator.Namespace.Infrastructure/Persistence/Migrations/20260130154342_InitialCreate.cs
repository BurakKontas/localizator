using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Localizator.Namespace.Infrastructure.Localizator.Namespace.Localizator.Namespace.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Namespaces",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CurrentVersion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LastPublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastPublishedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Namespaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NamespaceSupportedLanguages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LanguageCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    NamespaceId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NamespaceSupportedLanguages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NamespaceSupportedLanguages_Namespaces_NamespaceId",
                        column: x => x.NamespaceId,
                        principalTable: "Namespaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NamespaceUserPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    User = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NamespaceId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NamespaceUserPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NamespaceUserPermissions_Namespaces_NamespaceId",
                        column: x => x.NamespaceId,
                        principalTable: "Namespaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NamespacePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Permission = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NamespaceUserPermissionId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NamespacePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NamespacePermissions_NamespaceUserPermissions_NamespaceUser~",
                        column: x => x.NamespaceUserPermissionId,
                        principalTable: "NamespaceUserPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NamespacePermissions_NamespaceUserPermissionId_Permission",
                table: "NamespacePermissions",
                columns: new[] { "NamespaceUserPermissionId", "Permission" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Namespaces_CreatedAt",
                table: "Namespaces",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Namespaces_IsPublic",
                table: "Namespaces",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_Namespaces_Name",
                table: "Namespaces",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Namespaces_Slug",
                table: "Namespaces",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Namespaces_Status",
                table: "Namespaces",
                column: "Status",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NamespaceSupportedLanguages_NamespaceId",
                table: "NamespaceSupportedLanguages",
                column: "NamespaceId");

            migrationBuilder.CreateIndex(
                name: "IX_NamespaceSupportedLanguages_NamespaceId_LanguageCode",
                table: "NamespaceSupportedLanguages",
                columns: new[] { "NamespaceId", "LanguageCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NamespaceUserPermissions_NamespaceId_User",
                table: "NamespaceUserPermissions",
                columns: new[] { "NamespaceId", "User" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NamespacePermissions");

            migrationBuilder.DropTable(
                name: "NamespaceSupportedLanguages");

            migrationBuilder.DropTable(
                name: "NamespaceUserPermissions");

            migrationBuilder.DropTable(
                name: "Namespaces");
        }
    }
}
