using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AprendeMasWeb.Migrations
{
    /// <inheritdoc />
    public partial class PuntajeActividad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbAlumnosActividades_tbEntregablesAlumno_AlumnoActividadId",
                table: "tbAlumnosActividades");

            migrationBuilder.AddColumn<string>(
                name: "Respuesta",
                table: "tbEntregablesAlumno",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Puntaje",
                table: "tbActividades",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_tbEntregablesAlumno_AlumnoActividadId",
                table: "tbEntregablesAlumno",
                column: "AlumnoActividadId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tbEntregablesAlumno_tbAlumnosActividades_AlumnoActividadId",
                table: "tbEntregablesAlumno",
                column: "AlumnoActividadId",
                principalTable: "tbAlumnosActividades",
                principalColumn: "AlumnoActividadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbEntregablesAlumno_tbAlumnosActividades_AlumnoActividadId",
                table: "tbEntregablesAlumno");

            migrationBuilder.DropIndex(
                name: "IX_tbEntregablesAlumno_AlumnoActividadId",
                table: "tbEntregablesAlumno");

            migrationBuilder.DropColumn(
                name: "Respuesta",
                table: "tbEntregablesAlumno");

            migrationBuilder.DropColumn(
                name: "Puntaje",
                table: "tbActividades");

            migrationBuilder.AlterColumn<int>(
                name: "AlumnoActividadId",
                table: "tbAlumnosActividades",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddForeignKey(
                name: "FK_tbAlumnosActividades_tbEntregablesAlumno_AlumnoActividadId",
                table: "tbAlumnosActividades",
                column: "AlumnoActividadId",
                principalTable: "tbEntregablesAlumno",
                principalColumn: "EntregaId");
        }
    }
}
