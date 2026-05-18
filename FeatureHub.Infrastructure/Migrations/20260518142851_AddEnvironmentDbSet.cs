using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeatureHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEnvironmentDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Environment_Projects_ProjectId",
                table: "Environment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Environment",
                table: "Environment");

            migrationBuilder.RenameTable(
                name: "Environment",
                newName: "Environments");

            migrationBuilder.RenameIndex(
                name: "IX_Environment_ProjectId",
                table: "Environments",
                newName: "IX_Environments_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Environments",
                table: "Environments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Environments_Projects_ProjectId",
                table: "Environments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Environments_Projects_ProjectId",
                table: "Environments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Environments",
                table: "Environments");

            migrationBuilder.RenameTable(
                name: "Environments",
                newName: "Environment");

            migrationBuilder.RenameIndex(
                name: "IX_Environments_ProjectId",
                table: "Environment",
                newName: "IX_Environment_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Environment",
                table: "Environment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Environment_Projects_ProjectId",
                table: "Environment",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
