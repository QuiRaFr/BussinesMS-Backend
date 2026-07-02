using AutoMapper;
using BussinesMS.Aplicacion.Comun;
using BussinesMS.Aplicacion.Common;
using BussinesMS.Aplicacion.DTOs.Auth;
using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.Interfaces.Auth;
using BussinesMS.Aplicacion.Seguridad;
using BussinesMS.Dominio.Entidades.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BussinesMS.Aplicacion.Servicios.Auth;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repositorio;
    private readonly IMenuRepository _menuRepositorio;
    private readonly IMapper _mapper;
    private readonly ILogger<UsuarioService> _logger;
    private readonly JwtHelper _jwtHelper;

    public UsuarioService(
        IUsuarioRepository repositorio,
        IMenuRepository menuRepositorio,
        IMapper mapper,
        ILogger<UsuarioService> logger,
        JwtHelper jwtHelper)
    {
        _repositorio = repositorio;
        _menuRepositorio = menuRepositorio;
        _mapper = mapper;
        _logger = logger;
        _jwtHelper = jwtHelper;
    }

    public async Task<PagedResultDto<UsuarioDto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
    {
        try
        {
            var baseQuery = _repositorio.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var filterLower = query.Filter.ToLower();
                baseQuery = baseQuery.Where(u => u.Username!.ToLower().Contains(filterLower) ||
                                                 u.Nombre!.ToLower().Contains(filterLower));
            }

            (var filteredQuery, var totalCount) = baseQuery.ApplyFilters(query);

            var usuarios = await filteredQuery.ToListAsync();
            var dtos = _mapper.Map<List<UsuarioDto>>(usuarios);

            return new PagedResultDto<UsuarioDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios");
            throw;
        }
    }

    public async Task<UsuarioConMenusDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var usuario = await _repositorio.ObtenerConRolAsync(id);
            if (usuario == null) return null;

            var menus = await ConstruirArbolMenusAsync(usuario.Id, usuario.SistemaIdDefault);

            return new UsuarioConMenusDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Username = usuario.Username,
                SistemaIdDefault = usuario.SistemaIdDefault,
                RolId = usuario.RolId,
                RolNombre = usuario.Rol?.Nombre,
                IsActive = usuario.IsActive,
                CreatedAt = BoliviaTimeZone.ToLocal(usuario.CreatedAt),
                Menus = menus
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario {Id}", id);
            throw;
        }
    }

    public async Task<UsuarioDto> CrearAsync(CrearUsuarioDto dto)
    {
        try
        {
            var usuario = _mapper.Map<Usuario>(dto);
            usuario.PasswordHash = HashPassword(dto.Password);
            usuario.CreatedAt = DateTime.UtcNow;

            var resultado = await _repositorio.CrearAsync(usuario);

            List<MenuPermisoSimpleDto> menusAsignar;

            if (dto.Menus != null && dto.Menus.Count > 0)
            {
                menusAsignar = dto.Menus;
            }
            else
            {
                var rol = await _repositorio.ObtenerConRolAsync(resultado.Id);
                if (rol?.Rol != null && !string.IsNullOrEmpty(rol.Rol.MenuIds))
                {
                    var menuIds = JsonSerializer.Deserialize<List<int>>(rol.Rol.MenuIds) ?? new List<int>();
                    menusAsignar = menuIds.Select(menuId => new MenuPermisoSimpleDto
                    {
                        MenuId = menuId,
                        Leer = true,
                        Crear = true,
                        Editar = true,
                        Eliminar = true
                    }).ToList();
                }
                else
                {
                    menusAsignar = new List<MenuPermisoSimpleDto>();
                }
            }

            if (menusAsignar.Count > 0)
            {
                var menusEntidad = menusAsignar.Select(m => new UsuarioMenu
                {
                    UsuarioId = resultado.Id,
                    MenuId = m.MenuId,
                    Leer = m.Leer,
                    Crear = m.Crear,
                    Editar = m.Editar,
                    Eliminar = m.Eliminar,
                    PermisosEspeciales = m.PermisosEspeciales
                }).ToList();

                await _repositorio.AgregarMenusAsync(resultado.Id, menusEntidad);
            }

            return _mapper.Map<UsuarioDto>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            throw;
        }
    }

    public async Task<UsuarioDto> ActualizarAsync(int id, ActualizarUsuarioDto dto)
    {
        try
        {
            var usuario = await _repositorio.ObtenerPorIdAsync(id);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            usuario.Nombre = dto.Nombre;
            usuario.Apellido = dto.Apellido;
            usuario.Email = dto.Email;
            usuario.SistemaIdDefault = dto.SistemaIdDefault;
            usuario.UpdatedAt = DateTime.UtcNow;

            var resultado = await _repositorio.ActualizarAsync(usuario);
            return _mapper.Map<UsuarioDto>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario {Id}", id);
            throw;
        }
    }

    public async Task<List<MenuArbolDto>> ObtenerMenusAsync(int id)
    {
        try
        {
            var usuario = await _repositorio.ObtenerConRolAsync(id);
            if (usuario == null)
                return new List<MenuArbolDto>();

            return await ConstruirArbolMenusAsync(usuario.Id, usuario.SistemaIdDefault);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener menús del usuario {Id}", id);
            throw;
        }
    }

    public async Task<UsuarioDto> ActualizarMenusAsync(int id, List<MenuPermisoSimpleDto> menus)
    {
        try
        {
            var usuario = await _repositorio.ObtenerConRolAsync(id);
            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            var menusDelSistema = await _menuRepositorio.ObtenerActivosAsync(usuario.SistemaIdDefault);
            var menuIdsDelSistema = menusDelSistema.Select(m => m.Id).ToHashSet();

            var menusValidos = menus.Where(m => menuIdsDelSistema.Contains(m.MenuId)).ToList();

            if (menusValidos.Count != menus.Count)
            {
                var menusInvalidos = menus.Where(m => !menuIdsDelSistema.Contains(m.MenuId)).Select(m => m.MenuId).ToList();
                throw new Exception($"Los siguientes menús no pertenecen al sistema {usuario.SistemaIdDefault}: {string.Join(", ", menusInvalidos)}");
            }

            await _repositorio.EliminarMenusAsync(id);

            var usuarioMenus = menusValidos.Select(m => new UsuarioMenu
            {
                UsuarioId = id,
                MenuId = m.MenuId,
                Leer = m.Leer,
                Crear = m.Crear,
                Editar = m.Editar,
                Eliminar = m.Eliminar,
                PermisosEspeciales = m.PermisosEspeciales
            }).ToList();

            await _repositorio.AgregarMenusAsync(id, usuarioMenus);

            return _mapper.Map<UsuarioDto>(usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar menús del usuario {Id}", id);
            throw;
        }
    }

    public async Task<LoginResponseDto?> ValidarLoginAsync(string username, string password)
    {
        try
        {
            var usuario = await _repositorio.ObtenerConRolAsyncPorUsername(username);
            if (usuario == null) return null;

            if (!VerificarPassword(password, usuario.PasswordHash))
                return null;

            var token = _jwtHelper.GenerateToken(usuario.Id, usuario.Username, usuario.RolId);
            var menus = await ConstruirArbolMenusAsync(usuario.Id, usuario.SistemaIdDefault);

            return new LoginResponseDto
            {
                Usuario = _mapper.Map<UsuarioDto>(usuario),
                Token = token,
                RolNombre = usuario.Rol?.Nombre,
                Menus = menus
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar login para usuario {Username}", username);
            throw;
        }
    }

    private async Task<List<MenuArbolDto>> ConstruirArbolMenusAsync(int usuarioId, int sistemaId)
    {
        var usuarioMenus = await _repositorio.ObtenerMenusAsync(usuarioId);
        var todosMenus = await _menuRepositorio.ObtenerActivosAsync(sistemaId);

        var menusAsignados = usuarioMenus.ToDictionary(um => um.MenuId);
        var todosMenusDict = todosMenus.ToDictionary(m => m.Id);

        var resultado = new List<MenuArbolDto>();

        foreach (var menu in todosMenus.Where(m => m.ParentId == null).OrderBy(m => m.Orden))
        {
            var nodo = ConstruirNodo(menu, todosMenus, menusAsignados, todosMenusDict);
            if (nodo != null)
                resultado.Add(nodo);
        }

        return resultado;
    }

    private MenuArbolDto? ConstruirNodo(
        Menu menu,
        List<Menu> todosMenus,
        Dictionary<int, UsuarioMenu> menusAsignados,
        Dictionary<int, Menu> todosMenusDict)
    {
        var esAsignado = menusAsignados.ContainsKey(menu.Id);

        var hijos = todosMenus
            .Where(m => m.ParentId == menu.Id)
            .OrderBy(m => m.Orden)
            .Select(hijo => ConstruirNodo(hijo, todosMenus, menusAsignados, todosMenusDict))
            .Where(h => h != null)
            .ToList();

        if (!esAsignado && hijos.Count == 0)
            return null;

        var usuarioMenu = esAsignado ? menusAsignados[menu.Id] : null;

        if (menu.IsGroup)
        {
            return new MenuArbolDto
            {
                MenuId = menu.Id,
                Nombre = menu.Nombre,
                Icono = menu.Icono,
                Orden = menu.Orden,
                IsGroup = true,
                SistemaNombre = menu.Sistema?.Nombre,
                SubMenus = hijos!
            };
        }

        var permisosEspeciales = usuarioMenu?.PermisosEspeciales != null
            ? JsonSerializer.Deserialize<string[]>(usuarioMenu.PermisosEspeciales) ?? null
            : null;

        return new MenuArbolDto
        {
            MenuId = menu.Id,
            Nombre = menu.Nombre,
            Url = menu.Url,
            Icono = menu.Icono,
            Orden = menu.Orden,
            SistemaNombre = menu.Sistema?.Nombre,
            Leer = usuarioMenu?.Leer ?? false,
            Crear = usuarioMenu?.Crear ?? false,
            Editar = usuarioMenu?.Editar ?? false,
            Eliminar = usuarioMenu?.Eliminar ?? false,
            Permisos = permisosEspeciales
        };
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerificarPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}
