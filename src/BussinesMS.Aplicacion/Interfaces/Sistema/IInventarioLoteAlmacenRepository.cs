using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IInventarioLoteAlmacenRepository : IRepositorio<InventarioLoteAlmacen>
{
    Task<InventarioLoteAlmacen?> ObtenerConDetallesAsync(int id);
    Task<List<InventarioLoteAlmacen>> ObtenerLotesFEFOAsync(int varianteId, int almacenId);
    Task<int> ObtenerStockDisponibleAsync(int varianteId, int almacenId);
    Task<int> ObtenerStockTotalVarianteAsync(int varianteId, int almacenId);
    Task<InventarioLoteAlmacen?> ObtenerPorLoteYAlmacenAsync(int loteId, int almacenId);
    Task<InventarioLoteAlmacen> CrearSinGuardarAsync(InventarioLoteAlmacen entidad);
}
