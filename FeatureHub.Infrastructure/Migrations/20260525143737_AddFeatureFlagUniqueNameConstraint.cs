using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeatureHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFeatureFlagUniqueNameConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FeatureFlags_Name_EnvironmentId",
                table: "FeatureFlags",
                columns: new[] { "Name", "EnvironmentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FeatureFlags_Name_EnvironmentId",
                table: "FeatureFlags");
        }
    }
}
