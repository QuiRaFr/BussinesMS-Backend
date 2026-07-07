namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class DevolucionClienteDto
{
    public int Id { get; set; }
    public int LoteAlmacenOrigenId { get; set; }
    public int? LoteAlmacenDevueltoId { get; set; }
    public int VarianteId { get; set; }
    public string? VarianteNombre { get; set; }
    public int AlmacenId { get; set; }
    public int CantidadUnidades { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public int EstadoDevolucion { get; set; }
    public string? Observacion { get; set; }
    public DateTime FechaDevolucion { get; set; }
    public int UsuarioId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CrearDevolucionClienteDto
{
    public int VarianteId { get; set; }
    public int AlmacenId { get; set; }
    public int LoteAlmacenOrigenId { get; set; }
    public int CantidadUnidades { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? Observacion { get; set; }
}

public class RealizarCambioDto
{
    public int DevolucionId { get; set; }
    public int LoteDestinoId { get; set; }
}

public class DarDeBajaDevolucionDto
{
    public int DevolucionId { get; set; }
    public string? Observacion { get; set; }
}
