using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oficina.Estoque.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPecaSeedWithDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(4876), new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(4874) });

            migrationBuilder.UpdateData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6822), new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6821) });

            migrationBuilder.UpdateData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6827), new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6826) });

            migrationBuilder.UpdateData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6831), new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6830) });

            migrationBuilder.UpdateData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6834), new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6834) });

            migrationBuilder.UpdateData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6838), new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6837) });

            migrationBuilder.UpdateData(
                table: "Pecas",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6841), new DateTime(2025, 10, 16, 13, 30, 3, 519, DateTimeKind.Utc).AddTicks(6841) });
        }
    }
}
