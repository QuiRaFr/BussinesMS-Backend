namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class ProductoVarianteDto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string? ProductoNombre { get; set; }
    public string? CodigoBarras { get; set; }
    public int SaborId { get; set; }
    public string? SaborNombre { get; set; }
    public int TamanioId { get; set; }
    public string? TamanioNombre { get; set; }
    public decimal PrecioVentaActual { get; set; }
    public string? CodigoAlmacen { get; set; }
    public string TipoVenta { get; set; } = "u";
    public int? Unidad { get; set; }
    public int? Display { get; set; }
    public int? Caja { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CrearProductoVarianteDto
{
    public int ProductoId { get; set; }
    public string? CodigoBarras { get; set; }
    public int SaborId { get; set; }
    public int TamanioId { get; set; }
    public decimal PrecioVentaActual { get; set; }
    public string? CodigoAlmacen { get; set; }
    public string TipoVenta { get; set; } = "u";
    public int? Unidad { get; set; }
    public int? Display { get; set; }
    public int? Caja { get; set; }
}

public class ActualizarProductoVarianteDto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string? CodigoBarras { get; set; }
    public int SaborId { get; set; }
    public int TamanioId { get; set; }
    public decimal PrecioVentaActual { get; set; }
    public string? CodigoAlmacen { get; set; }
    public string TipoVenta { get; set; } = "u";
    public int? Unidad { get; set; }
    public int? Display { get; set; }
    public int? Caja { get; set; }
    public bool IsActive { get; set; }
}