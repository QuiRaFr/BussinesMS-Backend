namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class ProductoVarianteDto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string? NombreProducto { get; set; }
    public string? CodigoBarras { get; set; }
    public int SaborId { get; set; }
    public string? SaborDescripcion { get; set; }
    public int TamanioId { get; set; }
    public string? PesoTamanio { get; set; }
    public decimal PrecioVentaActual { get; set; }
    public decimal PrecioCompra { get; set; }
    public string? CodigoAlmacen { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CategoriaId { get; set; }
    public string? CategoriaNombre { get; set; }

    // Presentacion 1 (Unidad — siempre existe)
    public int? PresentacionId1 { get; set; }
    public string? TipoNombre1 { get; set; }
    public string? NombrePersonalizado1 { get; set; }
    public int EquivalenciaEnUnidades1 { get; set; }
    public bool EsDefaultReporte1 { get; set; }

    // Presentacion 2 (Caja — opcional)
    public int? PresentacionId2 { get; set; }
    public string? TipoNombre2 { get; set; }
    public string? NombrePersonalizado2 { get; set; }
    public int? EquivalenciaEnUnidades2 { get; set; }
    public bool? EsDefaultReporte2 { get; set; }

    // Presentacion 3 (Caja2 — opcional)
    public int? PresentacionId3 { get; set; }
    public string? TipoNombre3 { get; set; }
    public string? NombrePersonalizado3 { get; set; }
    public int? EquivalenciaEnUnidades3 { get; set; }
    public bool? EsDefaultReporte3 { get; set; }

    // Array completo solo para ObtenerPorId — formulario de edición
    public List<ProductoPresentacionDto> Presentaciones { get; set; } = [];
}
public class CrearProductoVarianteDto
{
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public string? CodigoBarras { get; set; }
    public int SaborId { get; set; }
    public string SaborDescripcion { get; set; } = string.Empty;
    public int TamanioId { get; set; }
    public string PesoTamanio { get; set; } = string.Empty;
    public decimal PrecioVentaActual { get; set; }
    public decimal PrecioCompra { get; set; }
    public string? CodigoAlmacen { get; set; }
    public List<CrearPresentacionEnVarianteDto> Presentaciones { get; set; } = [];
}

public class ActualizarProductoVarianteDto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public string? CodigoBarras { get; set; }
    public int SaborId { get; set; }
    public string SaborDescripcion { get; set; } = string.Empty;
    public int TamanioId { get; set; }
    public string PesoTamanio { get; set; } = string.Empty;
    public decimal PrecioVentaActual { get; set; }
    public decimal PrecioCompra { get; set; }
    public string? CodigoAlmacen { get; set; }
    public bool IsActive { get; set; }
    public List<CrearPresentacionEnVarianteDto> Presentaciones { get; set; } = [];
}

// DTO simplificado para crear presentaciones dentro de la variante
// No necesita VarianteId porque se asigna en el service
public class CrearPresentacionEnVarianteDto
{
    public int TipoPresentacionId { get; set; }
    public string? NombrePersonalizado { get; set; }
    public int CantidadDePadre { get; set; }
    public int? PresentacionPadreIndice { get; set; } // índice en el array, no Id — el Id aún no existe
    public bool EsDefaultReporte { get; set; }
    public string? CodigoBarras { get; set; }
}