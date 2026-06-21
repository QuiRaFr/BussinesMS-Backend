using BussinesMS.Dominio.Entidades.Compartido;
using BussinesMS.Dominio.Enums;

namespace BussinesMS.Dominio.Entidades.Sistema;

public class Compra : EntidadBase
{
    public int ProveedorId { get; set; }
    public int UsuarioId { get; set; }
    public int AlmacenId { get; set; }
    public DateTime FechaCompra { get; set; } = DateTime.UtcNow;
    public decimal TotalCompra { get; set; }
    public EstadoPago EstadoPago { get; set; } = EstadoPago.Contado;
    public string? Observacion { get; set; }

    public Proveedor? Proveedor { get; set; }
    public List<CompraDetalle> Detalles { get; set; } = new();
    public List<PagoCompra> Pagos { get; set; } = new();
}
