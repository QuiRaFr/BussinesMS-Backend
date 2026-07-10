using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IPagoCompraRepository : IRepositorio<PagoCompra>
{
    Task<PagoCompra> CrearSinGuardarAsync(PagoCompra entidad);
}
