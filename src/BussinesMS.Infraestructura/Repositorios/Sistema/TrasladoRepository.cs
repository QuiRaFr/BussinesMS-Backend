using BussinesMS.Aplicacion.Interfaces.Sistema;
using BussinesMS.Aplicacion.Seguridad;
using BussinesMS.Dominio.Entidades.Sistema;
using BussinesMS.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace BussinesMS.Infraestructura.Repositorios.Sistema;

public class TrasladoRepository : ITrasladoRepository
{
    private readonly SistemaDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public TrasladoRepository(SistemaDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public IQueryable<Traslado> AsQueryable()
        => _context.Traslados.AsQueryable();

    public async Task<List<Traslado>> ObtenerTodosAsync()
        => await _context.Traslados
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.FechaTraslado)
            .ToListAsync();

    public async Task<Traslado?> ObtenerPorIdAsync(int id)
        => await _context.Traslados
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Traslado?> ObtenerConDetallesAsync(int id)
        => await _context.Traslados
            .Include(x => x.Lote)
            .Include(x => x.LoteDestino)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Traslado> CrearAsync(Traslado entidad)
    {
        var usuarioId = _currentUser.GetUsuarioId() ?? 1;
        entidad.CreatedByUsuarioId = usuarioId;
        entidad.UsuarioId = usuarioId;
        entidad.CreatedAt = DateTime.UtcNow;
        entidad.IsActive = true;
        _context.Traslados.Add(entidad);
        await _context.SaveChangesAsync();
        return entidad;
    }

    public async Task<Traslado> ActualizarAsync(Traslado entidad)
    {
        var usuarioId = _currentUser.GetUsuarioId() ?? 1;
        entidad.UpdatedByUsuarioId = usuarioId;
        entidad.UpdatedAt = DateTime.UtcNow;
        _context.Traslados.Update(entidad);
        await _context.SaveChangesAsync();
        return entidad;
    }

    public async Task EliminarAsync(int id)
    {
        var entidad = await _context.Traslados.FindAsync(id);
        if (entidad == null) return;

        var usuarioId = _currentUser.GetUsuarioId() ?? 1;
        entidad.DeletedByUsuarioId = usuarioId;
        entidad.DeletedAt = DateTime.UtcNow;
        entidad.IsActive = false;
        _context.Traslados.Update(entidad);
        await _context.SaveChangesAsync();
    }
}
