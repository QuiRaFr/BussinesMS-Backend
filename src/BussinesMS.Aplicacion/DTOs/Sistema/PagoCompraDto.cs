namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class PagoCompraDto
{
    public int Id { get; set; }
    public int CompraId { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; }
    public int? SesionCajaId { get; set; }
    public int PagadoPorUsuarioId { get; set; }
    public string? Observacion { get; set; }
}

public class CrearPagoCompraDto
{
    public decimal Monto { get; set; }
    public int PagadoPorUsuarioId { get; set; }
    public int? SesionCajaId { get; set; }
    public string? Observacion { get; set; }
}
