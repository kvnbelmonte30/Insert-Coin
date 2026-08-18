using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maquinitas.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceGastoTipoWithCategoriaGasto : Migration
    {
        /// <inheritdoc />
        private const string GeneralId = "00000000-0000-0000-0000-000000000001";
        private const string ReposicionId = "00000000-0000-0000-0000-000000000002";
        private const string SueldosId = "00000000-0000-0000-0000-000000000003";
        private const string DepositoId = "00000000-0000-0000-0000-000000000004";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriasGasto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasGasto", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CategoriasGasto",
                columns: new[] { "Id", "Nombre", "Activo" },
                values: new object[,]
                {
                    { new Guid(GeneralId), "Gasto general", true },
                    { new Guid(ReposicionId), "Reposición de fondo de máquina", true },
                    { new Guid(SueldosId), "Sueldos de empleados", true },
                    { new Guid(DepositoId), "Depósito a administrador", true }
                });

            migrationBuilder.AddColumn<Guid>(
                name: "CategoriaGastoId",
                table: "Gastos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid(GeneralId));

            // Preserva la categoría de los gastos ya registrados según su Tipo (enum) anterior.
            migrationBuilder.Sql($@"
                UPDATE ""Gastos"" SET ""CategoriaGastoId"" = CASE ""Tipo""
                    WHEN 0 THEN '{GeneralId}'
                    WHEN 1 THEN '{ReposicionId}'
                    WHEN 2 THEN '{SueldosId}'
                    WHEN 3 THEN '{DepositoId}'
                    ELSE '{GeneralId}'
                END::uuid;
            ");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Gastos");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_CategoriaGastoId",
                table: "Gastos",
                column: "CategoriaGastoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gastos_CategoriasGasto_CategoriaGastoId",
                table: "Gastos",
                column: "CategoriaGastoId",
                principalTable: "CategoriasGasto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gastos_CategoriasGasto_CategoriaGastoId",
                table: "Gastos");

            migrationBuilder.DropTable(
                name: "CategoriasGasto");

            migrationBuilder.DropIndex(
                name: "IX_Gastos_CategoriaGastoId",
                table: "Gastos");

            migrationBuilder.DropColumn(
                name: "CategoriaGastoId",
                table: "Gastos");

            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "Gastos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
