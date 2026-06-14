using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Dominio.Entidades.Sistema;

public class Traslado : EntidadBase
{
    public int LoteId { get; set; }
    public int? LoteDestinoId { get; set; }
    public int AlmacenOrigenId { get; set; }
    public int AlmacenDestinoId { get; set; }
    public int CantidadUnidades { get; set; }
    public string? Observacion { get; set; }
    public DateTime FechaTraslado { get; set; } = DateTime.UtcNow;
    public int UsuarioId { get; set; }

    public InventarioLote? Lote { get; set; }
    public InventarioLote? LoteDestino { get; set; }
}
