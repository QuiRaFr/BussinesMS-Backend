namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class VarianteStockDto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string? NombreProducto { get; set; }
    public string? DescripcionProducto { get; set; }
    public string? CodigoBarras { get; set; }
    public decimal PrecioVentaUnitario { get; set; }
    public decimal PrecioVentaMayoreo { get; set; }
    public decimal PrecioCompra { get; set; }
    public string? CodigoAlmacen { get; set; }
    public bool IsActive { get; set; }
    public int? CategoriaId { get; set; }
    public string? CategoriaNombre { get; set; }
    public int StockDisponible { get; set; }
    public int CantidadVendida { get; set; }
    public int CantidadTrasladada { get; set; }
    public int CantidadVencida { get; set; }
}

public class VarianteStockDetalleDto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string? NombreProducto { get; set; }
    public string? DescripcionProducto { get; set; }
    public string? CodigoBarras { get; set; }
    public decimal PrecioVentaUnitario { get; set; }
    public decimal PrecioVentaMayoreo { get; set; }
    public decimal PrecioCompra { get; set; }
    public string? CodigoAlmacen { get; set; }
    public bool IsActive { get; set; }
    public int? CategoriaId { get; set; }
    public string? CategoriaNombre { get; set; }
    public int StockDisponible { get; set; }
    public int CantidadVendida { get; set; }
    public int CantidadTrasladada { get; set; }
    public int CantidadVencida { get; set; }
    public List<InventarioLoteDto> Lotes { get; set; } = [];
}
