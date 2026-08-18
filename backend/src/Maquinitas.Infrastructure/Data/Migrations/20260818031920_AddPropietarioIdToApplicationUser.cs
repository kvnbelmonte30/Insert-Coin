using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maquinitas.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPropietarioIdToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PropietarioId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PropietarioId",
                table: "AspNetUsers",
                column: "PropietarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Propietarios_PropietarioId",
                table: "AspNetUsers",
                column: "PropietarioId",
                principalTable: "Propietarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Propietarios_PropietarioId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PropietarioId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PropietarioId",
                table: "AspNetUsers");
        }
    }
}
