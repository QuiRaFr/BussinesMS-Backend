using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinesMS.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarTipoPresentacionYProductoPresentacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquivalenciaUnidades",
                table: "ProductoPresentaciones");

            migrationBuilder.RenameColumn(
                name: "Factor",
                table: "TipoPresentaciones",
                newName: "EquivalenciaUnidades");

            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "ProductoPresentaciones",
                newName: "TipoPresentacionId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductoPresentaciones_VarianteId_Tipo",
                table: "ProductoPresentaciones",
                newName: "IX_ProductoPresentaciones_VarianteId_TipoPresentacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoPresentaciones_TipoPresentacionId",
                table: "ProductoPresentaciones",
                column: "TipoPresentacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoPresentaciones_TipoPresentaciones_TipoPresentacionId",
                table: "ProductoPresentaciones",
                column: "TipoPresentacionId",
                principalTable: "TipoPresentaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductoPresentaciones_TipoPresentaciones_TipoPresentacionId",
                table: "ProductoPresentaciones");

            migrationBuilder.DropIndex(
                name: "IX_ProductoPresentaciones_TipoPresentacionId",
                table: "ProductoPresentaciones");

            migrationBuilder.RenameColumn(
                name: "EquivalenciaUnidades",
                table: "TipoPresentaciones",
                newName: "Factor");

            migrationBuilder.RenameColumn(
                name: "TipoPresentacionId",
                table: "ProductoPresentaciones",
                newName: "Tipo");

            migrationBuilder.RenameIndex(
                name: "IX_ProductoPresentaciones_VarianteId_TipoPresentacionId",
                table: "ProductoPresentaciones",
                newName: "IX_ProductoPresentaciones_VarianteId_Tipo");

            migrationBuilder.AddColumn<int>(
                name: "EquivalenciaUnidades",
                table: "ProductoPresentaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
