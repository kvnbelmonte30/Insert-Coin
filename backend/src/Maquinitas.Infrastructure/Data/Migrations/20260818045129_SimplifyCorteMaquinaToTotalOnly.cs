using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maquinitas.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyCorteMaquinaToTotalOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorteMaquinaDetalles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CorteMaquinaDetalles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CorteMaquinaId = table.Column<Guid>(type: "uuid", nullable: false),
                    DenominacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    PremioId = table.Column<Guid>(type: "uuid", nullable: true),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorteMaquinaDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorteMaquinaDetalles_CortesMaquina_CorteMaquinaId",
                        column: x => x.CorteMaquinaId,
                        principalTable: "CortesMaquina",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorteMaquinaDetalles_Denominaciones_DenominacionId",
                        column: x => x.DenominacionId,
                        principalTable: "Denominaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CorteMaquinaDetalles_Premios_PremioId",
                        column: x => x.PremioId,
                        principalTable: "Premios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorteMaquinaDetalles_CorteMaquinaId",
                table: "CorteMaquinaDetalles",
                column: "CorteMaquinaId");

            migrationBuilder.CreateIndex(
                name: "IX_CorteMaquinaDetalles_DenominacionId",
                table: "CorteMaquinaDetalles",
                column: "DenominacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CorteMaquinaDetalles_PremioId",
                table: "CorteMaquinaDetalles",
                column: "PremioId");
        }
    }
}
