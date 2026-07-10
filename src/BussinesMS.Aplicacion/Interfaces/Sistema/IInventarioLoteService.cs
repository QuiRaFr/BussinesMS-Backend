using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IInventarioLoteService
{
    Task<PagedResultDto<InventarioLoteAlmacenDto>> ObtenerTodosAsync(GenericPaginationQueryDto query, int? categoriaId = null, int? almacenId = null);
    Task<InventarioLoteAlmacenDto?> ObtenerPorIdAsync(int id);
    Task<InventarioLoteAlmacenDto> CrearAsync(CrearInventarioLoteAlmacenDto dto);
    Task<InventarioLoteAlmacenDto> ActualizarAsync(ActualizarInventarioLoteAlmacenDto dto);
    Task EliminarAsync(int id);
    Task<InventarioLoteAlmacenDto> AjustarStockAsync(int id, AjusteInventarioDto dto);
}
