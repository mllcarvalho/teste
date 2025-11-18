using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oficina.Atendimento.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrdemDeServicoOrcamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataAprovacao",
                table: "Orcamentos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorPecas",
                table: "Orcamentos",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorServicos",
                table: "Orcamentos",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataAprovacao",
                table: "Orcamentos");

            migrationBuilder.DropColumn(
                name: "ValorPecas",
                table: "Orcamentos");

            migrationBuilder.DropColumn(
                name: "ValorServicos",
                table: "Orcamentos");
        }
    }
}
