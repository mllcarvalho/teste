using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oficina.Atendimento.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("11111111-aaaa-aaaa-aaaa-111111111111"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-bbbb-bbbb-bbbb-222222222222"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("33333333-cccc-cccc-cccc-333333333333"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("44444444-dddd-dddd-dddd-444444444444"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("55555555-eeee-eeee-eeee-555555555555"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("66666666-ffff-ffff-ffff-666666666666"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("11111111-aaaa-aaaa-aaaa-111111111111"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(5645), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(5641) });

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("22222222-bbbb-bbbb-bbbb-222222222222"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6335), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6334) });

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("33333333-cccc-cccc-cccc-333333333333"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6341), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6340) });

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("44444444-dddd-dddd-dddd-444444444444"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6345), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6344) });

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("55555555-eeee-eeee-eeee-555555555555"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6348), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6348) });

            migrationBuilder.UpdateData(
                table: "Servicos",
                keyColumn: "Id",
                keyValue: new Guid("66666666-ffff-ffff-ffff-666666666666"),
                columns: new[] { "DataAtualizacao", "DataCriacao" },
                values: new object[] { new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6353), new DateTime(2025, 10, 16, 13, 29, 13, 237, DateTimeKind.Utc).AddTicks(6352) });
        }
    }
}
