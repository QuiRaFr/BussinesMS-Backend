using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Dominio.Entidades.Auth;

public class Rol : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public string? MenuIds { get; set; }
    
    public ICollection<PermisoRol> PermisoRoles { get; set; } = new List<PermisoRol>();
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}