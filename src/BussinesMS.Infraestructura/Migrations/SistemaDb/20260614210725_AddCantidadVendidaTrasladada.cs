using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinesMS.Infraestructura.Migrations.SistemaDb
{
    /// <inheritdoc />
    public partial class AddCantidadVendidaTrasladada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CantidadTrasladada",
                table: "InventarioLotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CantidadVendida",
                table: "InventarioLotes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantidadTrasladada",
                table: "InventarioLotes");

            migrationBuilder.DropColumn(
                name: "CantidadVendida",
                table: "InventarioLotes");
        }
    }
}
