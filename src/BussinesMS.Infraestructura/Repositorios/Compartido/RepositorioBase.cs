using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Aplicacion.Seguridad;
using BussinesMS.Dominio.Entidades.Compartido;
using Microsoft.EntityFrameworkCore;

namespace BussinesMS.Infraestructura.Repositorios.Compartido;

public class RepositorioBase<T> : IRepositorio<T> where T : EntidadBase
{
    protected readonly DbContext _contexto;
    protected readonly DbSet<T> _dbSet;
    protected readonly ICurrentUserService _currentUser;

    public RepositorioBase(DbContext contexto, ICurrentUserService currentUser)
    {
        _contexto = contexto;
        _dbSet = contexto.Set<T>();
        _currentUser = currentUser;
    }

    public virtual IQueryable<T> AsQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public virtual async Task<List<T>> ObtenerTodosAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T?> ObtenerPorIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T> CrearAsync(T entidad)
    {
        var usuarioId = _currentUser.GetUsuarioId() ?? 1;
        entidad.CreatedByUsuarioId = usuarioId;
        entidad.CreatedAt = DateTime.UtcNow;
        
        await _dbSet.AddAsync(entidad);
        await _contexto.SaveChangesAsync();
        return entidad;
    }

    public virtual async Task<T> ActualizarAsync(T entidad)
    {
        var usuarioId = _currentUser.GetUsuarioId();
        if (usuarioId.HasValue)
        {
            entidad.UpdatedByUsuarioId = usuarioId;
            entidad.UpdatedAt = DateTime.UtcNow;
        }
        
        _dbSet.Update(entidad);
        await _contexto.SaveChangesAsync();
        return entidad;
    }

    public virtual async Task EliminarAsync(int id)
    {
        var entidad = await _dbSet.FindAsync(id);
        if (entidad != null)
        {
            var usuarioId = _currentUser.GetUsuarioId();
            if (usuarioId.HasValue)
            {
                entidad.DeletedByUsuarioId = usuarioId;
                entidad.DeletedAt = DateTime.UtcNow;
                entidad.IsActive = false;
                _dbSet.Update(entidad);
            }
            else
            {
                _dbSet.Remove(entidad);
            }
            await _contexto.SaveChangesAsync();
        }
    }
}