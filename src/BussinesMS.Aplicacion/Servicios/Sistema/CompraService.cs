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
    private readonly IInventarioLoteRepository _loteRepo;
    private readonly IMovimientoInventarioRepository _movimientoRepo;
    private readonly ISistemaUnitOfWork _uow;

    public CompraService(
        ICompraRepository repo,
        IMapper mapper,
        ILogger<CompraService> logger,
        IInventarioLoteRepository loteRepo,
        IMovimientoInventarioRepository movimientoRepo,
        ISistemaUnitOfWork uow)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
        _loteRepo = loteRepo;
        _movimientoRepo = movimientoRepo;
        _uow = uow;
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

            foreach (var d in dto.Detalles)
            {
                if (d.PrecioVentaUnitario <= 0)
                    throw new ValidacionException("El precio de venta unitario debe ser mayor a 0");
                if (d.PrecioVentaMayoreo <= 0)
                    throw new ValidacionException("El precio de venta mayoreo debe ser mayor a 0");
            }

            await _uow.BeginTransactionAsync();
            try
            {
                var entidad = _mapper.Map<Compra>(dto);
                entidad.FechaCompra = DateTime.UtcNow;
                entidad.TotalCompra = dto.Detalles.Sum(d => d.CantidadUnidades * d.CostoUnitario);
                entidad.Detalles.Clear();

                var pares = dto.Detalles.Select(detalleDto =>
                {
                    var detalle = _mapper.Map<CompraDetalle>(detalleDto);
                    detalle.Subtotal = detalleDto.CantidadUnidades * detalleDto.CostoUnitario;
                    return (detalle, detalleDto);
                }).ToList();

                foreach (var (detalle, _) in pares)
                    entidad.Detalles.Add(detalle);

                await _repo.CrearSinGuardarAsync(entidad);
                await _uow.SaveChangesAsync();

                var lotes = new List<(InventarioLote lote, CrearCompraDetalleDto detalleDto)>();
                foreach (var (detalle, detalleDto) in pares)
                {
                    var lote = new InventarioLote
                    {
                        VarianteId = detalle.VarianteId,
                        AlmacenId = detalle.AlmacenId,
                        CompraDetalleId = detalle.Id,
                        StockInicial = detalle.CantidadUnidades,
                        StockDisponible = detalle.CantidadUnidades,
                        CantidadVencida = 0,
                        CostoCompraUnitario = detalle.CostoUnitario,
                        PrecioVentaUnitario = detalleDto.PrecioVentaUnitario,
                        PrecioVentaMayoreo = detalleDto.PrecioVentaMayoreo,
                        FechaVencimiento = detalle.FechaVencimiento,
                        EstadoLote = EstadoLote.Activo
                    };
                    await _loteRepo.CrearSinGuardarAsync(lote);
                    lotes.Add((lote, detalleDto));
                }

                await _uow.SaveChangesAsync();

                foreach (var (lote, _) in lotes)
                {
                    var movimiento = new MovimientoInventario
                    {
                        LoteId = lote.Id,
                        VarianteId = lote.VarianteId,
                        AlmacenOrigenId = null,
                        AlmacenDestinoId = lote.AlmacenId,
                        TipoMovimiento = TipoMovimiento.EntradaCompra,
                        CantidadUnidades = lote.StockInicial,
                        SaldoResultante = lote.StockDisponible,
                        ReferenciaId = lote.CompraDetalleId,
                        Observacion = "Entrada por compra"
                    };
                    await _movimientoRepo.CrearSinGuardarAsync(movimiento);
                }

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                _logger.LogInformation("Compra creada: {Id} - Total: {Total} - Lotes: {Cantidad}",
                    entidad.Id, entidad.TotalCompra, pares.Count);

                return _mapper.Map<CompraDto>(entidad);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
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

            if (compra!.EstadoPago == EstadoPago.Contado)
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
                compra.EstadoPago = EstadoPago.Contado;
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
