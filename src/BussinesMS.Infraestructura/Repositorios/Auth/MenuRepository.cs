using BussinesMS.Aplicacion.Interfaces.Auth;
using BussinesMS.Aplicacion.Seguridad;
using BussinesMS.Dominio.Entidades.Auth;
using BussinesMS.Infraestructura.Persistencia;
using BussinesMS.Infraestructura.Repositorios.Compartido;
using Microsoft.EntityFrameworkCore;

namespace BussinesMS.Infraestructura.Repositorios.Auth;

public class MenuRepository : RepositorioBase<Menu>, IMenuRepository
{
    public MenuRepository(AuthDbContext contexto, ICurrentUserService currentUser) : base(contexto, currentUser)
    {
    }

    public async Task<List<Menu>> ObtenerTodosAsync(int? sistemaId = null)
    {
        var query = _dbSet.AsQueryable();
        
        if (sistemaId.HasValue)
        {
            query = query.Where(m => m.SistemaId == sistemaId.Value);
        }
        
        return await query.ToListAsync();
    }

    public async Task<List<Menu>> ObtenerActivosAsync(int? sistemaId = null)
    {
        var query = _dbSet.Where(m => m.IsActive);
        
        if (sistemaId.HasValue)
        {
            query = query.Where(m => m.SistemaId == sistemaId.Value);
        }
        
        return await query.OrderBy(m => m.Orden).ToListAsync();
    }
}