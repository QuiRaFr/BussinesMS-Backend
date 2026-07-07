using BussinesMS.Dominio.Entidades.Compartido;
using BussinesMS.Dominio.Enums;

namespace BussinesMS.Dominio.Entidades.Sistema;

public class DevolucionCliente : EntidadBase
{
    public int LoteAlmacenOrigenId { get; set; }
    public int? LoteAlmacenDevueltoId { get; set; }
    public int VarianteId { get; set; }
    public int AlmacenId { get; set; }
    public int CantidadUnidades { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public EstadoDevolucion EstadoDevolucion { get; set; } = EstadoDevolucion.PendienteCambio;
    public string? Observacion { get; set; }
    public DateTime FechaDevolucion { get; set; } = DateTime.UtcNow;
    public int UsuarioId { get; set; }

    public InventarioLoteAlmacen? LoteAlmacenOrigen { get; set; }
    public InventarioLoteAlmacen? LoteAlmacenDevuelto { get; set; }
    public ProductoVariante? Variante { get; set; }
}
