using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.DTOs.Plantillas;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IProveedorService
{
    Task<PagedResultDto<ProveedorDto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
    Task<ProveedorDto?> ObtenerPorIdAsync(int id);
    Task<ProveedorDto> CrearAsync(CrearProveedorDto dto);
    Task<ProveedorDto> ActualizarAsync(ActualizarProveedorDto dto);
    Task EliminarAsync(int id);
}
