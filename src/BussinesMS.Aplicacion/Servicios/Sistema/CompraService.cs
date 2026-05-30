using AutoMapper;
using BussinesMS.Aplicacion.Comun;
using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Helpers;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using BussinesMS.Dominio.Entidades.Sistema;
using BussinesMS.Dominio.Enums;
using BussinesMS.Dominio.Excepciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BussinesMS.Aplicacion.Servicios.Sistema;

public class CompraService : ICompraService
{
    private readonly ICompraRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<CompraService> _logger;

    public CompraService(
        ICompraRepository repo,
        IMapper mapper,
        ILogger<CompraService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<CompraDto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
    {
        try
        {
            var baseQuery = _repo.AsQueryable().Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var f = query.Filter.ToLower();
                baseQuery = baseQuery.Where(x =>
                    x.Observacion != null && x.Observacion.ToLower().Contains(f));
            }

            (var filteredQuery, var totalCount) = baseQuery.ApplyFilters(query);
            var entidades = await filteredQuery
                .Include(x => x.Proveedor)
                .ToListAsync();

            return new PagedResultDto<CompraDto>
            {
                Items = _mapper.Map<List<CompraDto>>(entidades),
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener compras");
            throw;
        }
    }

    public async Task<CompraDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var entidad = await _repo.ObtenerConDetallesAsync(id);
            if (entidad == null || !entidad.IsActive) return null;

            return _mapper.Map<CompraDto>(entidad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener compra {Id}", id);
            throw;
        }
    }

    public async Task<CompraDto> CrearAsync(CrearCompraDto dto)
    {
        try
        {
            if (dto.Detalles == null || !dto.Detalles.Any())
                throw new ValidacionException("La compra debe tener al menos un detalle");

            var entidad = _mapper.Map<Compra>(dto);
            entidad.FechaCompra = DateTime.UtcNow;
            entidad.TotalCompra = dto.Detalles.Sum(d => d.CantidadUnidades * d.CostoUnitario);

            // Crear detalles con subtotal calculado
            foreach (var detalleDto in dto.Detalles)
            {
                var detalle = _mapper.Map<CompraDetalle>(detalleDto);
                detalle.Subtotal = detalleDto.CantidadUnidades * detalleDto.CostoUnitario;
                entidad.Detalles.Add(detalle);
            }

            var creada = await _repo.CrearAsync(entidad);

            _logger.LogInformation("Compra creada: {Id} - Total: {Total}", creada.Id, creada.TotalCompra);

            return _mapper.Map<CompraDto>(creada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear compra");
            throw;
        }
    }

    public async Task<CompraDto> ActualizarAsync(ActualizarCompraDto dto)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(dto.Id);
            ValidacionEntidad.VerificarActivo(existente, "Compra");

            existente!.ProveedorId = dto.ProveedorId;
            existente.AlmacenId = dto.AlmacenId;
            existente.EstadoPago = dto.EstadoPago;
            existente.Observacion = dto.Observacion;
            existente.IsActive = dto.IsActive;

            var actualizada = await _repo.ActualizarAsync(existente);

            _logger.LogInformation("Compra actualizada: {Id}", actualizada.Id);

            return _mapper.Map<CompraDto>(actualizada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar compra {Id}", dto.Id);
            throw;
        }
    }

    public async Task EliminarAsync(int id)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(id);
            ValidacionEntidad.VerificarActivo(existente, "Compra");

            await _repo.EliminarAsync(id);
            _logger.LogInformation("Compra eliminada: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar compra {Id}", id);
            throw;
        }
    }

    public async Task<PagoCompraDto> AgregarPagoAsync(int compraId, CrearPagoCompraDto dto)
    {
        try
        {
            var compra = await _repo.ObtenerConDetallesAsync(compraId);
            ValidacionEntidad.VerificarActivo(compra, "Compra");

            if (compra!.EstadoPago == EstadoPago.Pagado)
                throw new ValidacionException("La compra ya está pagada completamente");

            var pago = new PagoCompra
            {
                CompraId = compraId,
                Monto = dto.Monto,
                FechaPago = DateTime.UtcNow,
                SesionCajaId = dto.SesionCajaId,
                Observacion = dto.Observacion
            };

            compra.Pagos.Add(pago);

            // Recalcular total pagado
            var totalPagado = compra.Pagos.Sum(p => p.Monto);
            var totalCompra = compra.TotalCompra;

            if (totalPagado >= totalCompra)
                compra.EstadoPago = EstadoPago.Pagado;
            else
                compra.EstadoPago = EstadoPago.ParcialmentePagado;

            await _repo.ActualizarAsync(compra);

            _logger.LogInformation("Pago registrado para compra {CompraId}: {Monto}", compraId, dto.Monto);

            return _mapper.Map<PagoCompraDto>(pago);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar pago a compra {CompraId}", compraId);
            throw;
        }
    }
}
