using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IProveedorRepository : IRepositorio<Proveedor>
{
    Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null);
}
