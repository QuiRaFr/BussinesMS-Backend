namespace BussinesMS.Aplicacion.DTOs.Sistema;

// Lo que devuelve el GET — incluye el nombre mostrar y equivalencia calculada
public class ProductoPresentacionDto
{
    public int Id { get; set; }
    public int VarianteId { get; set; }
    public int TipoPresentacionId { get; set; }
    public string TipoNombre { get; set; } = string.Empty;       // nombre del tipo
    public string NombrePersonalizado { get; set; } = string.Empty;
    public string NombreMostrar { get; set; } = string.Empty;    // NombrePersonalizado ?? TipoNombre
    public int CantidadDePadre { get; set; }
    public int? PresentacionPadreId { get; set; }
    public int EquivalenciaEnUnidades { get; set; }              // calculado: producto de toda la cadena
    public bool EsDefaultReporte { get; set; }
    public string? CodigoBarras { get; set; }
    public int Orden { get; set; }                               // del TipoPresentacion para ordenar en UI
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CrearProductoPresentacionDto
{
    public int VarianteId { get; set; }
    public int TipoPresentacionId { get; set; }
    public string? NombrePersonalizado { get; set; }
    public int CantidadDePadre { get; set; }
    public int? PresentacionPadreId { get; set; }
    public bool EsDefaultReporte { get; set; }
    public string? CodigoBarras { get; set; }
}

public class ActualizarProductoPresentacionDto
{
    public int Id { get; set; }
    public int TipoPresentacionId { get; set; }
    public string? NombrePersonalizado { get; set; }
    public int CantidadDePadre { get; set; }
    public int? PresentacionPadreId { get; set; }
    public bool EsDefaultReporte { get; set; }
    public string? CodigoBarras { get; set; }
    public bool IsActive { get; set; }
}

// Para el reporte/inventario — resultado de conversión
public class StockPresentacionDto
{
    public string NombreMostrar { get; set; } = string.Empty;
    public int Cantidad { get; set; }                            // puede ser 0, no se omite
    public int EquivalenciaEnUnidades { get; set; }
    public bool EsDefault { get; set; }
}

public class StockConversionDto
{
    public int VarianteId { get; set; }
    public int StockEnUnidades { get; set; }
    public List<StockPresentacionDto> Desglose { get; set; } = [];
    // Ej: [ {Display, 36}, {Unidad, 3} ]
}