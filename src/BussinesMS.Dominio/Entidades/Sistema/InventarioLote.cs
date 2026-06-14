using BussinesMS.Dominio.Entidades.Compartido;
using BussinesMS.Dominio.Enums;

namespace BussinesMS.Dominio.Entidades.Sistema;

public class InventarioLote : EntidadBase
{
    public int VarianteId { get; set; }
    public int AlmacenId { get; set; }
    public int? CompraDetalleId { get; set; }
    public int StockInicial { get; set; }
    public int StockDisponible { get; set; }
    public int CantidadVencida { get; set; } = 0;
    public decimal CostoCompraUnitario { get; set; }
    public decimal PrecioVentaUnitario { get; set; }
    public decimal PrecioVentaMayoreo { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public EstadoLote EstadoLote { get; set; } = EstadoLote.Activo;

    public ProductoVariante? Variante { get; set; }
    public CompraDetalle? CompraDetalle { get; set; }
}
