using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Dominio.Entidades.Sistema;

public class ProductoVariante : EntidadBase
{
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
    public string? NombreProducto { get; set; }

    public string? CodigoBarras { get; set; }

    public int SaborId { get; set; }
    public DescripcionSabor? Sabor { get; set; }
    public string? SaborDescripcion { get; set; }
    public int CantidadCaja { get; set; }

    public int TamanioId { get; set; }
    public DescripcionTamanio? Tamanio { get; set; }
    public string? PesoTamanio { get; set; }

    public decimal PrecioVentaActual { get; set; }
    public decimal PrecioCompra { get; set; }
    public string? CodigoAlmacen { get; set; }

    // Reemplaza: TipoVenta, Unidad, Display, Caja
    public ICollection<ProductoPresentacion> Presentaciones { get; set; } = [];
}