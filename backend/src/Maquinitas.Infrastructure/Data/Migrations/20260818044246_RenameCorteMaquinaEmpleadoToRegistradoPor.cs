using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maquinitas.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameCorteMaquinaEmpleadoToRegistradoPor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CortesMaquina_AspNetUsers_EmpleadoId",
                table: "CortesMaquina");

            migrationBuilder.RenameColumn(
                name: "EmpleadoId",
                table: "CortesMaquina",
                newName: "RegistradoPorId");

            migrationBuilder.RenameIndex(
                name: "IX_CortesMaquina_EmpleadoId",
                table: "CortesMaquina",
                newName: "IX_CortesMaquina_RegistradoPorId");

            migrationBuilder.AddForeignKey(
                name: "FK_CortesMaquina_AspNetUsers_RegistradoPorId",
                table: "CortesMaquina",
                column: "RegistradoPorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CortesMaquina_AspNetUsers_RegistradoPorId",
                table: "CortesMaquina");

            migrationBuilder.RenameColumn(
                name: "RegistradoPorId",
                table: "CortesMaquina",
                newName: "EmpleadoId");

            migrationBuilder.RenameIndex(
                name: "IX_CortesMaquina_RegistradoPorId",
                table: "CortesMaquina",
                newName: "IX_CortesMaquina_EmpleadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_CortesMaquina_AspNetUsers_EmpleadoId",
                table: "CortesMaquina",
                column: "EmpleadoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
