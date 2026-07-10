using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IFabricanteRepository : IRepositorio<Fabricante>
{
    Task<Fabricante?> ObtenerPorNombreAsync(string nombre);
    Task<Fabricante> ReactivarAsync(int id);
}
