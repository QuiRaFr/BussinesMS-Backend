using AutoMapper;
using BussinesMS.Aplicacion.Comun;
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

    public async Task<UsuarioDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var usuario = await _repositorio.ObtenerConRolAsync(id);
            return usuario == null ? null : _mapper.Map<UsuarioDto>(usuario);
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
            
            var usuarioConRol = await _repositorio.ObtenerConRolAsync(dto.RolId);
            if (usuarioConRol != null)
            {
                if (!string.IsNullOrEmpty(usuarioConRol.Rol?.MenuIds))
                {
                    var menuIds = JsonSerializer.Deserialize<List<int>>(usuarioConRol.Rol.MenuIds) ?? new List<int>();
                    
                    var menusDelSistema = await _menuRepositorio.ObtenerActivosAsync(dto.SistemaIdDefault);
                    var menuIdsDelSistema = menusDelSistema.Select(m => m.Id).ToHashSet();
                    
                    var menuIdsValidos = menuIds.Where(id => menuIdsDelSistema.Contains(id)).ToList();
                    
                    var menus = menuIdsValidos.Select(mid => new MenuPermisoSimpleDto 
                    { 
                        MenuId = mid, 
                        Leer = true, 
                        Crear = true, 
                        Editar = true, 
                        Eliminar = true 
                    }).ToList();
                    usuario.Menus = JsonSerializer.Serialize(menus);
                }
            }
            
            var resultado = await _repositorio.CrearAsync(usuario);
            return _mapper.Map<UsuarioDto>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            throw;
        }
    }

    public async Task<List<MenuPermisoDto>> ObtenerMenusAsync(int id)
    {
        try
        {
            var usuario = await _repositorio.ObtenerConRolAsync(id);
            if (usuario == null)
                return new List<MenuPermisoDto>();
            
            var menus = new List<MenuPermisoDto>();
            var menuIdsAIncluir = new List<int>();
            
            if (!string.IsNullOrEmpty(usuario.Menus))
            {
                var menusUsuario = JsonSerializer.Deserialize<List<MenuPermisoSimpleDto>>(usuario.Menus) ?? new List<MenuPermisoSimpleDto>();
                menuIdsAIncluir = menusUsuario.Select(m => m.MenuId).ToList();
                menus = menusUsuario.Select(m => new MenuPermisoDto
                {
                    MenuId = m.MenuId,
                    Leer = m.Leer,
                    Crear = m.Crear,
                    Editar = m.Editar,
                    Eliminar = m.Eliminar
                }).ToList();
            }
            else if (!string.IsNullOrEmpty(usuario.Rol?.MenuIds))
            {
                var menuIds = JsonSerializer.Deserialize<List<int>>(usuario.Rol.MenuIds) ?? new List<int>();
                menuIdsAIncluir = menuIds;
                menus = menuIds.Select(mid => new MenuPermisoDto
                {
                    MenuId = mid,
                    Leer = true,
                    Crear = true,
                    Editar = true,
                    Eliminar = true
                }).ToList();
            }
            
            if (menuIdsAIncluir.Any())
            {
                var todosMenus = await _menuRepositorio.ObtenerTodosAsync(usuario.SistemaIdDefault);
                var menuDict = todosMenus.ToDictionary(m => m.Id);
                
                foreach (var menu in menus)
                {
                    if (menuDict.TryGetValue(menu.MenuId, out var menuEntidad))
                    {
                        menu.Nombre = menuEntidad.Nombre;
                        menu.Url = menuEntidad.Url;
                        menu.Icono = menuEntidad.Icono;
                        menu.JerarquiaName = menuEntidad.JerarquiaName;
                        menu.SistemaId = menuEntidad.SistemaId ?? 0;
                    }
                }
            }
            
            return menus;
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
            
            usuario.Menus = JsonSerializer.Serialize(menusValidos);
            await _repositorio.ActualizarAsync(usuario);
            
            return _mapper.Map<UsuarioDto>(usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar menús del usuario {Id}", id);
            throw;
        }
    }

    public async Task<(UsuarioDto? Usuario, string? Token, string? RolNombre, List<MenuPermisoDto> Menus)> ValidarLoginAsync(string username, string password)
    {
        try
        {
            var usuario = await _repositorio.ObtenerConRolAsyncPorUsername(username);
            if (usuario == null) return (null, null, null, new List<MenuPermisoDto>());
            
            if (VerificarPassword(password, usuario.PasswordHash))
            {
                var token = _jwtHelper.GenerateToken( usuario.Id, usuario.Username, usuario.RolId);
                
                var rolNombre = usuario.Rol?.Nombre;
                
                var menus = new List<MenuPermisoDto>();
                var menuIdsAIncluir = new List<int>();
                
                if (!string.IsNullOrEmpty(usuario.Menus))
                {
                    var menusUsuario = JsonSerializer.Deserialize<List<MenuPermisoSimpleDto>>(usuario.Menus) ?? new List<MenuPermisoSimpleDto>();
                    menuIdsAIncluir = menusUsuario.Select(m => m.MenuId).ToList();
                    menus = menusUsuario.Select(m => new MenuPermisoDto
                    {
                        MenuId = m.MenuId,
                        Leer = m.Leer,
                        Crear = m.Crear,
                        Editar = m.Editar,
                        Eliminar = m.Eliminar
                    }).ToList();
                }
                else if (!string.IsNullOrEmpty(usuario.Rol?.MenuIds))
                {
                    var menuIds = JsonSerializer.Deserialize<List<int>>(usuario.Rol.MenuIds) ?? new List<int>();
                    menuIdsAIncluir = menuIds;
                    menus = menuIds.Select(mid => new MenuPermisoDto
                    {
                        MenuId = mid,
                        Leer = true,
                        Crear = true,
                        Editar = true,
                        Eliminar = true
                    }).ToList();
                }
                
                if (menuIdsAIncluir.Any())
                {
                    var todosMenus = await _menuRepositorio.ObtenerTodosAsync(usuario.SistemaIdDefault);
                    var menuDict = todosMenus.ToDictionary(m => m.Id);
                    
                    foreach (var menu in menus)
                    {
                        if (menuDict.TryGetValue(menu.MenuId, out var menuEntidad))
                        {
                            menu.Nombre = menuEntidad.Nombre;
                            menu.Url = menuEntidad.Url;
                            menu.Icono = menuEntidad.Icono;
                            menu.JerarquiaName = menuEntidad.JerarquiaName;
                            menu.SistemaId = menuEntidad.SistemaId ?? 0;
                        }
                    }
                }
                
                return (_mapper.Map<UsuarioDto>(usuario), token, rolNombre, menus);
            }
            return (null, null, null, new List<MenuPermisoDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar login para usuario {Username}", username);
            throw;
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerificarPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}