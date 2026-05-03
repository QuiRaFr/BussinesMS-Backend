using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IProductoRepository : IRepositorio<Producto>
{
    Task<Producto?> ObtenerConDetallesAsync(int id);
    Task<List<Producto>> ObtenerActivosAsync();
    Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null);
}