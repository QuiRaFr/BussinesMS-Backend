using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinesMS.Infraestructura.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class RemovePermisosFromRolAndUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Permisos",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Permisos",
                table: "Roles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Permisos",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Permisos",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
