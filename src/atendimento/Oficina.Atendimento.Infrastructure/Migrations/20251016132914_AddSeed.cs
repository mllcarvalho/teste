using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Oficina.Atendimento.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Servicos",
                columns: new[] { "Id", "DataAtualizacao", "DataCriacao", "Nome", "Preco" },
                values: new object[,]
                {
                    { new Guid("11111111-aaaa-aaaa-aaaa-111111111111"), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(5645), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(5641), "Troca de óleo", 100m },
                    { new Guid("22222222-bbbb-bbbb-bbbb-222222222222"), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6335), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6334), "Alinhamento", 150m },
                    { new Guid("33333333-cccc-cccc-cccc-333333333333"), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6341), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6340), "Balanceamento", 120m },
                    { new Guid("44444444-dddd-dddd-dddd-444444444444"), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6345), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6344), "Troca de filtro de ar", 80m },
                    { new Guid("55555555-eeee-eeee-eeee-555555555555"), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6348), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6348), "Troca de pastilha de freio", 200m },
                    { new Guid("66666666-ffff-ffff-ffff-666666666666"), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6353), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6352), "Revisão completa", 350m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("11111111-aaaa-aaaa-aaaa-111111111111"));

            migrationBuilder.DeleteData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-bbbb-bbbb-bbbb-222222222222"));

            migrationBuilder.DeleteData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("33333333-cccc-cccc-cccc-333333333333"));

            migrationBuilder.DeleteData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("44444444-dddd-dddd-dddd-444444444444"));

            migrationBuilder.DeleteData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("55555555-eeee-eeee-eeee-555555555555"));

            migrationBuilder.DeleteData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("66666666-ffff-ffff-ffff-666666666666"));
        }
    }
}
