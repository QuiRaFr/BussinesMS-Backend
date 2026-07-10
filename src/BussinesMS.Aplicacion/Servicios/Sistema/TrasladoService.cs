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
    private readonly ITrasladoRepository _trasladoRepo;
    private readonly IInventarioLoteAlmacenRepository _loteAlmacenRepo;
    private readonly IInventarioLoteRepository _loteRepo;
    private readonly IMovimientoInventarioRepository _movimientoRepo;
    private readonly ISistemaUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<TrasladoService> _logger;

    public TrasladoService(
        ITrasladoRepository trasladoRepo,
        IInventarioLoteAlmacenRepository loteAlmacenRepo,
        IInventarioLoteRepository loteRepo,
        IMovimientoInventarioRepository movimientoRepo,
        ISistemaUnitOfWork uow,
        IMapper mapper,
        ILogger<TrasladoService> logger)
    {
        _trasladoRepo = trasladoRepo;
        _loteAlmacenRepo = loteAlmacenRepo;
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
            var baseQuery = _trasladoRepo.AsQueryable().Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var f = query.Filter.ToLower();
                baseQuery = baseQuery.Where(x =>
                    x.Observacion != null && x.Observacion.ToLower().Contains(f));
            }

            (var filteredQuery, var totalCount) = baseQuery.ApplyFilters(query);
            var entidades = await filteredQuery
                .Include(x => x.Detalles)
                .ToListAsync();

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
            var entidad = await _trasladoRepo.ObtenerConDetallesAsync(id);
            if (entidad == null || !entidad.IsActive) return null;

            return _mapper.Map<TrasladoDto>(entidad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener traslado {Id}", id);
            throw;
        }
    }

    public async Task<TrasladoDto> CrearPorLoteAsync(CrearTrasladoPorLoteDto dto)
    {
        try
        {
            if (dto.CantidadUnidades <= 0)
                throw new ValidacionException("La cantidad de unidades debe ser mayor a 0");

            var loteOrigen = await _loteAlmacenRepo.ObtenerPorIdAsync(dto.LoteAlmacenOrigenId);
            ValidacionEntidad.VerificarActivo(loteOrigen, "Lote almacén origen");

            if (dto.CantidadUnidades > loteOrigen!.StockDisponible)
                throw new ValidacionException("La cantidad a trasladar excede el stock disponible del lote origen");

            if (loteOrigen.AlmacenId == dto.AlmacenDestinoId)
                throw new ValidacionException("El almacén de origen y destino no pueden ser el mismo");

            var lote = await _loteRepo.ObtenerPorIdAsync(loteOrigen.LoteId);
            if (lote == null)
                throw new EntidadNoEncontradaException("InventarioLote", loteOrigen.LoteId);

            await _uow.BeginTransactionAsync();
            try
            {
                var cantidadRestante = dto.CantidadUnidades;
                var costoUnitario = lote.CostoCompraUnitario;
                var varianteId = lote.VarianteId;

                loteOrigen.StockDisponible -= dto.CantidadUnidades;
                loteOrigen.CantidadTrasladada += dto.CantidadUnidades;

                if (loteOrigen.StockDisponible == 0)
                    loteOrigen.EstadoLote = EstadoLote.Agotado;

                await _loteAlmacenRepo.ActualizarAsync(loteOrigen);

                var destinoExistente = await _loteAlmacenRepo.ObtenerPorLoteYAlmacenAsync(
                    loteOrigen.LoteId, dto.AlmacenDestinoId);

                InventarioLoteAlmacen loteDestino;
                if (destinoExistente != null)
                {
                    destinoExistente.StockInicial += dto.CantidadUnidades;
                    destinoExistente.StockDisponible += dto.CantidadUnidades;
                    await _loteAlmacenRepo.ActualizarAsync(destinoExistente);
                    loteDestino = destinoExistente;
                }
                else
                {
                    loteDestino = new InventarioLoteAlmacen
                    {
                        LoteId = loteOrigen.LoteId,
                        AlmacenId = dto.AlmacenDestinoId,
                        StockInicial = dto.CantidadUnidades,
                        StockDisponible = dto.CantidadUnidades,
                        CantidadVendida = 0,
                        CantidadTrasladada = 0,
                        CantidadVencida = 0,
                        EstadoLote = EstadoLote.Activo
                    };
                    loteDestino = await _loteAlmacenRepo.CrearSinGuardarAsync(loteDestino);
                }

                var detalle = new TrasladoDetalle
                {
                    LoteAlmacenOrigenId = loteOrigen.Id,
                    LoteAlmacenDestinoId = loteDestino.Id,
                    CantidadUnidades = dto.CantidadUnidades,
                    CostoUnitarioCapturado = costoUnitario
                };

                var entidad = new Traslado
                {
                    TipoTraslado = TipoTraslado.PorLote,
                    VarianteId = varianteId,
                    AlmacenOrigenId = loteOrigen.AlmacenId,
                    AlmacenDestinoId = dto.AlmacenDestinoId,
                    CantidadUnidades = dto.CantidadUnidades,
                    Observacion = dto.Observacion,
                    FechaTraslado = DateTime.UtcNow,
                    Detalles = new List<TrasladoDetalle> { detalle }
                };

                var creado = await _trasladoRepo.CrearAsync(entidad);

                var movimientoSalida = new MovimientoInventario
                {
                    LoteAlmacenId = loteOrigen.Id,
                    VarianteId = varianteId,
                    AlmacenOrigenId = loteOrigen.AlmacenId,
                    AlmacenDestinoId = dto.AlmacenDestinoId,
                    TipoMovimiento = TipoMovimiento.Traslado,
                    CantidadUnidades = dto.CantidadUnidades,
                    SaldoResultante = loteOrigen.StockDisponible,
                    ReferenciaId = creado.Id,
                    Observacion = $"Traslado por lote - salida de almacén {loteOrigen.AlmacenId}"
                };
                await _movimientoRepo.CrearSinGuardarAsync(movimientoSalida);

                var movimientoEntrada = new MovimientoInventario
                {
                    LoteAlmacenId = loteDestino.Id,
                    VarianteId = varianteId,
                    AlmacenOrigenId = loteOrigen.AlmacenId,
                    AlmacenDestinoId = dto.AlmacenDestinoId,
                    TipoMovimiento = TipoMovimiento.Traslado,
                    CantidadUnidades = dto.CantidadUnidades,
                    SaldoResultante = loteDestino.StockDisponible,
                    ReferenciaId = creado.Id,
                    Observacion = $"Traslado por lote - entrada a almacén {dto.AlmacenDestinoId}"
                };
                await _movimientoRepo.CrearSinGuardarAsync(movimientoEntrada);

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                _logger.LogInformation(
                    "Traslado por lote creado: {Id} - Variante: {VarianteId} - Cantidad: {Cantidad}",
                    creado.Id, varianteId, dto.CantidadUnidades);

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
            _logger.LogError(ex, "Error al crear traslado por lote");
            throw;
        }
    }

    public async Task<TrasladoDto> CrearPorVarianteAsync(CrearTrasladoPorVarianteDto dto)
    {
        try
        {
            if (dto.CantidadUnidades <= 0)
                throw new ValidacionException("La cantidad de unidades debe ser mayor a 0");

            if (dto.AlmacenOrigenId == dto.AlmacenDestinoId)
                throw new ValidacionException("El almacén de origen y destino no pueden ser el mismo");

            var stockDisponible = await _loteAlmacenRepo.ObtenerStockDisponibleAsync(
                dto.VarianteId, dto.AlmacenOrigenId);

            if (dto.CantidadUnidades > stockDisponible)
                throw new ValidacionException(
                    $"La cantidad a trasladar ({dto.CantidadUnidades}) excede el stock disponible ({stockDisponible}) de la variante en el almacén origen");

            var lotesFEFO = await _loteAlmacenRepo.ObtenerLotesFEFOAsync(
                dto.VarianteId, dto.AlmacenOrigenId);

            await _uow.BeginTransactionAsync();
            try
            {
                var cantidadRestante = dto.CantidadUnidades;
                var detalles = new List<TrasladoDetalle>();
                var movimientos = new List<MovimientoInventario>();

                foreach (var loteOrigen in lotesFEFO)
                {
                    if (cantidadRestante <= 0)
                        break;

                    if (loteOrigen.StockDisponible <= 0)
                        continue;

                    var cantidadParaEsteLote = Math.Min(cantidadRestante, loteOrigen.StockDisponible);

                    var lote = await _loteRepo.ObtenerPorIdAsync(loteOrigen.LoteId);
                    if (lote == null)
                        continue;

                    loteOrigen.StockDisponible -= cantidadParaEsteLote;
                    loteOrigen.CantidadTrasladada += cantidadParaEsteLote;

                    if (loteOrigen.StockDisponible == 0)
                        loteOrigen.EstadoLote = EstadoLote.Agotado;

                    await _loteAlmacenRepo.ActualizarAsync(loteOrigen);

                    var destinoExistente = await _loteAlmacenRepo.ObtenerPorLoteYAlmacenAsync(
                        loteOrigen.LoteId, dto.AlmacenDestinoId);

                    InventarioLoteAlmacen loteDestino;
                    if (destinoExistente != null)
                    {
                        destinoExistente.StockInicial += cantidadParaEsteLote;
                        destinoExistente.StockDisponible += cantidadParaEsteLote;
                        await _loteAlmacenRepo.ActualizarAsync(destinoExistente);
                        loteDestino = destinoExistente;
                    }
                    else
                    {
                        loteDestino = new InventarioLoteAlmacen
                        {
                            LoteId = loteOrigen.LoteId,
                            AlmacenId = dto.AlmacenDestinoId,
                            StockInicial = cantidadParaEsteLote,
                            StockDisponible = cantidadParaEsteLote,
                            CantidadVendida = 0,
                            CantidadTrasladada = 0,
                            CantidadVencida = 0,
                            EstadoLote = EstadoLote.Activo
                        };
                        loteDestino = await _loteAlmacenRepo.CrearSinGuardarAsync(loteDestino);
                    }

                    detalles.Add(new TrasladoDetalle
                    {
                        LoteAlmacenOrigenId = loteOrigen.Id,
                        LoteAlmacenDestinoId = loteDestino.Id,
                        CantidadUnidades = cantidadParaEsteLote,
                        CostoUnitarioCapturado = lote.CostoCompraUnitario
                    });

                    movimientos.Add(new MovimientoInventario
                    {
                        LoteAlmacenId = loteOrigen.Id,
                        VarianteId = dto.VarianteId,
                        AlmacenOrigenId = dto.AlmacenOrigenId,
                        AlmacenDestinoId = dto.AlmacenDestinoId,
                        TipoMovimiento = TipoMovimiento.Traslado,
                        CantidadUnidades = cantidadParaEsteLote,
                        SaldoResultante = loteOrigen.StockDisponible,
                        Observacion = $"Traslado FEFO - salida de almacén {dto.AlmacenOrigenId}"
                    });

                    movimientos.Add(new MovimientoInventario
                    {
                        LoteAlmacenId = loteDestino.Id,
                        VarianteId = dto.VarianteId,
                        AlmacenOrigenId = dto.AlmacenOrigenId,
                        AlmacenDestinoId = dto.AlmacenDestinoId,
                        TipoMovimiento = TipoMovimiento.Traslado,
                        CantidadUnidades = cantidadParaEsteLote,
                        SaldoResultante = loteDestino.StockDisponible,
                        Observacion = $"Traslado FEFO - entrada a almacén {dto.AlmacenDestinoId}"
                    });

                    cantidadRestante -= cantidadParaEsteLote;
                }

                if (cantidadRestante > 0)
                    throw new ValidacionException(
                        $"No hay suficiente stock para completar el traslado. Faltan {cantidadRestante} unidades");

                var entidad = new Traslado
                {
                    TipoTraslado = TipoTraslado.PorVariante,
                    VarianteId = dto.VarianteId,
                    AlmacenOrigenId = dto.AlmacenOrigenId,
                    AlmacenDestinoId = dto.AlmacenDestinoId,
                    CantidadUnidades = dto.CantidadUnidades,
                    Observacion = dto.Observacion,
                    FechaTraslado = DateTime.UtcNow,
                    Detalles = detalles
                };

                var creado = await _trasladoRepo.CrearAsync(entidad);

                foreach (var mov in movimientos)
                {
                    mov.ReferenciaId = creado.Id;
                    await _movimientoRepo.CrearSinGuardarAsync(mov);
                }

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                _logger.LogInformation(
                    "Traslado por variante creado: {Id} - Variante: {VarianteId} - Cantidad: {Cantidad} - Lotes: {LotesCount}",
                    creado.Id, dto.VarianteId, dto.CantidadUnidades, detalles.Count);

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
            _logger.LogError(ex, "Error al crear traslado por variante");
            throw;
        }
    }
}
