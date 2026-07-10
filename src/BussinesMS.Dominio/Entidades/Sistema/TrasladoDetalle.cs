using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Dominio.Entidades.Sistema;

public class TrasladoDetalle : EntidadBase
{
    public int TrasladoId { get; set; }
    public int LoteAlmacenOrigenId { get; set; }
    public int? LoteAlmacenDestinoId { get; set; }
    public int CantidadUnidades { get; set; }
    public decimal CostoUnitarioCapturado { get; set; }

    public Traslado? Traslado { get; set; }
    public InventarioLoteAlmacen? LoteAlmacenOrigen { get; set; }
    public InventarioLoteAlmacen? LoteAlmacenDestino { get; set; }
}
