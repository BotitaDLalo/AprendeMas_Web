using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AprendeMasWeb.Migrations
{
    /// <inheritdoc />
    public partial class GruposMaterias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoColor",
                table: "Materias");

            migrationBuilder.AlterColumn<string>(
                name: "CodigoColor",
                table: "Grupos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoColor",
                table: "Materias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "CodigoColor",
                table: "Grupos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
