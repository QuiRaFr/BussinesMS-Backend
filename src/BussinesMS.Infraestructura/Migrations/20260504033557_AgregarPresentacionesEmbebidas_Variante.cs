using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinesMS.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPresentacionesEmbebidas_Variante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Caja",
                table: "ProductoVariantes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Display",
                table: "ProductoVariantes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Unidad",
                table: "ProductoVariantes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Caja",
                table: "ProductoVariantes");

            migrationBuilder.DropColumn(
                name: "Display",
                table: "ProductoVariantes");

            migrationBuilder.DropColumn(
                name: "Unidad",
                table: "ProductoVariantes");
        }
    }
}
