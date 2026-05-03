using BussinesMS.Aplicacion.Interfaces.Sistema;
using BussinesMS.Aplicacion.Seguridad;
using BussinesMS.Dominio.Entidades.Sistema;
using BussinesMS.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace BussinesMS.Infraestructura.Repositorios.Sistema;

public class TipoPresentacionRepository : ITipoPresentacionRepository
{
    private readonly SistemaDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public TipoPresentacionRepository(SistemaDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public IQueryable<TipoPresentacion> AsQueryable()
    {
        return _context.TipoPresentaciones.AsQueryable();
    }

    public async Task<List<TipoPresentacion>> ObtenerTodosAsync()
    {
        return await _context.TipoPresentaciones
            .Where(t => t.IsActive)
            .OrderBy(t => t.Nombre)
            .ToListAsync();
    }

    public async Task<TipoPresentacion?> ObtenerPorIdAsync(int id)
    {
        return await _context.TipoPresentaciones.FindAsync(id);
    }

    public async Task<TipoPresentacion> CrearAsync(TipoPresentacion entidad)
    {
        var usuarioId = _currentUser.GetUsuarioId() ?? 1;
        entidad.CreatedByUsuarioId = usuarioId;
        entidad.CreatedAt = DateTime.UtcNow;
        entidad.IsActive = true;

        _context.TipoPresentaciones.Add(entidad);
        await _context.SaveChangesAsync();
        return entidad;
    }

    public async Task<TipoPresentacion> ActualizarAsync(TipoPresentacion entidad)
    {
        var usuarioId = _currentUser.GetUsuarioId();
        if (usuarioId.HasValue)
        {
            entidad.UpdatedByUsuarioId = usuarioId;
            entidad.UpdatedAt = DateTime.UtcNow;
        }

        _context.TipoPresentaciones.Update(entidad);
        await _context.SaveChangesAsync();
        return entidad;
    }

    public async Task EliminarAsync(int id)
    {
        var entidad = await _context.TipoPresentaciones.FindAsync(id);
        if (entidad != null)
        {
            var usuarioId = _currentUser.GetUsuarioId();
            if (usuarioId.HasValue)
            {
                entidad.DeletedByUsuarioId = usuarioId;
                entidad.DeletedAt = DateTime.UtcNow;
                entidad.IsActive = false;
                _context.TipoPresentaciones.Update(entidad);
            }
            else
            {
                _context.TipoPresentaciones.Remove(entidad);
            }
            await _context.SaveChangesAsync();
        }
    }
}