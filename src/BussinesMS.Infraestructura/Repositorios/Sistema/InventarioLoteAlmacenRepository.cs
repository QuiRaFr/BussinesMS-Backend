using BussinesMS.Aplicacion.Interfaces.Sistema;
using BussinesMS.Aplicacion.Seguridad;
using BussinesMS.Dominio.Entidades.Sistema;
using BussinesMS.Dominio.Enums;
using BussinesMS.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace BussinesMS.Infraestructura.Repositorios.Sistema;

public class InventarioLoteAlmacenRepository : IInventarioLoteAlmacenRepository
{
    private readonly SistemaDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public InventarioLoteAlmacenRepository(SistemaDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public IQueryable<InventarioLoteAlmacen> AsQueryable()
        => _context.InventarioLoteAlmacenes.AsQueryable();

    public async Task<List<InventarioLoteAlmacen>> ObtenerTodosAsync()
        => await _context.InventarioLoteAlmacenes
            .Where(x => x.IsActive)
            .OrderBy(x => x.Lote!.FechaVencimiento)
            .ToListAsync();

    public async Task<InventarioLoteAlmacen?> ObtenerPorIdAsync(int id)
        => await _context.InventarioLoteAlmacenes
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<InventarioLoteAlmacen?> ObtenerConDetallesAsync(int id)
        => await _context.InventarioLoteAlmacenes
            .Include(x => x.Lote)
                .ThenInclude(l => l!.Variante)
                    .ThenInclude(v => v!.Producto)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<InventarioLoteAlmacen>> ObtenerLotesFEFOAsync(int varianteId, int almacenId)
        => await _context.InventarioLoteAlmacenes
            .Include(x => x.Lote)
            .Where(x => x.Lote!.VarianteId == varianteId
                     && x.AlmacenId == almacenId
                     && x.IsActive
                     && x.EstadoLote == EstadoLote.Activo
                     && x.StockDisponible > 0)
            .OrderBy(x => x.Lote!.FechaVencimiento)
            .ToListAsync();

    public async Task<int> ObtenerStockDisponibleAsync(int varianteId, int almacenId)
        => await _context.InventarioLoteAlmacenes
            .Where(x => x.Lote!.VarianteId == varianteId
                     && x.AlmacenId == almacenId
                     && x.IsActive
                     && x.EstadoLote == EstadoLote.Activo)
            .SumAsync(x => x.StockDisponible);

    public async Task<int> ObtenerStockTotalVarianteAsync(int varianteId, int almacenId)
        => await _context.InventarioLoteAlmacenes
            .Where(x => x.Lote!.VarianteId == varianteId
                     && x.AlmacenId == almacenId
                     && x.IsActive
                     && x.EstadoLote == EstadoLote.Activo
                     && x.StockDisponible > 0)
            .SumAsync(x => x.StockDisponible);

    public async Task<InventarioLoteAlmacen?> ObtenerPorLoteYAlmacenAsync(int loteId, int almacenId)
        => await _context.InventarioLoteAlmacenes
            .FirstOrDefaultAsync(x => x.LoteId == loteId
                                   && x.AlmacenId == almacenId
                                   && x.IsActive);

    public async Task<InventarioLoteAlmacen> CrearAsync(InventarioLoteAlmacen entidad)
    {
        var usuarioId = _currentUser.GetUsuarioId() ?? 1;
        entidad.CreatedByUsuarioId = usuarioId;
        entidad.CreatedAt = DateTime.UtcNow;
        entidad.IsActive = true;
        _context.InventarioLoteAlmacenes.Add(entidad);
        await _context.SaveChangesAsync();
        return entidad;
    }

    public async Task<InventarioLoteAlmacen> CrearSinGuardarAsync(InventarioLoteAlmacen entidad)
    {
        var usuarioId = _currentUser.GetUsuarioId() ?? 1;
        entidad.CreatedByUsuarioId = usuarioId;
        entidad.CreatedAt = DateTime.UtcNow;
        entidad.IsActive = true;
        _context.InventarioLoteAlmacenes.Add(entidad);
        return entidad;
    }

    public async Task<InventarioLoteAlmacen> ActualizarAsync(InventarioLoteAlmacen entidad)
    {
        var usuarioId = _currentUser.GetUsuarioId() ?? 1;
        entidad.UpdatedByUsuarioId = usuarioId;
        entidad.UpdatedAt = DateTime.UtcNow;
        _context.InventarioLoteAlmacenes.Update(entidad);
        await _context.SaveChangesAsync();
        return entidad;
    }

    public async Task EliminarAsync(int id)
    {
        var entidad = await _context.InventarioLoteAlmacenes.FindAsync(id);
        if (entidad == null) return;

        var usuarioId = _currentUser.GetUsuarioId() ?? 1;
        entidad.DeletedByUsuarioId = usuarioId;
        entidad.DeletedAt = DateTime.UtcNow;
        entidad.IsActive = false;
        _context.InventarioLoteAlmacenes.Update(entidad);
        await _context.SaveChangesAsync();
    }
}
