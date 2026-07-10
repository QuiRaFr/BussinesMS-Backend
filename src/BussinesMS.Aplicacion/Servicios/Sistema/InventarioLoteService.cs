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
    private readonly IInventarioLoteRepository _loteRepo;
    private readonly IInventarioLoteAlmacenRepository _loteAlmacenRepo;
    private readonly IMovimientoInventarioRepository _movimientoRepo;
    private readonly ISistemaUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<InventarioLoteService> _logger;

    public InventarioLoteService(
        IInventarioLoteRepository loteRepo,
        IInventarioLoteAlmacenRepository loteAlmacenRepo,
        IMovimientoInventarioRepository movimientoRepo,
        ISistemaUnitOfWork uow,
        IMapper mapper,
        ILogger<InventarioLoteService> logger)
    {
        _loteRepo = loteRepo;
        _loteAlmacenRepo = loteAlmacenRepo;
        _movimientoRepo = movimientoRepo;
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<InventarioLoteAlmacenDto>> ObtenerTodosAsync(
        GenericPaginationQueryDto query, int? categoriaId = null, int? almacenId = null)
    {
        try
        {
            var baseQuery = _loteAlmacenRepo.AsQueryable().Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var f = query.Filter.ToLower();
                baseQuery = baseQuery.Where(x =>
                    x.Lote != null && x.Lote.Variante != null &&
                    ((x.Lote.Variante.NombreProducto != null &&
                      x.Lote.Variante.NombreProducto.ToLower().Contains(f)) ||
                     (x.Lote.Variante.CodigoBarras != null &&
                      x.Lote.Variante.CodigoBarras.ToLower().Contains(f))));
            }

            if (categoriaId.HasValue)
            {
                baseQuery = baseQuery.Where(x =>
                    x.Lote != null && x.Lote.Variante != null &&
                    x.Lote.Variante.Producto != null &&
                    x.Lote.Variante.Producto.CategoriaId == categoriaId.Value);
            }

            if (almacenId.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.AlmacenId == almacenId.Value);
            }

            (var filteredQuery, var totalCount) = baseQuery.ApplyFilters(query);
            var entidades = await filteredQuery
                .Include(x => x.Lote)
                    .ThenInclude(l => l!.Variante)
                        .ThenInclude(v => v!.Producto)
                            .ThenInclude(p => p!.Categoria)
                .ToListAsync();

            var dtos = entidades.Select(MapToDto).ToList();

            return new PagedResultDto<InventarioLoteAlmacenDto>
            {
                Items = dtos,
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

    public async Task<InventarioLoteAlmacenDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var entidad = await _loteAlmacenRepo.ObtenerConDetallesAsync(id);
            if (entidad == null || !entidad.IsActive) return null;

            return MapToDto(entidad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener lote de inventario {Id}", id);
            throw;
        }
    }

    public async Task<InventarioLoteAlmacenDto> CrearAsync(CrearInventarioLoteAlmacenDto dto)
    {
        try
        {
            if (dto.StockInicial <= 0)
                throw new ValidacionException("El stock inicial debe ser mayor a 0");

            await _uow.BeginTransactionAsync();
            try
            {
                var lote = new InventarioLote
                {
                    VarianteId = dto.VarianteId,
                    CompraDetalleId = dto.CompraDetalleId,
                    CostoCompraUnitario = dto.CostoCompraUnitario,
                    FechaVencimiento = dto.FechaVencimiento
                };
                var loteCreado = await _loteRepo.CrearSinGuardarAsync(lote);

                var loteAlmacen = new InventarioLoteAlmacen
                {
                    LoteId = loteCreado.Id,
                    AlmacenId = dto.AlmacenId,
                    StockInicial = dto.StockInicial,
                    StockDisponible = dto.StockInicial,
                    CantidadVendida = 0,
                    CantidadTrasladada = 0,
                    CantidadVencida = 0,
                    EstadoLote = EstadoLote.Activo
                };
                var creado = await _loteAlmacenRepo.CrearSinGuardarAsync(loteAlmacen);

                var movimiento = new MovimientoInventario
                {
                    LoteAlmacenId = creado.Id,
                    VarianteId = dto.VarianteId,
                    AlmacenOrigenId = dto.AlmacenId,
                    TipoMovimiento = TipoMovimiento.EntradaCompra,
                    CantidadUnidades = dto.StockInicial,
                    SaldoResultante = dto.StockInicial,
                    ReferenciaId = dto.CompraDetalleId,
                    Observacion = "Creación de lote"
                };
                await _movimientoRepo.CrearSinGuardarAsync(movimiento);

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                var resultado = await _loteAlmacenRepo.ObtenerConDetallesAsync(creado.Id);

                _logger.LogInformation(
                    "Lote de inventario creado: LoteAlmacenId={Id} - Variante: {VarianteId}",
                    creado.Id, dto.VarianteId);

                return MapToDto(resultado!);
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

    public async Task<InventarioLoteAlmacenDto> ActualizarAsync(ActualizarInventarioLoteAlmacenDto dto)
    {
        try
        {
            var existente = await _loteAlmacenRepo.ObtenerPorIdAsync(dto.Id);
            ValidacionEntidad.VerificarActivo(existente, "InventarioLoteAlmacen");

            existente!.StockDisponible = dto.StockDisponible;
            existente.EstadoLote = (EstadoLote)dto.EstadoLote;
            existente.IsActive = dto.IsActive;

            var actualizada = await _loteAlmacenRepo.ActualizarAsync(existente);

            _logger.LogInformation("Lote de inventario actualizado: {Id}", actualizada.Id);

            var resultado = await _loteAlmacenRepo.ObtenerConDetallesAsync(actualizada.Id);
            return MapToDto(resultado!);
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
            var existente = await _loteAlmacenRepo.ObtenerPorIdAsync(id);
            ValidacionEntidad.VerificarActivo(existente, "InventarioLoteAlmacen");

            await _loteAlmacenRepo.EliminarAsync(id);
            _logger.LogInformation("Lote de inventario eliminado: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar lote {Id}", id);
            throw;
        }
    }

    public async Task<InventarioLoteAlmacenDto> AjustarStockAsync(int id, AjusteInventarioDto dto)
    {
        try
        {
            var existente = await _loteAlmacenRepo.ObtenerPorIdAsync(id);
            ValidacionEntidad.VerificarActivo(existente, "InventarioLoteAlmacen");

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
                    throw new ValidacionException(
                        "La suma de stock, vendido, trasladado y vencido supera el stock inicial");

                if (existente.StockDisponible == 0)
                    existente.EstadoLote = EstadoLote.Agotado;
                else if (existente.StockDisponible > 0 &&
                         existente.EstadoLote == EstadoLote.Agotado)
                    existente.EstadoLote = EstadoLote.Activo;

                await _loteAlmacenRepo.ActualizarAsync(existente);

                var tipoMovimiento = dto.CantidadAjuste > 0
                    ? TipoMovimiento.AjustePositivo
                    : TipoMovimiento.AjusteNegativo;

                var movimiento = new MovimientoInventario
                {
                    LoteAlmacenId = existente.Id,
                    VarianteId = existente.Lote?.VarianteId ?? 0,
                    AlmacenOrigenId = existente.AlmacenId,
                    TipoMovimiento = tipoMovimiento,
                    CantidadUnidades = Math.Abs(dto.CantidadAjuste),
                    SaldoResultante = existente.StockDisponible,
                    Observacion = dto.Observacion ??
                        $"Ajuste de stock: {stockAnterior} → {existente.StockDisponible}"
                };
                await _movimientoRepo.CrearSinGuardarAsync(movimiento);

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                var resultado = await _loteAlmacenRepo.ObtenerConDetallesAsync(existente.Id);

                _logger.LogInformation(
                    "Ajuste de stock en lote {Id}: {Anterior} → {Nuevo}",
                    id, stockAnterior, existente.StockDisponible);

                return MapToDto(resultado!);
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

    private static InventarioLoteAlmacenDto MapToDto(InventarioLoteAlmacen entidad)
    {
        var lote = entidad.Lote;
        var variante = lote?.Variante;
        var producto = variante?.Producto;
        var categoria = producto?.Categoria;

        int? diasParaVencer = null;
        if (lote?.FechaVencimiento.HasValue == true)
        {
            diasParaVencer = (int)(lote.FechaVencimiento.Value - DateTime.UtcNow).TotalDays;
        }

        return new InventarioLoteAlmacenDto
        {
            Id = lote?.Id ?? 0,
            AlmacenId = entidad.AlmacenId,
            StockInicial = entidad.StockInicial,
            StockDisponible = entidad.StockDisponible,
            CantidadVendida = entidad.CantidadVendida,
            CantidadTrasladada = entidad.CantidadTrasladada,
            CantidadVencida = entidad.CantidadVencida,
            EstadoLote = (int)entidad.EstadoLote,
            IsActive = entidad.IsActive,
            CreatedAt = entidad.CreatedAt,
            VarianteId = variante?.Id ?? 0,
            VarianteNombre = variante?.DescripcionProducto,
            NombreProducto = producto?.Nombre,
            CodigoBarras = variante?.CodigoBarras,
            CategoriaId = categoria?.Id ?? 0,
            CategoriaNombre = categoria?.Nombre,
            CompraDetalleId = lote?.CompraDetalleId,
            CostoCompraUnitario = lote?.CostoCompraUnitario ?? 0,
            PrecioVentaUnitario = variante?.PrecioVentaUnitario ?? 0,
            PrecioVentaMayoreo = variante?.PrecioVentaMayoreo ?? 0,
            FechaVencimiento = lote?.FechaVencimiento,
            DiasParaVencer = diasParaVencer
        };
    }
}
