using BussinesMS.Aplicacion.DTOs.Auth;
using BussinesMS.Aplicacion.DTOs.Plantillas;

namespace BussinesMS.Aplicacion.Interfaces.Auth;

public interface IMenuService
{
    Task<PagedResultDto<MenuDto>> ObtenerTodosAsync(GenericPaginationQueryDto query, int? sistemaId = null);
    Task<List<MenuDto>> ObtenerActivosAsync(int? sistemaId = null);
    Task<MenuDto?> ObtenerPorIdAsync(int id);
    Task<MenuDto> CrearAsync(CrearMenuDto menu);
    Task<MenuDto> ActualizarAsync(MenuDto menu);
    Task EliminarAsync(int id);
}