using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinesMS.Infraestructura.Migrations.SistemaDb
{
    /// <inheritdoc />
    public partial class RefactorCompraLotesPorVariante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlmacenId",
                table: "CompraDetalles");

            migrationBuilder.AddColumn<int>(
                name: "CantidadTotal",
                table: "InventarioLotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VarianteId",
                table: "InventarioLoteAlmacenes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LoteAlmacen_VarianteAlmacen",
                table: "InventarioLoteAlmacenes",
                columns: new[] { "VarianteId", "AlmacenId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LoteAlmacen_VarianteAlmacen",
                table: "InventarioLoteAlmacenes");

            migrationBuilder.DropColumn(
                name: "CantidadTotal",
                table: "InventarioLotes");

            migrationBuilder.DropColumn(
                name: "VarianteId",
                table: "InventarioLoteAlmacenes");

            migrationBuilder.AddColumn<int>(
                name: "AlmacenId",
                table: "CompraDetalles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
