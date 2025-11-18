using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Oficina.Estoque.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPecaSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Movimentos",
                table: "Movimentos");

            migrationBuilder.RenameTable(
                name: "Movimentos",
                newName: "Estoque");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Estoque",
                table: "Estoque",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Pecas",
                columns: new[] { "Id", "Codigo", "DataAtualizacao", "DataCriacao", "Descricao", "Fabricante", "Nome", "Preco", "Quantidade", "QuantidadeMinima", "Tipo" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "FILTRO-001", new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(4876), new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(4874), "Filtro de óleo sintético para motores 1.0 a 2.0", "Bosch", "Filtro de óleo", 45.90m, 10, 2, 0 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "PAST-FREIO-002", new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6822), new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6821), "Pastilha de freio dianteira para veículos leves", "TRW", "Pastilha de freio", 120.00m, 8, 2, 0 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "OLEO-5W30-003", new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6827), new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6826), "Óleo lubrificante sintético 5W30", "Mobil", "Óleo 5W30", 32.50m, 20, 5, 1 },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "FILTRO-AR-004", new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6831), new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6830), "Filtro de ar para motores flex", "Mann", "Filtro de ar", 38.00m, 6, 2, 0 },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "COR-DENT-005", new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6834), new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6834), "Correia dentada para motores 1.6", "Gates", "Correia dentada", 85.00m, 4, 1, 0 },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "VELA-IGN-006", new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6838), new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6837), "Vela de ignição para motores flex", "NGK", "Vela de ignição", 22.00m, 16, 4, 0 },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "AMORT-D-007", new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6841), new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6841), "Amortecedor dianteiro para veículos compactos", "Monroe", "Amortecedor dianteiro", 210.00m, 2, 2, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Estoque",
                table: "Estoque");

            migrationBuilder.DeleteData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.RenameTable(
                name: "Estoque",
                newName: "Movimentos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Movimentos",
                table: "Movimentos",
                column: "Id");
        }
    }
}
