using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AprendeMasWeb.Migrations
{
    /// <inheritdoc />
    public partial class tb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actividades_Materias_MateriaId",
                table: "Actividades");

            migrationBuilder.DropForeignKey(
                name: "FK_GruposMaterias_Grupos_GrupoId",
                table: "GruposMaterias");

            migrationBuilder.DropForeignKey(
                name: "FK_GruposMaterias_Materias_MateriaId",
                table: "GruposMaterias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Materias",
                table: "Materias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GruposMaterias",
                table: "GruposMaterias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Grupos",
                table: "Grupos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Actividades",
                table: "Actividades");

            migrationBuilder.RenameTable(
                name: "Materias",
                newName: "tbMaterias");

            migrationBuilder.RenameTable(
                name: "GruposMaterias",
                newName: "tbGruposMaterias");

            migrationBuilder.RenameTable(
                name: "Grupos",
                newName: "tbGrupos");

            migrationBuilder.RenameTable(
                name: "Actividades",
                newName: "tbActividades");

            migrationBuilder.RenameIndex(
                name: "IX_GruposMaterias_MateriaId",
                table: "tbGruposMaterias",
                newName: "IX_tbGruposMaterias_MateriaId");

            migrationBuilder.RenameIndex(
                name: "IX_GruposMaterias_GrupoId",
                table: "tbGruposMaterias",
                newName: "IX_tbGruposMaterias_GrupoId");

            migrationBuilder.RenameIndex(
                name: "IX_Actividades_MateriaId",
                table: "tbActividades",
                newName: "IX_tbActividades_MateriaId");

            migrationBuilder.AlterColumn<string>(
                name: "CodigoColor",
                table: "tbGrupos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbMaterias",
                table: "tbMaterias",
                column: "MateriaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbGruposMaterias",
                table: "tbGruposMaterias",
                column: "GrupoMateriasId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbGrupos",
                table: "tbGrupos",
                column: "GrupoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbActividades",
                table: "tbActividades",
                column: "ActividadId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbActividades_tbMaterias_MateriaId",
                table: "tbActividades",
                column: "MateriaId",
                principalTable: "tbMaterias",
                principalColumn: "MateriaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbGruposMaterias_tbGrupos_GrupoId",
                table: "tbGruposMaterias",
                column: "GrupoId",
                principalTable: "tbGrupos",
                principalColumn: "GrupoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbGruposMaterias_tbMaterias_MateriaId",
                table: "tbGruposMaterias",
                column: "MateriaId",
                principalTable: "tbMaterias",
                principalColumn: "MateriaId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbActividades_tbMaterias_MateriaId",
                table: "tbActividades");

            migrationBuilder.DropForeignKey(
                name: "FK_tbGruposMaterias_tbGrupos_GrupoId",
                table: "tbGruposMaterias");

            migrationBuilder.DropForeignKey(
                name: "FK_tbGruposMaterias_tbMaterias_MateriaId",
                table: "tbGruposMaterias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbMaterias",
                table: "tbMaterias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbGruposMaterias",
                table: "tbGruposMaterias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbGrupos",
                table: "tbGrupos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbActividades",
                table: "tbActividades");

            migrationBuilder.RenameTable(
                name: "tbMaterias",
                newName: "Materias");

            migrationBuilder.RenameTable(
                name: "tbGruposMaterias",
                newName: "GruposMaterias");

            migrationBuilder.RenameTable(
                name: "tbGrupos",
                newName: "Grupos");

            migrationBuilder.RenameTable(
                name: "tbActividades",
                newName: "Actividades");

            migrationBuilder.RenameIndex(
                name: "IX_tbGruposMaterias_MateriaId",
                table: "GruposMaterias",
                newName: "IX_GruposMaterias_MateriaId");

            migrationBuilder.RenameIndex(
                name: "IX_tbGruposMaterias_GrupoId",
                table: "GruposMaterias",
                newName: "IX_GruposMaterias_GrupoId");

            migrationBuilder.RenameIndex(
                name: "IX_tbActividades_MateriaId",
                table: "Actividades",
                newName: "IX_Actividades_MateriaId");

            migrationBuilder.AlterColumn<string>(
                name: "CodigoColor",
                table: "Grupos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Materias",
                table: "Materias",
                column: "MateriaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GruposMaterias",
                table: "GruposMaterias",
                column: "GrupoMateriasId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Grupos",
                table: "Grupos",
                column: "GrupoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Actividades",
                table: "Actividades",
                column: "ActividadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actividades_Materias_MateriaId",
                table: "Actividades",
                column: "MateriaId",
                principalTable: "Materias",
                principalColumn: "MateriaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GruposMaterias_Grupos_GrupoId",
                table: "GruposMaterias",
                column: "GrupoId",
                principalTable: "Grupos",
                principalColumn: "GrupoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GruposMaterias_Materias_MateriaId",
                table: "GruposMaterias",
                column: "MateriaId",
                principalTable: "Materias",
                principalColumn: "MateriaId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
