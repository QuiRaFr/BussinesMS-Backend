using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinesMS.Infraestructura.Migrations.SistemaDb
{
    /// <inheritdoc />
    public partial class AddInventarioLotesYMovimientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventarioLotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VarianteId = table.Column<int>(type: "int", nullable: false),
                    AlmacenId = table.Column<int>(type: "int", nullable: false),
                    CompraDetalleId = table.Column<int>(type: "int", nullable: true),
                    StockInicial = table.Column<int>(type: "int", nullable: false),
                    StockDisponible = table.Column<int>(type: "int", nullable: false),
                    CantidadVencida = table.Column<int>(type: "int", nullable: false),
                    CostoCompraUnitario = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PrecioVentaUnitario = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    PrecioVentaMayoreo = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "date", nullable: true),
                    EstadoLote = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_InventarioLotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventarioLotes_CompraDetalles_CompraDetalleId",
                        column: x => x.CompraDetalleId,
                        principalTable: "CompraDetalles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InventarioLotes_ProductoVariantes_VarianteId",
                        column: x => x.VarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosInventario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoteId = table.Column<int>(type: "int", nullable: false),
                    VarianteId = table.Column<int>(type: "int", nullable: false),
                    AlmacenOrigenId = table.Column<int>(type: "int", nullable: true),
                    AlmacenDestinoId = table.Column<int>(type: "int", nullable: true),
                    TipoMovimiento = table.Column<int>(type: "int", nullable: false),
                    CantidadUnidades = table.Column<int>(type: "int", nullable: false),
                    SaldoResultante = table.Column<int>(type: "int", nullable: false),
                    ReferenciaId = table.Column<int>(type: "int", nullable: true),
                    Observacion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosInventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_InventarioLotes_LoteId",
                        column: x => x.LoteId,
                        principalTable: "InventarioLotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_ProductoVariantes_VarianteId",
                        column: x => x.VarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Traslados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoteId = table.Column<int>(type: "int", nullable: false),
                    LoteDestinoId = table.Column<int>(type: "int", nullable: true),
                    AlmacenOrigenId = table.Column<int>(type: "int", nullable: false),
                    AlmacenDestinoId = table.Column<int>(type: "int", nullable: false),
                    CantidadUnidades = table.Column<int>(type: "int", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FechaTraslado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Traslados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Traslados_InventarioLotes_LoteDestinoId",
                        column: x => x.LoteDestinoId,
                        principalTable: "InventarioLotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Traslados_InventarioLotes_LoteId",
                        column: x => x.LoteId,
                        principalTable: "InventarioLotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventarioLotes_CompraDetalleId",
                table: "InventarioLotes",
                column: "CompraDetalleId");

            migrationBuilder.CreateIndex(
                name: "IX_Lote_FEFO",
                table: "InventarioLotes",
                columns: new[] { "VarianteId", "AlmacenId", "FechaVencimiento" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_AlmacenOrigenId_AlmacenDestinoId",
                table: "MovimientosInventario",
                columns: new[] { "AlmacenOrigenId", "AlmacenDestinoId" });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_LoteId",
                table: "MovimientosInventario",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_VarianteId",
                table: "MovimientosInventario",
                column: "VarianteId");

            migrationBuilder.CreateIndex(
                name: "IX_Traslados_LoteDestinoId",
                table: "Traslados",
                column: "LoteDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Traslados_LoteId",
                table: "Traslados",
                column: "LoteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimientosInventario");

            migrationBuilder.DropTable(
                name: "Traslados");

            migrationBuilder.DropTable(
                name: "InventarioLotes");
        }
    }
}
