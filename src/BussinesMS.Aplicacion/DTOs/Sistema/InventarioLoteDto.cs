namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class InventarioLoteDto
{
    public int Id { get; set; }
    public int VarianteId { get; set; }
    public string? VarianteNombre { get; set; }
    public int AlmacenId { get; set; }
    public string? AlmacenNombre { get; set; }
    public int? CompraDetalleId { get; set; }
    public int StockInicial { get; set; }
    public int StockDisponible { get; set; }
    public int CantidadVendida { get; set; }
    public int CantidadTrasladada { get; set; }
    public int CantidadVencida { get; set; }
    public decimal CostoCompraUnitario { get; set; }
    public decimal PrecioVentaUnitario { get; set; }
    public decimal PrecioVentaMayoreo { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public int? DiasParaVencer { get; set; }
    public int EstadoLote { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CrearInventarioLoteDto
{
    public int VarianteId { get; set; }
    public int AlmacenId { get; set; }
    public int? CompraDetalleId { get; set; }
    public int StockInicial { get; set; }
    public decimal CostoCompraUnitario { get; set; }
    public decimal PrecioVentaUnitario { get; set; }
    public decimal PrecioVentaMayoreo { get; set; }
    public DateTime? FechaVencimiento { get; set; }
}

public class ActualizarInventarioLoteDto
{
    public int Id { get; set; }
    public int VarianteId { get; set; }
    public int AlmacenId { get; set; }
    public int? CompraDetalleId { get; set; }
    public int StockInicial { get; set; }
    public int StockDisponible { get; set; }
    public int CantidadVendida { get; set; }
    public int CantidadTrasladada { get; set; }
    public int CantidadVencida { get; set; }
    public decimal CostoCompraUnitario { get; set; }
    public decimal PrecioVentaUnitario { get; set; }
    public decimal PrecioVentaMayoreo { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public int EstadoLote { get; set; }
    public bool IsActive { get; set; }
}

public class AjusteInventarioDto
{
    public int CantidadAjuste { get; set; }
    public string? Observacion { get; set; }
}
