using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Dominio.Entidades.Auth;

public class Menu : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Icono { get; set; }
    public int? Orden { get; set; }
    public string? JerarquiaName { get; set; }
    public int? SistemaId { get; set; }
    public Sistema? Sistema { get; set; }
    public int? PermisoId { get; set; }
    public Permiso? Permiso { get; set; }
}