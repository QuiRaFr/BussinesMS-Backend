using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Dominio.Entidades.Sistema;

public class ProductoPresentacion : EntidadBase
{
    public int VarianteId { get; set; }
    public ProductoVariante? Variante { get; set; }

    public int TipoPresentacionId { get; set; }
    public TipoPresentacion? TipoPresentacion { get; set; }

    public string? NombrePersonalizado { get; set; }  // "Display","Tira","Botella" — null=usa nombre del tipo
    public int CantidadDePadre { get; set; }          // Unidad siempre=1, Caja=cuántas unidades contiene

    public int? PresentacionPadreId { get; set; }     // null = es Unidad (raíz)
    public ProductoPresentacion? PresentacionPadre { get; set; }
    public ICollection<ProductoPresentacion> PresentacionesHijas { get; set; } = [];

    public bool EsDefaultReporte { get; set; }        // Solo 1 true por VarianteId
    public string? CodigoBarras { get; set; }

    // Nombre que se muestra: NombrePersonalizado ?? TipoPresentacion.Nombre
    public string NombreMostrar => NombrePersonalizado ?? TipoPresentacion?.Nombre ?? string.Empty;
}