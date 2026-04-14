using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Auth;

namespace BussinesMS.Aplicacion.Interfaces.Auth;

public interface IUsuarioRepository : IRepositorio<Usuario>
{
    Task<Usuario?> ObtenerPorUsernameAsync(string username);
    Task<Usuario?> ValidarCredencialesAsync(string username, string password);
    Task<Usuario?> ObtenerConRolAsync(int id);
    Task<Usuario?> ObtenerConRolAsyncPorUsername(string username);
}