using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinesMS.Infraestructura.Migrations.SistemaDb
{
    /// <inheritdoc />
    public partial class AddDevolucionesClientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DevolucionesClientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoteOrigenId = table.Column<int>(type: "int", nullable: false),
                    LoteDevueltoId = table.Column<int>(type: "int", nullable: true),
                    VarianteId = table.Column<int>(type: "int", nullable: false),
                    AlmacenId = table.Column<int>(type: "int", nullable: false),
                    CantidadUnidades = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EstadoDevolucion = table.Column<int>(type: "int", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FechaDevolucion = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_DevolucionesClientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DevolucionesClientes_InventarioLotes_LoteDevueltoId",
                        column: x => x.LoteDevueltoId,
                        principalTable: "InventarioLotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DevolucionesClientes_InventarioLotes_LoteOrigenId",
                        column: x => x.LoteOrigenId,
                        principalTable: "InventarioLotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DevolucionesClientes_ProductoVariantes_VarianteId",
                        column: x => x.VarianteId,
                        principalTable: "ProductoVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DevolucionesClientes_LoteDevueltoId",
                table: "DevolucionesClientes",
                column: "LoteDevueltoId");

            migrationBuilder.CreateIndex(
                name: "IX_DevolucionesClientes_LoteOrigenId",
                table: "DevolucionesClientes",
                column: "LoteOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_DevolucionesClientes_VarianteId",
                table: "DevolucionesClientes",
                column: "VarianteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DevolucionesClientes");
        }
    }
}
