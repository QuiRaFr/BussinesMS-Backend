using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinesMS.Infraestructura.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class AgregarCamposCompra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegistradoByUsuarioId",
                table: "PagosCompra",
                newName: "PagadoPorUsuarioId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PagosCompra",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUsuarioId",
                table: "PagosCompra",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PagosCompra",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUsuarioId",
                table: "PagosCompra",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PagosCompra",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PagosCompra",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUsuarioId",
                table: "PagosCompra",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EstaLiquidada",
                table: "Compras",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NumeroFactura",
                table: "Compras",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PagosCompra");

            migrationBuilder.DropColumn(
                name: "CreatedByUsuarioId",
                table: "PagosCompra");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PagosCompra");

            migrationBuilder.DropColumn(
                name: "DeletedByUsuarioId",
                table: "PagosCompra");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PagosCompra");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PagosCompra");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsuarioId",
                table: "PagosCompra");

            migrationBuilder.DropColumn(
                name: "EstaLiquidada",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "NumeroFactura",
                table: "Compras");

            migrationBuilder.RenameColumn(
                name: "PagadoPorUsuarioId",
                table: "PagosCompra",
                newName: "RegistradoByUsuarioId");
        }
    }
}
