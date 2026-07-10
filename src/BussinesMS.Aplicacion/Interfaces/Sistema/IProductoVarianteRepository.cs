using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IProductoVarianteRepository : IRepositorio<ProductoVariante>
{
    Task<ProductoVariante?> ObtenerConDetallesAsync(int id);
    Task<List<ProductoVariante>> ObtenerActivosAsync();
    Task<bool> ExisteCombinacionAsync(int productoId, int saborId, int tamanioId, int? excludeId = null);
    Task<bool> ExisteCodigoBarrasAsync(string? codigoBarras, int? excludeId = null);
    Task<ProductoVariante?> ObtenerPorCodigoBarrasAsync(string codigoBarras);
}