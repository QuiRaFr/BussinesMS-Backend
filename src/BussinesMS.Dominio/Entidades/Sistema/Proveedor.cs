using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Dominio.Entidades.Sistema;

public class Proveedor : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public string? Nit { get; set; }
    public string? Telefono { get; set; }
}
