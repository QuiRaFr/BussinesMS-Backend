using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using Microsoft.AspNetCore.Mvc;

namespace BussinesMS.API.Controllers;

[ApiController]
[Route("api/Sistema/[controller]")]
[Produces("application/json")]
public class TrasladosController : BaseController
{
    private readonly ITrasladoService _servicio;

    public TrasladosController(ITrasladoService servicio)
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
        return resultado == null
            ? RespuestaError("Traslado no encontrado", 404)
            : RespuestaOk(resultado);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearTrasladoDto dto)
    {
        var resultado = await _servicio.CrearAsync(dto);
        return RespuestaCreado(resultado, "Traslado creado");
    }
}
