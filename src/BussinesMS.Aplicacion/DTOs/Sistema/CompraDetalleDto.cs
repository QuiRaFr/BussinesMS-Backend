namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class CompraDetalleDto
{
    public int Id { get; set; }
    public int CompraId { get; set; }
    public int VarianteId { get; set; }
    public string? VarianteNombre { get; set; }
    public int CantidadUnidades { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public List<CompraDetalleAlmacenDto> Almacenes { get; set; } = new();
}

public class CompraDetalleAlmacenDto
{
    public int InventarioLoteAlmacenId { get; set; }
    public int AlmacenId { get; set; }
    public int CantidadUnidades { get; set; }
    public int StockDisponible { get; set; }
}

public class CrearCompraDetalleDto
{
    public int VarianteId { get; set; }
    public decimal CostoUnitario { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public bool ActualizarPrecioVenta { get; set; }
    public decimal PrecioVentaUnitario { get; set; }
    public decimal PrecioVentaMayor { get; set; }
    public List<CrearCompraDetalleAlmacenDto> Almacenes { get; set; } = new();
}

public class CrearCompraDetalleAlmacenDto
{
    public int AlmacenId { get; set; }
    public int CantidadUnidades { get; set; }
}
