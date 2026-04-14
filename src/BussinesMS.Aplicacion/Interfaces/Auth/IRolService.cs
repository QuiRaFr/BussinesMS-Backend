using BussinesMS.Aplicacion.DTOs.Auth;
using BussinesMS.Aplicacion.DTOs.Plantillas;

namespace BussinesMS.Aplicacion.Interfaces.Auth;

public interface IRolService
{
    Task<PagedResultDto<RolDto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
    Task<RolDto?> ObtenerPorIdAsync(int id);
    Task<RolDto> CrearAsync(CrearRolDto rol);
    Task<RolDto> ActualizarMenuIdsAsync(int id, List<int> menuIds);
}