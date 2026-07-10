using BussinesMS.Dominio.Entidades.Compartido;
using BussinesMS.Dominio.Enums;

namespace BussinesMS.Dominio.Entidades.Sistema;

public class InventarioLoteAlmacen : EntidadBase
{
    public int LoteId { get; set; }
    public int VarianteId { get; set; }
    public int AlmacenId { get; set; }
    public int StockInicial { get; set; }
    public int StockDisponible { get; set; }
    public int CantidadVendida { get; set; } = 0;
    public int CantidadTrasladada { get; set; } = 0;
    public int CantidadVencida { get; set; } = 0;
    public EstadoLote EstadoLote { get; set; } = EstadoLote.Activo;

    public InventarioLote? Lote { get; set; }
}
