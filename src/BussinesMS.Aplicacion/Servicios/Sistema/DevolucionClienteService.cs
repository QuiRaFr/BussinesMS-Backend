using AutoMapper;
using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Helpers;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using BussinesMS.Dominio.Entidades.Sistema;
using BussinesMS.Dominio.Enums;
using BussinesMS.Dominio.Excepciones;
using Microsoft.Extensions.Logging;

namespace BussinesMS.Aplicacion.Servicios.Sistema;

public class DevolucionClienteService : IDevolucionClienteService
{
    private readonly IDevolucionClienteRepository _repo;
    private readonly IInventarioLoteRepository _loteRepo;
    private readonly IMovimientoInventarioRepository _movimientoRepo;
    private readonly ISistemaUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<DevolucionClienteService> _logger;

    public DevolucionClienteService(
        IDevolucionClienteRepository repo,
        IInventarioLoteRepository loteRepo,
        IMovimientoInventarioRepository movimientoRepo,
        ISistemaUnitOfWork uow,
        IMapper mapper,
        ILogger<DevolucionClienteService> logger)
    {
        _repo = repo;
        _loteRepo = loteRepo;
        _movimientoRepo = movimientoRepo;
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DevolucionClienteDto> CrearAsync(CrearDevolucionClienteDto dto)
    {
        try
        {
            if (dto.CantidadUnidades <= 0)
                throw new ValidacionException("La cantidad de unidades debe ser mayor a 0");

            if (string.IsNullOrWhiteSpace(dto.Motivo))
                throw new ValidacionException("El motivo es obligatorio");

            var loteOrigen = await _loteRepo.ObtenerPorIdAsync(dto.LoteOrigenId);
            ValidacionEntidad.VerificarActivo(loteOrigen, "Lote origen");

            if (loteOrigen!.StockDisponible < dto.CantidadUnidades)
                throw new ValidacionException("El stock disponible del lote origen es insuficiente");

            if (loteOrigen.AlmacenId != dto.AlmacenId)
                throw new ValidacionException("El almacén de la devolución no coincide con el almacén del lote origen");

            await _uow.BeginTransactionAsync();
            try
            {
                loteOrigen.StockDisponible -= dto.CantidadUnidades;
                await _loteRepo.ActualizarAsync(loteOrigen);

                var loteDevuelto = new InventarioLote
                {
                    VarianteId = loteOrigen.VarianteId,
                    AlmacenId = dto.AlmacenId,
                    CompraDetalleId = null,
                    StockInicial = dto.CantidadUnidades,
                    StockDisponible = dto.CantidadUnidades,
                    CantidadVendida = 0,
                    CantidadTrasladada = 0,
                    CantidadVencida = 0,
                    CostoCompraUnitario = loteOrigen.CostoCompraUnitario,
                    PrecioVentaUnitario = loteOrigen.PrecioVentaUnitario,
                    PrecioVentaMayoreo = loteOrigen.PrecioVentaMayoreo,
                    FechaVencimiento = loteOrigen.FechaVencimiento,
                    EstadoLote = EstadoLote.Devuelto
                };
                await _loteRepo.CrearSinGuardarAsync(loteDevuelto);
                await _uow.SaveChangesAsync();

                var devolucion = new DevolucionCliente
                {
                    LoteOrigenId = dto.LoteOrigenId,
                    LoteDevueltoId = loteDevuelto.Id,
                    VarianteId = dto.VarianteId,
                    AlmacenId = dto.AlmacenId,
                    CantidadUnidades = dto.CantidadUnidades,
                    Motivo = dto.Motivo,
                    EstadoDevolucion = EstadoDevolucion.PendienteCambio,
                    Observacion = dto.Observacion,
                    FechaDevolucion = DateTime.UtcNow
                };
                var creada = await _repo.CrearAsync(devolucion);

                var movimiento = new MovimientoInventario
                {
                    LoteId = loteOrigen.Id,
                    VarianteId = loteOrigen.VarianteId,
                    AlmacenDestinoId = dto.AlmacenId,
                    TipoMovimiento = TipoMovimiento.DevolucionCliente,
                    CantidadUnidades = dto.CantidadUnidades,
                    SaldoResultante = loteOrigen.StockDisponible,
                    ReferenciaId = creada.Id,
                    Observacion = $"Devolución de cliente - {dto.Motivo}"
                };
                await _movimientoRepo.CrearAsync(movimiento);

                await _uow.CommitAsync();

                _logger.LogInformation("Devolución creada: {Id} - Variante: {VarianteId} - Cantidad: {Cantidad}",
                    creada.Id, dto.VarianteId, dto.CantidadUnidades);

                return _mapper.Map<DevolucionClienteDto>(creada);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear devolución de cliente");
            throw;
        }
    }

    public async Task RealizarCambioAsync(RealizarCambioDto dto)
    {
        try
        {
            var devolucion = await _repo.ObtenerConDetallesAsync(dto.DevolucionId);
            ValidacionEntidad.VerificarActivo(devolucion, "Devolución");

            if (devolucion!.EstadoDevolucion != EstadoDevolucion.PendienteCambio)
                throw new ValidacionException("La devolución no está en estado Pendiente de Cambio");

            var loteDestino = await _loteRepo.ObtenerPorIdAsync(dto.LoteDestinoId);
            ValidacionEntidad.VerificarActivo(loteDestino, "Lote destino");

            if (loteDestino!.StockDisponible < devolucion.CantidadUnidades)
                throw new ValidacionException("El stock disponible del lote destino es insuficiente");

            await _uow.BeginTransactionAsync();
            try
            {
                loteDestino.StockDisponible -= devolucion.CantidadUnidades;
                await _loteRepo.ActualizarAsync(loteDestino);

                var movimiento = new MovimientoInventario
                {
                    LoteId = loteDestino.Id,
                    VarianteId = devolucion.VarianteId,
                    AlmacenOrigenId = loteDestino.AlmacenId,
                    TipoMovimiento = TipoMovimiento.SalidaCambio,
                    CantidadUnidades = devolucion.CantidadUnidades,
                    SaldoResultante = loteDestino.StockDisponible,
                    ReferenciaId = devolucion.Id,
                    Observacion = $"Salida por cambio de devolución #{devolucion.Id}"
                };
                await _movimientoRepo.CrearAsync(movimiento);

                devolucion.EstadoDevolucion = EstadoDevolucion.CambioRealizado;
                await _repo.ActualizarAsync(devolucion);

                await _uow.CommitAsync();

                _logger.LogInformation("Cambio realizado en devolución: {Id}", devolucion.Id);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar cambio de devolución");
            throw;
        }
    }

    public async Task DarDeBajaAsync(DarDeBajaDevolucionDto dto)
    {
        try
        {
            var devolucion = await _repo.ObtenerConDetallesAsync(dto.DevolucionId);
            ValidacionEntidad.VerificarActivo(devolucion, "Devolución");

            if (devolucion!.EstadoDevolucion != EstadoDevolucion.PendienteCambio)
                throw new ValidacionException("La devolución no está en estado Pendiente de Cambio");

            if (devolucion.LoteDevueltoId == null)
                throw new ValidacionException("La devolución no tiene lote devuelto asociado");

            var loteDevuelto = await _loteRepo.ObtenerPorIdAsync(devolucion.LoteDevueltoId.Value);
            ValidacionEntidad.VerificarActivo(loteDevuelto, "Lote devuelto");

            await _uow.BeginTransactionAsync();
            try
            {
                var cantidadBaja = loteDevuelto!.StockDisponible;

                loteDevuelto.StockDisponible = 0;
                loteDevuelto.EstadoLote = EstadoLote.Baja;
                await _loteRepo.ActualizarAsync(loteDevuelto);

                var movimiento = new MovimientoInventario
                {
                    LoteId = loteDevuelto.Id,
                    VarianteId = devolucion.VarianteId,
                    AlmacenOrigenId = loteDevuelto.AlmacenId,
                    TipoMovimiento = TipoMovimiento.AjusteNegativo,
                    CantidadUnidades = cantidadBaja,
                    SaldoResultante = 0,
                    ReferenciaId = devolucion.Id,
                    Observacion = dto.Observacion ?? $"Baja de devolución #{devolucion.Id}"
                };
                await _movimientoRepo.CrearAsync(movimiento);

                devolucion.EstadoDevolucion = EstadoDevolucion.DadoDeBaja;
                if (dto.Observacion != null)
                    devolucion.Observacion = dto.Observacion;
                await _repo.ActualizarAsync(devolucion);

                await _uow.CommitAsync();

                _logger.LogInformation("Devolución dada de baja: {Id}", devolucion.Id);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al dar de baja devolución");
            throw;
        }
    }

    public async Task<DevolucionClienteDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var entidad = await _repo.ObtenerConDetallesAsync(id);
            if (entidad == null || !entidad.IsActive) return null;

            return _mapper.Map<DevolucionClienteDto>(entidad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener devolución {Id}", id);
            throw;
        }
    }
}
