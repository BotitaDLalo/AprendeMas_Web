using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AprendeMasWeb.Migrations
{
    /// <inheritdoc />
    public partial class NuevaMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Materias");

            migrationBuilder.DropTable(
                name: "Grupos");

            migrationBuilder.CreateTable(
                name: "cTiposActividades",
                columns: table => new
                {
                    TipoActividadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cTiposActividades", x => x.TipoActividadId);
                });

            migrationBuilder.CreateTable(
                name: "tbAlumnos",
                columns: table => new
                {
                    AlumnoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAlumnos", x => x.AlumnoId);
                    table.ForeignKey(
                        name: "FK_tbAlumnos_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbDocentes",
                columns: table => new
                {
                    DocenteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbDocentes", x => x.DocenteId);
                    table.ForeignKey(
                        name: "FK_tbDocentes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbEntregablesAlumno",
                columns: table => new
                {
                    EntregaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlumnoActividadId = table.Column<int>(type: "int", nullable: false),
                    Enlace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Archivo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbEntregablesAlumno", x => x.EntregaId);
                });

            migrationBuilder.CreateTable(
                name: "tbActividades",
                columns: table => new
                {
                    ActividadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreActividad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaLimite = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoActividadId = table.Column<int>(type: "int", nullable: false),
                    MateriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbActividades", x => x.ActividadId);
                    table.ForeignKey(
                        name: "FK_tbActividades_cTiposActividades_TipoActividadId",
                        column: x => x.TipoActividadId,
                        principalTable: "cTiposActividades",
                        principalColumn: "TipoActividadId");
                });

            migrationBuilder.CreateTable(
                name: "tbAlumnosTokens",
                columns: table => new
                {
                    FCMToken = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlumnoId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAlumnosTokens", x => x.FCMToken);
                    table.ForeignKey(
                        name: "FK_tbAlumnosTokens_tbAlumnos_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "tbAlumnos",
                        principalColumn: "AlumnoId");
                });

            migrationBuilder.CreateTable(
                name: "tbAvisos",
                columns: table => new
                {
                    AvisoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocenteId = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAvisos", x => x.AvisoId);
                    table.ForeignKey(
                        name: "FK_tbAvisos_tbDocentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "tbDocentes",
                        principalColumn: "DocenteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbEventosAgenda",
                columns: table => new
                {
                    EventoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocenteId = table.Column<int>(type: "int", nullable: false),
                    FechaMarcada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbEventosAgenda", x => x.EventoId);
                    table.ForeignKey(
                        name: "FK_tbEventosAgenda_tbDocentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "tbDocentes",
                        principalColumn: "DocenteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbGrupos",
                columns: table => new
                {
                    GrupoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreGrupo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodigoAcceso = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodigoColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocenteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbGrupos", x => x.GrupoId);
                    table.ForeignKey(
                        name: "FK_tbGrupos_tbDocentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "tbDocentes",
                        principalColumn: "DocenteId");
                });

            migrationBuilder.CreateTable(
                name: "tbMaterias",
                columns: table => new
                {
                    MateriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreMateria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodigoColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodigoAcceso = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocenteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbMaterias", x => x.MateriaId);
                    table.ForeignKey(
                        name: "FK_tbMaterias_tbDocentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "tbDocentes",
                        principalColumn: "DocenteId");
                });

            migrationBuilder.CreateTable(
                name: "tbAlumnosActividades",
                columns: table => new
                {
                    AlumnoActividadId = table.Column<int>(type: "int", nullable: false),
                    ActividadId = table.Column<int>(type: "int", nullable: false),
                    AlumnoId = table.Column<int>(type: "int", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstatusEntrega = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAlumnosActividades", x => x.AlumnoActividadId);
                    table.ForeignKey(
                        name: "FK_tbAlumnosActividades_tbActividades_ActividadId",
                        column: x => x.ActividadId,
                        principalTable: "tbActividades",
                        principalColumn: "ActividadId");
                    table.ForeignKey(
                        name: "FK_tbAlumnosActividades_tbAlumnos_ActividadId",
                        column: x => x.ActividadId,
                        principalTable: "tbAlumnos",
                        principalColumn: "AlumnoId");
                    table.ForeignKey(
                        name: "FK_tbAlumnosActividades_tbEntregablesAlumno_AlumnoActividadId",
                        column: x => x.AlumnoActividadId,
                        principalTable: "tbEntregablesAlumno",
                        principalColumn: "EntregaId");
                });

            migrationBuilder.CreateTable(
                name: "tbCalificaciones",
                columns: table => new
                {
                    CalificacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActividadId = table.Column<int>(type: "int", nullable: false),
                    FechaCalificacionAsignada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comentarios = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Calificacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbCalificaciones", x => x.CalificacionId);
                    table.ForeignKey(
                        name: "FK_tbCalificaciones_tbActividades_ActividadId",
                        column: x => x.ActividadId,
                        principalTable: "tbActividades",
                        principalColumn: "ActividadId");
                });

            migrationBuilder.CreateTable(
                name: "tbAlumnosGrupos",
                columns: table => new
                {
                    AlumnoGrupoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlumnoId = table.Column<int>(type: "int", nullable: false),
                    GrupoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAlumnosGrupos", x => x.AlumnoGrupoId);
                    table.ForeignKey(
                        name: "FK_tbAlumnosGrupos_tbAlumnos_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "tbAlumnos",
                        principalColumn: "AlumnoId");
                    table.ForeignKey(
                        name: "FK_tbAlumnosGrupos_tbGrupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "tbGrupos",
                        principalColumn: "GrupoId");
                });

            migrationBuilder.CreateTable(
                name: "tbEventosGrupos",
                columns: table => new
                {
                    EventoGrupoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaId = table.Column<int>(type: "int", nullable: false),
                    GrupoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbEventosGrupos", x => x.EventoGrupoId);
                    table.ForeignKey(
                        name: "FK_tbEventosGrupos_tbEventosAgenda_FechaId",
                        column: x => x.FechaId,
                        principalTable: "tbEventosAgenda",
                        principalColumn: "EventoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbEventosGrupos_tbGrupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "tbGrupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbAlumnosMaterias",
                columns: table => new
                {
                    AlumnoMateriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlumnoId = table.Column<int>(type: "int", nullable: false),
                    MateriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbAlumnosMaterias", x => x.AlumnoMateriaId);
                    table.ForeignKey(
                        name: "FK_tbAlumnosMaterias_tbAlumnos_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "tbAlumnos",
                        principalColumn: "AlumnoId");
                    table.ForeignKey(
                        name: "FK_tbAlumnosMaterias_tbMaterias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "tbMaterias",
                        principalColumn: "MateriaId");
                });

            migrationBuilder.CreateTable(
                name: "tbEventosMaterias",
                columns: table => new
                {
                    EventoMateriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaId = table.Column<int>(type: "int", nullable: false),
                    MateriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbEventosMaterias", x => x.EventoMateriaId);
                    table.ForeignKey(
                        name: "FK_tbEventosMaterias_tbEventosAgenda_FechaId",
                        column: x => x.FechaId,
                        principalTable: "tbEventosAgenda",
                        principalColumn: "EventoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbEventosMaterias_tbMaterias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "tbMaterias",
                        principalColumn: "MateriaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbGruposMaterias",
                columns: table => new
                {
                    GrupoMateriasId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrupoId = table.Column<int>(type: "int", nullable: false),
                    MateriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbGruposMaterias", x => x.GrupoMateriasId);
                    table.ForeignKey(
                        name: "FK_tbGruposMaterias_tbGrupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "tbGrupos",
                        principalColumn: "GrupoId");
                    table.ForeignKey(
                        name: "FK_tbGruposMaterias_tbMaterias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "tbMaterias",
                        principalColumn: "MateriaId");
                });

            migrationBuilder.CreateTable(
                name: "tbMateriasActividades",
                columns: table => new
                {
                    MateriaActividad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MateriaId = table.Column<int>(type: "int", nullable: false),
                    ActividadId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbMateriasActividades", x => x.MateriaActividad);
                    table.ForeignKey(
                        name: "FK_tbMateriasActividades_tbActividades_ActividadId",
                        column: x => x.ActividadId,
                        principalTable: "tbActividades",
                        principalColumn: "ActividadId");
                    table.ForeignKey(
                        name: "FK_tbMateriasActividades_tbMaterias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "tbMaterias",
                        principalColumn: "MateriaId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbActividades_TipoActividadId",
                table: "tbActividades",
                column: "TipoActividadId");

            migrationBuilder.CreateIndex(
                name: "IX_tbAlumnos_UserId",
                table: "tbAlumnos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tbAlumnosActividades_ActividadId",
                table: "tbAlumnosActividades",
                column: "ActividadId");

            migrationBuilder.CreateIndex(
                name: "IX_tbAlumnosGrupos_AlumnoId",
                table: "tbAlumnosGrupos",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_tbAlumnosGrupos_GrupoId",
                table: "tbAlumnosGrupos",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_tbAlumnosMaterias_AlumnoId",
                table: "tbAlumnosMaterias",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_tbAlumnosMaterias_MateriaId",
                table: "tbAlumnosMaterias",
                column: "MateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_tbAlumnosTokens_AlumnoId",
                table: "tbAlumnosTokens",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_tbAvisos_DocenteId",
                table: "tbAvisos",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_tbCalificaciones_ActividadId",
                table: "tbCalificaciones",
                column: "ActividadId");

            migrationBuilder.CreateIndex(
                name: "IX_tbDocentes_UserId",
                table: "tbDocentes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tbEventosAgenda_DocenteId",
                table: "tbEventosAgenda",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_tbEventosGrupos_FechaId",
                table: "tbEventosGrupos",
                column: "FechaId");

            migrationBuilder.CreateIndex(
                name: "IX_tbEventosGrupos_GrupoId",
                table: "tbEventosGrupos",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_tbEventosMaterias_FechaId",
                table: "tbEventosMaterias",
                column: "FechaId");

            migrationBuilder.CreateIndex(
                name: "IX_tbEventosMaterias_MateriaId",
                table: "tbEventosMaterias",
                column: "MateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_tbGrupos_DocenteId",
                table: "tbGrupos",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_tbGruposMaterias_GrupoId",
                table: "tbGruposMaterias",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_tbGruposMaterias_MateriaId",
                table: "tbGruposMaterias",
                column: "MateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_tbMaterias_DocenteId",
                table: "tbMaterias",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_tbMateriasActividades_ActividadId",
                table: "tbMateriasActividades",
                column: "ActividadId");

            migrationBuilder.CreateIndex(
                name: "IX_tbMateriasActividades_MateriaId",
                table: "tbMateriasActividades",
                column: "MateriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbAlumnosActividades");

            migrationBuilder.DropTable(
                name: "tbAlumnosGrupos");

            migrationBuilder.DropTable(
                name: "tbAlumnosMaterias");

            migrationBuilder.DropTable(
                name: "tbAlumnosTokens");

            migrationBuilder.DropTable(
                name: "tbAvisos");

            migrationBuilder.DropTable(
                name: "tbCalificaciones");

            migrationBuilder.DropTable(
                name: "tbEventosGrupos");

            migrationBuilder.DropTable(
                name: "tbEventosMaterias");

            migrationBuilder.DropTable(
                name: "tbGruposMaterias");

            migrationBuilder.DropTable(
                name: "tbMateriasActividades");

            migrationBuilder.DropTable(
                name: "tbEntregablesAlumno");

            migrationBuilder.DropTable(
                name: "tbAlumnos");

            migrationBuilder.DropTable(
                name: "tbEventosAgenda");

            migrationBuilder.DropTable(
                name: "tbGrupos");

            migrationBuilder.DropTable(
                name: "tbActividades");

            migrationBuilder.DropTable(
                name: "tbMaterias");

            migrationBuilder.DropTable(
                name: "cTiposActividades");

            migrationBuilder.DropTable(
                name: "tbDocentes");

            migrationBuilder.CreateTable(
                name: "Grupos",
                columns: table => new
                {
                    GrupoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoAcceso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NombreGrupo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupos", x => x.GrupoId);
                });

            migrationBuilder.CreateTable(
                name: "Materias",
                columns: table => new
                {
                    MateriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrupoId = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreMateria = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materias", x => x.MateriaId);
                    table.ForeignKey(
                        name: "FK_Materias_Grupos_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupos",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Materias_GrupoId",
                table: "Materias",
                column: "GrupoId");
        }
    }
}
