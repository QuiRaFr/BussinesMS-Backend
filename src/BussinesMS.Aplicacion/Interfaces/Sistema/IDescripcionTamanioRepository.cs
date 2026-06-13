using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IDescripcionTamanioRepository : IRepositorio<DescripcionTamanio>
{
    Task<DescripcionTamanio?> ObtenerPorNombreAsync(string nombre);
    Task<DescripcionTamanio> ReactivarAsync(int id);
}
