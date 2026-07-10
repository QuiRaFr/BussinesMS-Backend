namespace BussinesMS.Dominio.Entidades.Auth;

public class UsuarioMenu
{
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public int MenuId { get; set; }
    public Menu? Menu { get; set; }

    // Permisos CRUD
    public bool Leer { get; set; } = true;
    public bool Crear { get; set; } = true;
    public bool Editar { get; set; } = true;
    public bool Eliminar { get; set; } = true;

    // Permisos especiales: ["aprobar_descuento", "anular_venta"]
    public string? PermisosEspeciales { get; set; }
}