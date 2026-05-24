using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocANAI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameIsActiveToIsAvailable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "a_i_model",
                newName: "is_available");

            migrationBuilder.CreateIndex(
                name: "IX_a_i_model_name_version",
                table: "a_i_model",
                columns: new[] { "name", "version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_a_i_model_name_version",
                table: "a_i_model");

            migrationBuilder.RenameColumn(
                name: "is_available",
                table: "a_i_model",
                newName: "is_active");
        }
    }
}
