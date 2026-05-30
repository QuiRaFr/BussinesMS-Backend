namespace BussinesMS.Dominio.Entidades.Sistema;

public class PagoCompra
{
    public int Id { get; set; }
    public int CompraId { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; } = DateTime.UtcNow;
    public int? SesionCajaId { get; set; }
    public string? Observacion { get; set; }
    public int RegistradoByUsuarioId { get; set; }

    public Compra? Compra { get; set; }
}
