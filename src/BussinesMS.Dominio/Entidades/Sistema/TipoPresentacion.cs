using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Dominio.Entidades.Sistema;

public class TipoPresentacion : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;  // "Unidad", "Caja", "Caja2", "Caja3"
    public int Orden { get; set; }                       // 1=Unidad, 2=Caja, 3=Caja2 ...
    public ICollection<ProductoPresentacion> Presentaciones { get; set; } = [];
}