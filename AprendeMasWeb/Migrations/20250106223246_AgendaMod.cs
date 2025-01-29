using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AprendeMasWeb.Migrations
{
    /// <inheritdoc />
    public partial class AgendaMod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaMarcada",
                table: "tbEventosAgenda",
                newName: "FechaInicio");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "tbEventosAgenda",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFinal",
                table: "tbEventosAgenda",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Titulo",
                table: "tbEventosAgenda",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "tbEventosAgenda");

            migrationBuilder.DropColumn(
                name: "FechaFinal",
                table: "tbEventosAgenda");

            migrationBuilder.DropColumn(
                name: "Titulo",
                table: "tbEventosAgenda");

            migrationBuilder.RenameColumn(
                name: "FechaInicio",
                table: "tbEventosAgenda",
                newName: "FechaMarcada");
        }
    }
}
