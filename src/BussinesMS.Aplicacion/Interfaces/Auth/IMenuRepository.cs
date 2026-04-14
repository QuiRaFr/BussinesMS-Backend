using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Auth;

namespace BussinesMS.Aplicacion.Interfaces.Auth;

public interface IMenuRepository : IRepositorio<Menu>
{
    Task<List<Menu>> ObtenerTodosAsync(int? sistemaId = null);
    Task<List<Menu>> ObtenerActivosAsync(int? sistemaId = null);
}