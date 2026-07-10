using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using Microsoft.AspNetCore.Mvc;

namespace BussinesMS.API.Controllers;

[ApiController]
[Route("api/Sistema/[controller]")]
[Produces("application/json")]
public class CategoriasController : BaseController
{
    private readonly ICategoriaService _servicio;

    public CategoriasController(ICategoriaService servicio)
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
        return resultado == null ? RespuestaError("Categoría no encontrada", 404) : RespuestaOk(resultado);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearCategoriaDto dto)
    {
        var (categoria, fueReactivada) = await _servicio.CrearAsync(dto);

        return fueReactivada
            ? RespuestaOk(new { mensaje = $"La categoría '{categoria.Nombre}' estaba desactivada y fue reactivada.", data = categoria })
            : RespuestaCreado(categoria, "Categoría creada exitosamente.");
    }

    [HttpPut]
    public async Task<IActionResult> Actualizar([FromBody] ActualizarCategoriaDto dto)
    {
        var resultado = await _servicio.ActualizarAsync(dto);
        return RespuestaOk(resultado, "Categoría actualizada exitosamente.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return RespuestaOk(new { mensaje = "Categoría eliminada" });
    }
}
