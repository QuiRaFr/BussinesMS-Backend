namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class CompraDetalleDto
{
    public int Id { get; set; }
    public int CompraId { get; set; }
    public int VarianteId { get; set; }
    public string? VarianteNombre { get; set; }
    public int AlmacenId { get; set; }
    public int CantidadUnidades { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public DateTime? FechaVencimiento { get; set; }
}

public class CrearCompraDetalleDto
{
    public int VarianteId { get; set; }
    public int AlmacenId { get; set; }
    public int CantidadUnidades { get; set; }
    public decimal CostoUnitario { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public decimal PrecioVentaUnitario { get; set; }
    public decimal PrecioVentaMayoreo { get; set; }
}
