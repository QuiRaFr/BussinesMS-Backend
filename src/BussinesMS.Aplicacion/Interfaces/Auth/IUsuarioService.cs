using BussinesMS.Aplicacion.DTOs.Auth;
using BussinesMS.Aplicacion.DTOs.Plantillas;

namespace BussinesMS.Aplicacion.Interfaces.Auth;

public interface IUsuarioService
{
    Task<PagedResultDto<UsuarioDto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
    Task<UsuarioDto?> ObtenerPorIdAsync(int id);
    Task<UsuarioDto> CrearAsync(CrearUsuarioDto usuario);
    Task<List<MenuPermisoDto>> ObtenerMenusAsync(int id);
    Task<UsuarioDto> ActualizarMenusAsync(int id, List<MenuPermisoSimpleDto> menus);
    Task<(UsuarioDto? Usuario, string? Token, string? RolNombre, List<MenuPermisoDto> Menus)> ValidarLoginAsync(string username, string password);
}