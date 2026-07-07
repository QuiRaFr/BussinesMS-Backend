using BussinesMS.Dominio.Entidades.Compartido;
using BussinesMS.Dominio.Enums;

namespace BussinesMS.Dominio.Entidades.Sistema;

public class Traslado : EntidadBase
{
    public TipoTraslado TipoTraslado { get; set; }
    public int VarianteId { get; set; }
    public int AlmacenOrigenId { get; set; }
    public int AlmacenDestinoId { get; set; }
    public int CantidadUnidades { get; set; }
    public string? Observacion { get; set; }
    public DateTime FechaTraslado { get; set; } = DateTime.UtcNow;
    public int UsuarioId { get; set; }

    public ProductoVariante? Variante { get; set; }
    public List<TrasladoDetalle> Detalles { get; set; } = new();
}
