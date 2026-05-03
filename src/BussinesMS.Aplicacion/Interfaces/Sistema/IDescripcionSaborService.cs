using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IDescripcionSaborService
{
    Task<PagedResultDto<DescripcionSaborDto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
    Task<DescripcionSaborDto?> ObtenerPorIdAsync(int id);
    Task<DescripcionSaborDto> CrearAsync(CrearDescripcionSaborDto dto);
    Task<DescripcionSaborDto> ActualizarAsync(ActualizarDescripcionSaborDto dto);
    Task EliminarAsync(int id);
}