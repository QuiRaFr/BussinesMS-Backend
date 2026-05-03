using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IDescripcionTamanioService
{
    Task<PagedResultDto<DescripcionTamanioDto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
    Task<DescripcionTamanioDto?> ObtenerPorIdAsync(int id);
    Task<DescripcionTamanioDto> CrearAsync(CrearDescripcionTamanioDto dto);
    Task<DescripcionTamanioDto> ActualizarAsync(ActualizarDescripcionTamanioDto dto);
    Task EliminarAsync(int id);
}