using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Dominio.Entidades.Sistema;

public class TipoPresentacion : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public int Factor { get; set; }
}