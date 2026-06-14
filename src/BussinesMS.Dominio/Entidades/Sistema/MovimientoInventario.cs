using BussinesMS.Dominio.Enums;

namespace BussinesMS.Dominio.Entidades.Sistema;

public class MovimientoInventario
{
    public int Id { get; set; }
    public int LoteId { get; set; }
    public int VarianteId { get; set; }
    public int? AlmacenOrigenId { get; set; }
    public int? AlmacenDestinoId { get; set; }
    public TipoMovimiento TipoMovimiento { get; set; }
    public int CantidadUnidades { get; set; }
    public int SaldoResultante { get; set; }
    public int? ReferenciaId { get; set; }
    public string? Observacion { get; set; }
    public DateTime FechaMovimiento { get; set; } = DateTime.UtcNow;
    public int UsuarioId { get; set; }

    public InventarioLote? Lote { get; set; }
    public ProductoVariante? Variante { get; set; }
}
