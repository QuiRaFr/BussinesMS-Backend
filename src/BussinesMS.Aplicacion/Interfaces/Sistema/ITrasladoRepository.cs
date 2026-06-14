using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface ITrasladoRepository : IRepositorio<Traslado>
{
    Task<Traslado?> ObtenerConDetallesAsync(int id);
}
