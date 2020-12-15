using Microsoft.EntityFrameworkCore.Migrations;

namespace Localization.AspNetCore.EntityFramework.Test.Migrations
{
    public partial class FixConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LocalizationResourceTranslations_Language",
                table: "LocalizationResourceTranslations");

            migrationBuilder.CreateIndex(
                name: "IX_LocalizationResourceTranslations_Language_ResourceId",
                table: "LocalizationResourceTranslations",
                columns: new[] { "Language", "ResourceId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LocalizationResourceTranslations_Language_ResourceId",
                table: "LocalizationResourceTranslations");

            migrationBuilder.CreateIndex(
                name: "IX_LocalizationResourceTranslations_Language",
                table: "LocalizationResourceTranslations",
                column: "Language",
                unique: true);
        }
    }
}
