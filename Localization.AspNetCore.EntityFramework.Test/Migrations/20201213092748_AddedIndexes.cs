using Microsoft.EntityFrameworkCore.Migrations;

namespace Localization.AspNetCore.EntityFramework.Test.Migrations
{
    public partial class AddedIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LocalizationResourceTranslations_Language_ResourceId",
                table: "LocalizationResourceTranslations");

            migrationBuilder.CreateIndex(
                name: "IX_LocalizationResourceTranslations_Language",
                table: "LocalizationResourceTranslations",
                column: "Language",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalizationResources_ResourceKey",
                table: "LocalizationResources",
                column: "ResourceKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LocalizationResourceTranslations_Language",
                table: "LocalizationResourceTranslations");

            migrationBuilder.DropIndex(
                name: "IX_LocalizationResources_ResourceKey",
                table: "LocalizationResources");

            migrationBuilder.CreateIndex(
                name: "IX_LocalizationResourceTranslations_Language_ResourceId",
                table: "LocalizationResourceTranslations",
                columns: new[] { "Language", "ResourceId" },
                unique: true);
        }
    }
}
