using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AprendeMasWeb.Migrations
{
    /// <inheritdoc />
    public partial class Actividades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actividades_Grupos_GrupoId",
                table: "Actividades");

            migrationBuilder.RenameColumn(
                name: "GrupoId",
                table: "Actividades",
                newName: "MateriaId");

            migrationBuilder.RenameIndex(
                name: "IX_Actividades_GrupoId",
                table: "Actividades",
                newName: "IX_Actividades_MateriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actividades_Materias_MateriaId",
                table: "Actividades",
                column: "MateriaId",
                principalTable: "Materias",
                principalColumn: "MateriaId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actividades_Materias_MateriaId",
                table: "Actividades");

            migrationBuilder.RenameColumn(
                name: "MateriaId",
                table: "Actividades",
                newName: "GrupoId");

            migrationBuilder.RenameIndex(
                name: "IX_Actividades_MateriaId",
                table: "Actividades",
                newName: "IX_Actividades_GrupoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actividades_Grupos_GrupoId",
                table: "Actividades",
                column: "GrupoId",
                principalTable: "Grupos",
                principalColumn: "GrupoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
