using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface ICompraService
{
    Task<PagedResultDto<CompraDto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
    Task<CompraDto?> ObtenerPorIdAsync(int id);
    Task<CompraDto> CrearAsync(CrearCompraDto dto);
    Task<CompraDto> ActualizarAsync(ActualizarCompraDto dto);
    Task EliminarAsync(int id);
    Task<PagoCompraDto> AgregarPagoAsync(int compraId, CrearPagoCompraDto dto);
}
