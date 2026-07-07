using BussinesMS.Aplicacion.Common;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using BussinesMS.Dominio.Entidades.Sistema;
using BussinesMS.Dominio.Enums;
using BussinesMS.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BussinesMS.Infraestructura.Jobs;

public class VencimientoLotesJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<VencimientoLotesJob> _logger;
    private readonly PeriodicTimer _timer;

    public VencimientoLotesJob(
        IServiceScopeFactory scopeFactory,
        ILogger<VencimientoLotesJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _timer = new PeriodicTimer(TimeSpan.FromHours(24));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("VencimientoLotesJob iniciado");

        while (await _timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcesarLotesVencidosAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en VencimientoLotesJob");
            }
        }
    }

    private async Task ProcesarLotesVencidosAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var contexto = scope.ServiceProvider.GetRequiredService<SistemaDbContext>();
        var movimientoRepo = scope.ServiceProvider.GetRequiredService<IMovimientoInventarioRepository>();

        var (inicioHoyUtc, _) = BoliviaTimeZone.RangoDiaUtc(DateOnly.FromDateTime(DateTime.UtcNow));

        var lotesAVencer = await contexto.InventarioLoteAlmacenes
            .Include(la => la.Lote)
            .Where(la => la.Lote!.FechaVencimiento != null
                     && la.Lote!.FechaVencimiento < inicioHoyUtc
                     && la.EstadoLote == EstadoLote.Activo
                     && la.StockDisponible > 0
                     && la.Lote!.IsActive)
            .ToListAsync();

        if (!lotesAVencer.Any())
        {
            _logger.LogInformation("VencimientoLotesJob: No hay lotes por vencer hoy");
            return;
        }

        _logger.LogInformation("VencimientoLotesJob: Procesando {Cantidad} lotes vencidos", lotesAVencer.Count);

        foreach (var loteAlmacen in lotesAVencer)
        {
            var cantidadVencida = loteAlmacen.StockDisponible;

            loteAlmacen.CantidadVencida += cantidadVencida;
            loteAlmacen.StockDisponible = 0;
            loteAlmacen.EstadoLote = EstadoLote.Vencido;

            contexto.InventarioLoteAlmacenes.Update(loteAlmacen);

            var movimiento = new MovimientoInventario
            {
                LoteAlmacenId = loteAlmacen.Id,
                VarianteId = loteAlmacen.Lote!.VarianteId,
                AlmacenOrigenId = loteAlmacen.AlmacenId,
                TipoMovimiento = TipoMovimiento.Vencimiento,
                CantidadUnidades = cantidadVencida,
                SaldoResultante = 0,
                Observacion = $"Vencimiento automático - Stock vencido: {cantidadVencida}"
            };
            await movimientoRepo.CrearAsync(movimiento);

            _logger.LogInformation(
                "Lote {LoteAlmacenId} vencido: {Cantidad} unidades marcadas como vencidas",
                loteAlmacen.Id, cantidadVencida);
        }

        await contexto.SaveChangesAsync();
        _logger.LogInformation("VencimientoLotesJob: {Cantidad} lotes procesados exitosamente", lotesAVencer.Count);
    }
}
