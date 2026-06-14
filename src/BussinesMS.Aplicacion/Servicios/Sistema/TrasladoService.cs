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

public class TrasladoService : ITrasladoService
{
    private readonly ITrasladoRepository _repo;
    private readonly IInventarioLoteRepository _loteRepo;
    private readonly IMovimientoInventarioRepository _movimientoRepo;
    private readonly ISistemaUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<TrasladoService> _logger;

    public TrasladoService(
        ITrasladoRepository repo,
        IInventarioLoteRepository loteRepo,
        IMovimientoInventarioRepository movimientoRepo,
        ISistemaUnitOfWork uow,
        IMapper mapper,
        ILogger<TrasladoService> logger)
    {
        _repo = repo;
        _loteRepo = loteRepo;
        _movimientoRepo = movimientoRepo;
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<TrasladoDto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
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
            var entidades = await filteredQuery.ToListAsync();

            return new PagedResultDto<TrasladoDto>
            {
                Items = _mapper.Map<List<TrasladoDto>>(entidades),
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener traslados");
            throw;
        }
    }

    public async Task<TrasladoDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var entidad = await _repo.ObtenerConDetallesAsync(id);
            if (entidad == null || !entidad.IsActive) return null;

            return _mapper.Map<TrasladoDto>(entidad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener traslado {Id}", id);
            throw;
        }
    }

    public async Task<TrasladoDto> CrearAsync(CrearTrasladoDto dto)
    {
        try
        {
            var loteOrigen = await _loteRepo.ObtenerPorIdAsync(dto.LoteId);
            ValidacionEntidad.VerificarActivo(loteOrigen, "Lote origen");

            if (dto.CantidadUnidades <= 0)
                throw new ValidacionException("La cantidad de unidades debe ser mayor a 0");

            if (dto.CantidadUnidades > loteOrigen!.StockDisponible)
                throw new ValidacionException("La cantidad a trasladar excede el stock disponible del lote origen");

            if (loteOrigen.AlmacenId == dto.AlmacenDestinoId)
                throw new ValidacionException("El almacén de origen y destino no pueden ser el mismo");

            var almacenOrigenId = loteOrigen.AlmacenId;

            await _uow.BeginTransactionAsync();
            try
            {
                var esTotal = dto.CantidadUnidades == loteOrigen.StockDisponible;

                var entidad = new Traslado
                {
                    LoteId = dto.LoteId,
                    AlmacenOrigenId = almacenOrigenId,
                    AlmacenDestinoId = dto.AlmacenDestinoId,
                    CantidadUnidades = dto.CantidadUnidades,
                    Observacion = dto.Observacion,
                    FechaTraslado = DateTime.UtcNow
                };

                var creado = await _repo.CrearAsync(entidad);

                if (esTotal)
                {
                    loteOrigen.AlmacenId = dto.AlmacenDestinoId;
                    await _loteRepo.ActualizarAsync(loteOrigen);

                    var movimientoOrigen = new MovimientoInventario
                    {
                        LoteId = loteOrigen.Id,
                        VarianteId = loteOrigen.VarianteId,
                        AlmacenOrigenId = almacenOrigenId,
                        AlmacenDestinoId = dto.AlmacenDestinoId,
                        TipoMovimiento = TipoMovimiento.Traslado,
                        CantidadUnidades = dto.CantidadUnidades,
                        SaldoResultante = loteOrigen.StockDisponible,
                        ReferenciaId = creado.Id,
                        Observacion = $"Traslado total a almacén {dto.AlmacenDestinoId}"
                    };
                    await _movimientoRepo.CrearAsync(movimientoOrigen);

                    creado.LoteDestinoId = loteOrigen.Id;
                    await _repo.ActualizarAsync(creado);
                }
                else
                {
                    loteOrigen.StockDisponible -= dto.CantidadUnidades;
                    await _loteRepo.ActualizarAsync(loteOrigen);

                    var nuevoLote = new InventarioLote
                    {
                        VarianteId = loteOrigen.VarianteId,
                        AlmacenId = dto.AlmacenDestinoId,
                        CompraDetalleId = loteOrigen.CompraDetalleId,
                        StockInicial = dto.CantidadUnidades,
                        StockDisponible = dto.CantidadUnidades,
                        CantidadVencida = 0,
                        CostoCompraUnitario = loteOrigen.CostoCompraUnitario,
                        PrecioVentaUnitario = loteOrigen.PrecioVentaUnitario,
                        PrecioVentaMayoreo = loteOrigen.PrecioVentaMayoreo,
                        FechaVencimiento = loteOrigen.FechaVencimiento,
                        EstadoLote = EstadoLote.Activo
                    };
                    var loteCreado = await _loteRepo.CrearAsync(nuevoLote);

                    var movimientoSalida = new MovimientoInventario
                    {
                        LoteId = loteOrigen.Id,
                        VarianteId = loteOrigen.VarianteId,
                        AlmacenOrigenId = almacenOrigenId,
                        AlmacenDestinoId = dto.AlmacenDestinoId,
                        TipoMovimiento = TipoMovimiento.Traslado,
                        CantidadUnidades = dto.CantidadUnidades,
                        SaldoResultante = loteOrigen.StockDisponible,
                        ReferenciaId = creado.Id,
                        Observacion = $"Traslado parcial - salida de almacén {almacenOrigenId}"
                    };
                    await _movimientoRepo.CrearAsync(movimientoSalida);

                    var movimientoEntrada = new MovimientoInventario
                    {
                        LoteId = loteCreado.Id,
                        VarianteId = loteOrigen.VarianteId,
                        AlmacenDestinoId = dto.AlmacenDestinoId,
                        TipoMovimiento = TipoMovimiento.Traslado,
                        CantidadUnidades = dto.CantidadUnidades,
                        SaldoResultante = loteCreado.StockDisponible,
                        ReferenciaId = creado.Id,
                        Observacion = $"Traslado parcial - entrada a almacén {dto.AlmacenDestinoId}"
                    };
                    await _movimientoRepo.CrearAsync(movimientoEntrada);

                    creado.LoteDestinoId = loteCreado.Id;
                    await _repo.ActualizarAsync(creado);
                }

                await _uow.CommitAsync();

                _logger.LogInformation("Traslado creado: {Id} - Lote: {LoteId} - Cantidad: {Cantidad}",
                    creado.Id, dto.LoteId, dto.CantidadUnidades);

                return _mapper.Map<TrasladoDto>(creado);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear traslado");
            throw;
        }
    }
}
