using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinesMS.Infraestructura.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class AddPresentaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Caja",
                table: "ProductoVariantes");

            migrationBuilder.DropColumn(
                name: "Display",
                table: "ProductoVariantes");

            migrationBuilder.DropColumn(
                name: "TipoVenta",
                table: "ProductoVariantes");

            migrationBuilder.DropColumn(
                name: "Unidad",
                table: "ProductoVariantes");

            migrationBuilder.RenameIndex(
                name: "IX_ProductoVariantes_ProductoId_SaborId_TamanioId",
                table: "ProductoVariantes",
                newName: "UQ_Variante_Combinacion");

            migrationBuilder.CreateTable(
                name: "TiposPresentacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUsuarioId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUsuarioId = table.Column<int>(type: "int", nullable: true),
                    DeletedByUsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPresentacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductoPresentaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VarianteId = table.Column<int>(type: "int", nullable: false),
                    TipoPresentacionId = table.Column<int>(type: "int", nullable: false),
                    NombrePersonalizado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CantidadDePadre = table.Column<int>(type: "int", nullable: false),
                    PresentacionPadreId = table.Column<int>(type: "int", nullable: true),
                    EsDefaultReporte = table.Column<bool>(type: "bit", nullable: false),
                    CodigoBarras = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUsuarioId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUsuarioId = table.Column<int>(type: "int", nullable: true),
                    DeletedByUsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoPresentaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoPresentaciones_ProductoPresentaciones_PresentacionPadreId",
                        column: x => x.PresentacionPadreId,
                        principalTable: "ProductoPresentaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductoPresentaciones_ProductoVariantes_VarianteId",
                        column: x => x.VarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductoPresentaciones_TiposPresentacion_TipoPresentacionId",
                        column: x => x.TipoPresentacionId,
                        principalTable: "TiposPresentacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductoVariantes_CodigoBarras",
                table: "ProductoVariantes",
                column: "CodigoBarras",
                unique: true,
                filter: "[CodigoBarras] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoPresentaciones_CodigoBarras",
                table: "ProductoPresentaciones",
                column: "CodigoBarras",
                unique: true,
                filter: "[CodigoBarras] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoPresentaciones_PresentacionPadreId",
                table: "ProductoPresentaciones",
                column: "PresentacionPadreId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoPresentaciones_TipoPresentacionId",
                table: "ProductoPresentaciones",
                column: "TipoPresentacionId");

            migrationBuilder.CreateIndex(
                name: "UQ_Presentacion_DefaultReporte",
                table: "ProductoPresentaciones",
                column: "VarianteId",
                unique: true,
                filter: "[EsDefaultReporte] = 1");

            migrationBuilder.CreateIndex(
                name: "UQ_Presentacion_Variante",
                table: "ProductoPresentaciones",
                columns: new[] { "VarianteId", "TipoPresentacionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposPresentacion_Nombre",
                table: "TiposPresentacion",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposPresentacion_Orden",
                table: "TiposPresentacion",
                column: "Orden",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductoPresentaciones");

            migrationBuilder.DropTable(
                name: "TiposPresentacion");

            migrationBuilder.DropIndex(
                name: "IX_ProductoVariantes_CodigoBarras",
                table: "ProductoVariantes");

            migrationBuilder.RenameIndex(
                name: "UQ_Variante_Combinacion",
                table: "ProductoVariantes",
                newName: "IX_ProductoVariantes_ProductoId_SaborId_TamanioId");

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
                name: "TipoVenta",
                table: "ProductoVariantes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Unidad",
                table: "ProductoVariantes",
                type: "int",
                nullable: true);
        }
    }
}
