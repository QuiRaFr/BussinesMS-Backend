namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class TrasladoDto
{
    public int Id { get; set; }
    public int TipoTraslado { get; set; }
    public int? VarianteId { get; set; }
    public string? VarianteNombre { get; set; }
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
    public List<TrasladoDetalleDto> Detalles { get; set; } = [];
}

public class TrasladoDetalleDto
{
    public int Id { get; set; }
    public int TrasladoId { get; set; }
    public int LoteAlmacenOrigenId { get; set; }
    public int? LoteAlmacenDestinoId { get; set; }
    public int CantidadUnidades { get; set; }
    public decimal CostoUnitarioCapturado { get; set; }
}

public class CrearTrasladoPorLoteDto
{
    public int LoteAlmacenOrigenId { get; set; }
    public int AlmacenDestinoId { get; set; }
    public int CantidadUnidades { get; set; }
    public string? Observacion { get; set; }
}

public class CrearTrasladoPorVarianteDto
{
    public int VarianteId { get; set; }
    public int AlmacenOrigenId { get; set; }
    public int AlmacenDestinoId { get; set; }
    public int CantidadUnidades { get; set; }
    public string? Observacion { get; set; }
}
