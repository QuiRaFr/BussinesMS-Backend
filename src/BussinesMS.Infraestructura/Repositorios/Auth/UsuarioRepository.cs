using BussinesMS.Aplicacion.Interfaces.Auth;
using BussinesMS.Aplicacion.Seguridad;
using BussinesMS.Dominio.Entidades.Auth;
using BussinesMS.Infraestructura.Persistencia;
using BussinesMS.Infraestructura.Repositorios.Compartido;
using Microsoft.EntityFrameworkCore;

namespace BussinesMS.Infraestructura.Repositorios.Auth;

public class UsuarioRepository : RepositorioBase<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(AuthDbContext contexto, ICurrentUserService currentUser) : base(contexto, currentUser)
    {
    }

    public async Task<Usuario?> ObtenerPorUsernameAsync(string username)
    {
        return await _dbSet.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<Usuario?> ValidarCredencialesAsync(string username, string password)
    {
        return await ObtenerPorUsernameAsync(username);
    }

    public async Task<Usuario?> ObtenerConRolAsync(int id)
    {
        return await _dbSet.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuario?> ObtenerConRolAsyncPorUsername(string username)
    {
        return await _dbSet.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Username == username);
    }
}