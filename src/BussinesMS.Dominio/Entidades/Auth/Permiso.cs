using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Dominio.Entidades.Auth;

public class Permiso : EntidadBase
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Tipo { get; set; } = "general"; 
    public string Categoria { get; set; } = string.Empty; 
    public int? MenuId { get; set; }
    public Menu? Menu { get; set; }
    
    public ICollection<PermisoRol> PermisoRoles { get; set; } = new List<PermisoRol>();
}

public class PermisoRol
{
    public int PermisoId { get; set; }
    public Permiso Permiso { get; set; } = null!;
    
    public int RolId { get; set; }
    public Rol Rol { get; set; } = null!;
}