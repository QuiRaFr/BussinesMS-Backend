using BussinesMS.Dominio.Enums;

namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class CompraDto
{
    public int Id { get; set; }
    public int ProveedorId { get; set; }
    public string? ProveedorNombre { get; set; }
    public int UsuarioId { get; set; }
    public int AlmacenId { get; set; }
    public DateTime FechaCompra { get; set; }
    public decimal TotalCompra { get; set; }
    public EstadoPago EstadoPago { get; set; }
    public string? NumeroFactura { get; set; }
    public bool EstaLiquidada { get; set; }
    public string? Observacion { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CompraDetalleDto> Detalles { get; set; } = new();
    public List<PagoCompraDto> Pagos { get; set; } = new();
}

public class CrearCompraDto
{
    public int ProveedorId { get; set; }
    public int AlmacenId { get; set; }
    public EstadoPago EstadoPago { get; set; } = EstadoPago.Contado;
    public string? NumeroFactura { get; set; }
    public decimal? MontoParcial { get; set; }
    public int PagadoPorUsuarioId { get; set; }
    public string? Observacion { get; set; }
    public List<CrearCompraDetalleDto> Detalles { get; set; } = new();
}

public class ActualizarCompraDto
{
    public int Id { get; set; }
    public int ProveedorId { get; set; }
    public int AlmacenId { get; set; }
    public EstadoPago EstadoPago { get; set; }
    public string? NumeroFactura { get; set; }
    public string? Observacion { get; set; }
    public bool IsActive { get; set; }
}
