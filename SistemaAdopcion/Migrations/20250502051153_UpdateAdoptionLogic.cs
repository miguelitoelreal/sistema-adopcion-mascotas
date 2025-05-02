using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaAdopcion.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdoptionLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdoptionStatus",
                table: "Pets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdoptionStatus",
                table: "Pets",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
