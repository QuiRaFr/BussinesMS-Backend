namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class TrasladoDto
{
    public int Id { get; set; }
    public int LoteId { get; set; }
    public int? LoteDestinoId { get; set; }
    public int AlmacenOrigenId { get; set; }
    public string? AlmacenOrigenNombre { get; set; }
    public int AlmacenDestinoId { get; set; }
    public string? AlmacenDestinoNombre { get; set; }
    public int CantidadUnidades { get; set; }
    public string? Observacion { get; set; }
    public DateTime FechaTraslado { get; set; }
    public int UsuarioId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CrearTrasladoDto
{
    public int LoteId { get; set; }
    public int AlmacenDestinoId { get; set; }
    public int CantidadUnidades { get; set; }
    public string? Observacion { get; set; }
}
