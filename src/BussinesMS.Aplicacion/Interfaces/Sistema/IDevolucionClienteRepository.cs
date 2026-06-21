using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IDevolucionClienteRepository
{
    IQueryable<DevolucionCliente> AsQueryable();
    Task<DevolucionCliente?> ObtenerPorIdAsync(int id);
    Task<DevolucionCliente?> ObtenerConDetallesAsync(int id);
    Task<DevolucionCliente> CrearAsync(DevolucionCliente entidad);
    Task<DevolucionCliente> ActualizarAsync(DevolucionCliente entidad);
}
