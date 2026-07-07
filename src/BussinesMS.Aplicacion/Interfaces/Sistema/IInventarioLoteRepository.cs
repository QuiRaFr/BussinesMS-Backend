using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IInventarioLoteRepository : IRepositorio<InventarioLote>
{
    Task<InventarioLote?> ObtenerConDetallesAsync(int id);
    Task<InventarioLote> CrearSinGuardarAsync(InventarioLote entidad);
}
