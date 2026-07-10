using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Dominio.Entidades.Sistema;

public class InventarioLote : EntidadBase
{
    public int VarianteId { get; set; }
    public int? CompraDetalleId { get; set; }
    public decimal CostoCompraUnitario { get; set; }
    public int CantidadTotal { get; set; }
    public DateTime? FechaVencimiento { get; set; }

    public ProductoVariante? Variante { get; set; }
    public CompraDetalle? CompraDetalle { get; set; }
}
