using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace BussinesMS.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class FixUniqueIndexCompuesto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categorias_Nombre",
                table: "Categorias");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nombre_ParentId",
                table: "Categorias",
                columns: new[] { "Nombre", "ParentId" },
                unique: true,
                filter: "[ParentId] IS NOT NULL");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categorias_Nombre_ParentId",
                table: "Categorias");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nombre",
                table: "Categorias",
                columns: new[] { "Nombre" },
                unique: true);
        }
    }
}