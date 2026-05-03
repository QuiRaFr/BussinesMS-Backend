using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using BussinesMS.Aplicacion.Seguridad;
using BussinesMS.Dominio.Entidades.Sistema;
using BussinesMS.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace BussinesMS.Infraestructura.Repositorios.Sistema;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly SistemaDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CategoriaRepository(SistemaDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public IQueryable<Categoria> AsQueryable()
    {
        return _context.Categorias
            .Include(c => c.Parent)
            .AsQueryable();
    }

    public async Task<List<CategoriaDto>> ObtenerTodosDtoAsync()
    {
        var categorias = await _context.Categorias
            .Include(c => c.Parent)
            .Where(c => c.IsActive)
            .OrderBy(c => c.Nombre)
            .ToListAsync();

        return categorias.Select(c => new CategoriaDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            ParentId = c.ParentId,
            NombrePadre = c.Parent != null ? c.Parent.Nombre : null,
            CreatedAt = c.CreatedAt,
            IsActive = c.IsActive
        }).ToList();
    }

    public async Task<List<Categoria>> ObtenerTodosAsync()
    {
        return await _context.Categorias
            .Where(c => c.IsActive)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<Categoria?> ObtenerPorIdAsync(int id)
    {
        return await _context.Categorias
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Categoria>> ObtenerRaicesAsync()
    {
        return await _context.Categorias
            .Include(c => c.Subcategorias)
            .Where(c => c.ParentId == null && c.IsActive)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<List<Categoria>> ObtenerSubcategoriasAsync(int parentId)
    {
        return await _context.Categorias
            .Where(c => c.ParentId == parentId && c.IsActive)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<Categoria?> ObtenerPorNombreAsync(string nombre)
    {
        return await _context.Categorias
            .FirstOrDefaultAsync(c => c.Nombre.ToLower() == nombre.ToLower() && c.IsActive);
    }

    public async Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null)
    {
        var query = _context.Categorias
            .Where(c => c.Nombre.ToLower() == nombre.ToLower() && c.IsActive);

        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<bool> ExisteNombreConParentAsync(string nombre, int? parentId, int? excludeId = null)
    {
        var query = _context.Categorias
            .Where(c => c.Nombre.ToLower() == nombre.ToLower() 
                   && c.IsActive
                   && c.ParentId == parentId);

        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<Categoria> CrearAsync(Categoria entidad)
    {
        var usuarioId = _currentUser.GetUsuarioId() ?? 1;
        entidad.CreatedByUsuarioId = usuarioId;
        entidad.CreatedAt = DateTime.UtcNow;
        entidad.IsActive = true;

        _context.Categorias.Add(entidad);
        await _context.SaveChangesAsync();
        return entidad;
    }

    public async Task<Categoria> ActualizarAsync(Categoria entidad)
    {
        var usuarioId = _currentUser.GetUsuarioId();
        if (usuarioId.HasValue)
        {
            entidad.UpdatedByUsuarioId = usuarioId;
            entidad.UpdatedAt = DateTime.UtcNow;
        }

        _context.Categorias.Update(entidad);
        await _context.SaveChangesAsync();
        return entidad;
    }

    public async Task EliminarAsync(int id)
    {
        var entidad = await _context.Categorias.FindAsync(id);
        if (entidad != null)
        {
            var usuarioId = _currentUser.GetUsuarioId();
            if (usuarioId.HasValue)
            {
                entidad.DeletedByUsuarioId = usuarioId;
                entidad.DeletedAt = DateTime.UtcNow;
                entidad.IsActive = false;
                _context.Categorias.Update(entidad);
            }
            else
            {
                _context.Categorias.Remove(entidad);
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Categoria?> ObtenerPorNombreYParentAsync(string nombre, int? parentId)
    {
        return await _context.Categorias
            .FirstOrDefaultAsync(c => c.Nombre.ToLower() == nombre.ToLower() 
                                   && c.ParentId == parentId);
    }

    public async Task<Categoria> ReactivarAsync(int id)
    {
        var entidad = await _context.Categorias.FindAsync(id);
        if (entidad == null)
            throw new Exception("Categoría no encontrada");

        var usuarioId = _currentUser.GetUsuarioId() ?? 1;
        entidad.UpdatedByUsuarioId = usuarioId;
        entidad.UpdatedAt = DateTime.UtcNow;
        entidad.DeletedAt = null;
        entidad.DeletedByUsuarioId = null;
        entidad.IsActive = true;

        _context.Categorias.Update(entidad);
        await _context.SaveChangesAsync();
        return entidad;
    }
}
