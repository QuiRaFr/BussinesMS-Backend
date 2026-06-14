using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IInventarioLoteService
{
    Task<PagedResultDto<InventarioLoteDto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
    Task<InventarioLoteDto?> ObtenerPorIdAsync(int id);
    Task<InventarioLoteDto> CrearAsync(CrearInventarioLoteDto dto);
    Task<InventarioLoteDto> ActualizarAsync(ActualizarInventarioLoteDto dto);
    Task EliminarAsync(int id);
    Task<List<InventarioLoteDto>> ObtenerLotesFEFOAsync(int varianteId, int almacenId);
    Task<int> ObtenerStockDisponibleAsync(int varianteId, int almacenId);
    Task<InventarioLoteDto> AjustarStockAsync(int id, AjusteInventarioDto dto);
}
