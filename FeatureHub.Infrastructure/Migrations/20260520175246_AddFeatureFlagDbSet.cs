using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeatureHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFeatureFlagDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeatureFlag_Environments_EnvironmentId",
                table: "FeatureFlag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FeatureFlag",
                table: "FeatureFlag");

            migrationBuilder.RenameTable(
                name: "FeatureFlag",
                newName: "FeatureFlags");

            migrationBuilder.RenameIndex(
                name: "IX_FeatureFlag_EnvironmentId",
                table: "FeatureFlags",
                newName: "IX_FeatureFlags_EnvironmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FeatureFlags",
                table: "FeatureFlags",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FeatureFlags_Environments_EnvironmentId",
                table: "FeatureFlags",
                column: "EnvironmentId",
                principalTable: "Environments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeatureFlags_Environments_EnvironmentId",
                table: "FeatureFlags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FeatureFlags",
                table: "FeatureFlags");

            migrationBuilder.RenameTable(
                name: "FeatureFlags",
                newName: "FeatureFlag");

            migrationBuilder.RenameIndex(
                name: "IX_FeatureFlags_EnvironmentId",
                table: "FeatureFlag",
                newName: "IX_FeatureFlag_EnvironmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FeatureFlag",
                table: "FeatureFlag",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FeatureFlag_Environments_EnvironmentId",
                table: "FeatureFlag",
                column: "EnvironmentId",
                principalTable: "Environments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
