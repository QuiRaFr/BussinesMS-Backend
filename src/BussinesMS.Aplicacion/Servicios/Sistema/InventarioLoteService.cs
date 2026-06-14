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

public class InventarioLoteService : IInventarioLoteService
{
    private readonly IInventarioLoteRepository _repo;
    private readonly IMovimientoInventarioRepository _movimientoRepo;
    private readonly ISistemaUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<InventarioLoteService> _logger;

    public InventarioLoteService(
        IInventarioLoteRepository repo,
        IMovimientoInventarioRepository movimientoRepo,
        ISistemaUnitOfWork uow,
        IMapper mapper,
        ILogger<InventarioLoteService> logger)
    {
        _repo = repo;
        _movimientoRepo = movimientoRepo;
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<InventarioLoteDto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
    {
        try
        {
            var baseQuery = _repo.AsQueryable().Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var f = query.Filter.ToLower();
                baseQuery = baseQuery.Where(x =>
                    x.VarianteId.ToString().Contains(f));
            }

            (var filteredQuery, var totalCount) = baseQuery.ApplyFilters(query);
            var entidades = await filteredQuery
                .Include(x => x.Variante)
                .ToListAsync();

            return new PagedResultDto<InventarioLoteDto>
            {
                Items = _mapper.Map<List<InventarioLoteDto>>(entidades),
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener lotes de inventario");
            throw;
        }
    }

    public async Task<InventarioLoteDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var entidad = await _repo.ObtenerConDetallesAsync(id);
            if (entidad == null || !entidad.IsActive) return null;

            return _mapper.Map<InventarioLoteDto>(entidad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener lote {Id}", id);
            throw;
        }
    }

    public async Task<InventarioLoteDto> CrearAsync(CrearInventarioLoteDto dto)
    {
        try
        {
            if (dto.StockInicial <= 0)
                throw new ValidacionException("El stock inicial debe ser mayor a 0");

            await _uow.BeginTransactionAsync();
            try
            {
                var entidad = _mapper.Map<InventarioLote>(dto);
                entidad.StockDisponible = dto.StockInicial;
                entidad.EstadoLote = EstadoLote.Activo;
                entidad.CantidadVencida = 0;

                var creada = await _repo.CrearAsync(entidad);

                var movimiento = new MovimientoInventario
                {
                    LoteId = creada.Id,
                    VarianteId = creada.VarianteId,
                    AlmacenDestinoId = creada.AlmacenId,
                    TipoMovimiento = TipoMovimiento.EntradaCompra,
                    CantidadUnidades = creada.StockInicial,
                    SaldoResultante = creada.StockDisponible,
                    ReferenciaId = creada.CompraDetalleId,
                    Observacion = "Creación de lote"
                };
                await _movimientoRepo.CrearAsync(movimiento);

                await _uow.CommitAsync();

                _logger.LogInformation("Lote de inventario creado: {Id} - Variante: {VarianteId}", creada.Id, creada.VarianteId);

                return _mapper.Map<InventarioLoteDto>(creada);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear lote de inventario");
            throw;
        }
    }

    public async Task<InventarioLoteDto> ActualizarAsync(ActualizarInventarioLoteDto dto)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(dto.Id);
            ValidacionEntidad.VerificarActivo(existente, "InventarioLote");

            existente!.VarianteId = dto.VarianteId;
            existente.AlmacenId = dto.AlmacenId;
            existente.CompraDetalleId = dto.CompraDetalleId;
            existente.StockInicial = dto.StockInicial;
            existente.StockDisponible = dto.StockDisponible;
            existente.CantidadVencida = dto.CantidadVencida;
            existente.CostoCompraUnitario = dto.CostoCompraUnitario;
            existente.PrecioVentaUnitario = dto.PrecioVentaUnitario;
            existente.PrecioVentaMayoreo = dto.PrecioVentaMayoreo;
            existente.FechaVencimiento = dto.FechaVencimiento;
            existente.EstadoLote = (EstadoLote)dto.EstadoLote;
            existente.IsActive = dto.IsActive;

            var actualizada = await _repo.ActualizarAsync(existente);

            _logger.LogInformation("Lote de inventario actualizado: {Id}", actualizada.Id);

            return _mapper.Map<InventarioLoteDto>(actualizada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar lote {Id}", dto.Id);
            throw;
        }
    }

    public async Task EliminarAsync(int id)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(id);
            ValidacionEntidad.VerificarActivo(existente, "InventarioLote");

            await _repo.EliminarAsync(id);
            _logger.LogInformation("Lote de inventario eliminado: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar lote {Id}", id);
            throw;
        }
    }

    public async Task<List<InventarioLoteDto>> ObtenerLotesFEFOAsync(int varianteId, int almacenId)
    {
        try
        {
            var lotes = await _repo.ObtenerLotesFEFOAsync(varianteId, almacenId);
            return _mapper.Map<List<InventarioLoteDto>>(lotes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener lotes FEFO para variante {VarianteId} en almacén {AlmacenId}", varianteId, almacenId);
            throw;
        }
    }

    public async Task<int> ObtenerStockDisponibleAsync(int varianteId, int almacenId)
    {
        try
        {
            return await _repo.ObtenerStockDisponibleAsync(varianteId, almacenId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener stock disponible para variante {VarianteId} en almacén {AlmacenId}", varianteId, almacenId);
            throw;
        }
    }

    public async Task<InventarioLoteDto> AjustarStockAsync(int id, AjusteInventarioDto dto)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(id);
            ValidacionEntidad.VerificarActivo(existente, "InventarioLote");

            await _uow.BeginTransactionAsync();
            try
            {
                var stockAnterior = existente!.StockDisponible;
                existente.StockDisponible += dto.CantidadAjuste;

                if (existente.StockDisponible < 0)
                    throw new ValidacionException("El ajuste resultaría en stock negativo");

                var suma = existente.StockDisponible + existente.CantidadVendida
                         + existente.CantidadTrasladada + existente.CantidadVencida;
                if (suma > existente.StockInicial)
                    throw new ValidacionException("La suma de stock, vendido, trasladado y vencido supera el stock inicial");

                if (existente.StockDisponible == 0)
                    existente.EstadoLote = EstadoLote.Agotado;
                else if (existente.StockDisponible > 0 && existente.EstadoLote == EstadoLote.Agotado)
                    existente.EstadoLote = EstadoLote.Activo;

                await _repo.ActualizarAsync(existente);

                var tipoMovimiento = dto.CantidadAjuste > 0
                    ? TipoMovimiento.AjustePositivo
                    : TipoMovimiento.AjusteNegativo;

                var movimiento = new MovimientoInventario
                {
                    LoteId = existente.Id,
                    VarianteId = existente.VarianteId,
                    AlmacenOrigenId = existente.AlmacenId,
                    TipoMovimiento = tipoMovimiento,
                    CantidadUnidades = Math.Abs(dto.CantidadAjuste),
                    SaldoResultante = existente.StockDisponible,
                    Observacion = dto.Observacion ?? $"Ajuste de stock: {stockAnterior} → {existente.StockDisponible}"
                };
                await _movimientoRepo.CrearAsync(movimiento);

                await _uow.CommitAsync();

                _logger.LogInformation("Ajuste de stock en lote {Id}: {Anterior} → {Nuevo}", id, stockAnterior, existente.StockDisponible);

                return _mapper.Map<InventarioLoteDto>(existente);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al ajustar stock en lote {Id}", id);
            throw;
        }
    }
}
