using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using Microsoft.AspNetCore.Mvc;

namespace BussinesMS.API.Controllers;

[ApiController]
[Route("api/Sistema/[controller]")]
[Produces("application/json")]
public class FabricantesController : BaseController
{
    private readonly IFabricanteService _servicio;

    public FabricantesController(IFabricanteService servicio)
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
        return resultado == null ? RespuestaError("Fabricante no encontrado", 404) : RespuestaOk(resultado);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearFabricanteDto dto)
    {
        var resultado = await _servicio.CrearAsync(dto);
        return StatusCode(201, new
        {
            Success = true,
            Message = "Fabricante creado",
            Data = resultado
        });
    }

    [HttpPut]
    public async Task<IActionResult> Actualizar([FromBody] ActualizarFabricanteDto dto)
    {
        var resultado = await _servicio.ActualizarAsync(dto);
        return RespuestaOk(resultado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return RespuestaOk(new { mensaje = "Fabricante eliminado" });
    }
}