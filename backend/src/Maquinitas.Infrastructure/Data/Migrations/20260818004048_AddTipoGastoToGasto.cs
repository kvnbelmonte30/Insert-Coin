using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maquinitas.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoGastoToGasto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "Gastos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Gastos");
        }
    }
}
