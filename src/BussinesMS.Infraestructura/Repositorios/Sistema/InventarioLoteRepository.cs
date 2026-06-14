using BussinesMS.Aplicacion.Interfaces.Sistema;
using BussinesMS.Aplicacion.Seguridad;
using BussinesMS.Dominio.Entidades.Sistema;
using BussinesMS.Dominio.Enums;
using BussinesMS.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace BussinesMS.Infraestructura.Repositorios.Sistema;

public class InventarioLoteRepository : IInventarioLoteRepository
{
    private readonly SistemaDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public InventarioLoteRepository(SistemaDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public IQueryable<InventarioLote> AsQueryable()
        => _context.InventarioLotes.AsQueryable();

    public async Task<List<InventarioLote>> ObtenerTodosAsync()
        => await _context.InventarioLotes
            .Where(x => x.IsActive)
            .OrderBy(x => x.FechaVencimiento)
            .ToListAsync();

    public async Task<InventarioLote?> ObtenerPorIdAsync(int id)
        => await _context.InventarioLotes
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<InventarioLote?> ObtenerConDetallesAsync(int id)
        => await _context.InventarioLotes
            .Include(x => x.Variante)
            .Include(x => x.CompraDetalle)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<InventarioLote>> ObtenerLotesFEFOAsync(int varianteId, int almacenId)
        => await _context.InventarioLotes
            .Where(x => x.VarianteId == varianteId
                     && x.AlmacenId == almacenId
                     && x.IsActive
                     && x.EstadoLote == EstadoLote.Activo
                     && x.StockDisponible > 0)
            .OrderBy(x => x.FechaVencimiento)
            .ToListAsync();

    public async Task<int> ObtenerStockDisponibleAsync(int varianteId, int almacenId)
        => await _context.InventarioLotes
            .Where(x => x.VarianteId == varianteId
                     && x.AlmacenId == almacenId
                     && x.IsActive
                     && x.EstadoLote == EstadoLote.Activo)
            .SumAsync(x => x.StockDisponible);

    public async Task<InventarioLote> CrearAsync(InventarioLote entidad)
    {
        var usuarioId = _currentUser.GetUsuarioId() ?? 1;
        entidad.CreatedByUsuarioId = usuarioId;
        entidad.CreatedAt = DateTime.UtcNow;
        entidad.IsActive = true;
        _context.InventarioLotes.Add(entidad);
        await _context.SaveChangesAsync();
        return entidad;
    }

    public async Task<InventarioLote> CrearSinGuardarAsync(InventarioLote entidad)
    {
        var usuarioId = _currentUser.GetUsuarioId() ?? 1;
        entidad.CreatedByUsuarioId = usuarioId;
        entidad.CreatedAt = DateTime.UtcNow;
        entidad.IsActive = true;
        _context.InventarioLotes.Add(entidad);
        return entidad;
    }

    public async Task<InventarioLote> ActualizarAsync(InventarioLote entidad)
    {
        var usuarioId = _currentUser.GetUsuarioId() ?? 1;
        entidad.UpdatedByUsuarioId = usuarioId;
        entidad.UpdatedAt = DateTime.UtcNow;
        _context.InventarioLotes.Update(entidad);
        await _context.SaveChangesAsync();
        return entidad;
    }

    public async Task EliminarAsync(int id)
    {
        var entidad = await _context.InventarioLotes.FindAsync(id);
        if (entidad == null) return;

        var usuarioId = _currentUser.GetUsuarioId() ?? 1;
        entidad.DeletedByUsuarioId = usuarioId;
        entidad.DeletedAt = DateTime.UtcNow;
        entidad.IsActive = false;
        _context.InventarioLotes.Update(entidad);
        await _context.SaveChangesAsync();
    }
}
