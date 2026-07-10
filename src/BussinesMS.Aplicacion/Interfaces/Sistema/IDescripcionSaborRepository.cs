using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IDescripcionSaborRepository : IRepositorio<DescripcionSabor>
{
    Task<DescripcionSabor?> ObtenerPorNombreAsync(string nombre);
    Task<DescripcionSabor> ReactivarAsync(int id);
}
