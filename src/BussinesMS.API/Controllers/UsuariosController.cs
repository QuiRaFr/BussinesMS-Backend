using Microsoft.AspNetCore.Mvc;
using BussinesMS.Aplicacion.DTOs.Auth;
using BussinesMS.Aplicacion.Interfaces.Auth;
using BussinesMS.Aplicacion.DTOs.Plantillas;

namespace BussinesMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : BaseController
{
    private readonly IUsuarioService _servicio;

    public UsuariosController(IUsuarioService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] GenericPaginationQueryDto query)
    {
        var resultado = await _servicio.ObtenerTodosAsync(query);
        return RespuestaOk(resultado);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var resultado = await _servicio.ObtenerPorIdAsync(id);
        return resultado == null ? RespuestaError("Usuario no encontrado", 404) : RespuestaOk(resultado);
    }

    [HttpGet("{id}/menus")]
    public async Task<IActionResult> ObtenerMenus(int id)
    {
        var menus = await _servicio.ObtenerMenusAsync(id);
        return RespuestaOk(menus);
    }

    [HttpPut("{id}/menus")]
    public async Task<IActionResult> ActualizarMenus(int id, [FromBody] List<MenuPermisoSimpleDto> menus)
    {
        var resultado = await _servicio.ActualizarMenusAsync(id, menus);
        return RespuestaOk(resultado, "Menús actualizados");
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearUsuarioDto usuario)
    {
        var resultado = await _servicio.CrearAsync(usuario);
        return RespuestaOk(resultado, "Usuario creado");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        var (usuario, token, rolNombre, menus) = await _servicio.ValidarLoginAsync(login.Username, login.Password);
        if (usuario == null || token == null)
            return RespuestaError("Credenciales inválidas", 401);
        
        var respuesta = new LoginResponseDto 
        { 
            Usuario = usuario, 
            Token = token,
            RolNombre = rolNombre,
            Menus = menus
        };
        return RespuestaOk(respuesta, "Login exitoso");
    }
}