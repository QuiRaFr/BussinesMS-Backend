using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IMovimientoInventarioService
{
    Task<PagedResultDto<MovimientoInventarioDto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
    Task<PagedResultDto<MovimientoInventarioDto>> ObtenerFiltradosAsync(MovimientoInventarioFiltroDto filtro, GenericPaginationQueryDto query);
    Task<MovimientoInventarioDto?> ObtenerPorIdAsync(int id);
}
