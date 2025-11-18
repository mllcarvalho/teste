using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oficina.Atendimento.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OsUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataConclusao",
                table: "OrdensDeServico",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataFinalizacao",
                table: "OrdensDeServico",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInicioExecucao",
                table: "OrdensDeServico",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataConclusao",
                table: "OrdensDeServico");

            migrationBuilder.DropColumn(
                name: "DataFinalizacao",
                table: "OrdensDeServico");

            migrationBuilder.DropColumn(
                name: "DataInicioExecucao",
                table: "OrdensDeServico");
        }
    }
}
