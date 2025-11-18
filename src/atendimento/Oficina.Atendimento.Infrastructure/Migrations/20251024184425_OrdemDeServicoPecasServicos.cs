using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oficina.Atendimento.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrdemDeServicoPecasServicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NomeServico",
                table: "OrdemDeServicoServicos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ServicoId",
                table: "OrdemDeServicoServicos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "NomePeca",
                table: "OrdemDeServicoPecas",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "PecaId",
                table: "OrdemDeServicoPecas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Quantidade",
                table: "OrdemDeServicoPecas",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomeServico",
                table: "OrdemDeServicoServicos");

            migrationBuilder.DropColumn(
                name: "ServicoId",
                table: "OrdemDeServicoServicos");

            migrationBuilder.DropColumn(
                name: "NomePeca",
                table: "OrdemDeServicoPecas");

            migrationBuilder.DropColumn(
                name: "PecaId",
                table: "OrdemDeServicoPecas");

            migrationBuilder.DropColumn(
                name: "Quantidade",
                table: "OrdemDeServicoPecas");
        }
    }
}
