using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinesMS.Infraestructura.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class AddSistemaIdToMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SistemaId",
                table: "Menus",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Menus_SistemaId",
                table: "Menus",
                column: "SistemaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Sistemas_SistemaId",
                table: "Menus",
                column: "SistemaId",
                principalTable: "Sistemas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Sistemas_SistemaId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Menus_SistemaId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "SistemaId",
                table: "Menus");
        }
    }
}
