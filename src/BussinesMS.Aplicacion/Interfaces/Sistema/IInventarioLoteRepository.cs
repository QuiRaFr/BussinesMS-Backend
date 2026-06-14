using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IInventarioLoteRepository : IRepositorio<InventarioLote>
{
    Task<InventarioLote?> ObtenerConDetallesAsync(int id);
    Task<List<InventarioLote>> ObtenerLotesFEFOAsync(int varianteId, int almacenId);
    Task<int> ObtenerStockDisponibleAsync(int varianteId, int almacenId);
}
