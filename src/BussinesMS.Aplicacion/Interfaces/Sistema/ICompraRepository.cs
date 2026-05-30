using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface ICompraRepository : IRepositorio<Compra>
{
    Task<Compra?> ObtenerConDetallesAsync(int id);
    Task<List<Compra>> ObtenerActivasAsync();
}
