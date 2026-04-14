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

    /// <summary>
    /// Obtiene categorías con filtros opcionales:
    /// FiltroTipo: 0 = Todos (defecto), 1 = Solo categorías raíz, 2 = Solo subcategorías
    /// </summary>
    /// <param name="query">Parámetros de paginación y filtro. FiltroTipo: 0=Todos, 1=Categoria, 2=Subcategoria</param>
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] GenericPaginationQueryDto query)
    {
        var resultado = await _servicio.ObtenerTodosAsync(query);
        return RespuestaOk(resultado);
    }

    [HttpGet("raices")]
    public async Task<IActionResult> ObtenerRaices()
    {
        var resultado = await _servicio.ObtenerRaicesAsync();
        return RespuestaOk(resultado);
    }

    [HttpGet("{id}/subcategorias")]
    public async Task<IActionResult> ObtenerSubcategorias(int id)
    {
        var resultado = await _servicio.ObtenerSubcategoriasAsync(id);
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
        var resultado = await _servicio.CrearAsync(dto);
        return StatusCode(201, new
        {
            Success = true,
            Message = "Categoría creada",
            Data = resultado
        });
    }

    [HttpPut]
    public async Task<IActionResult> Actualizar([FromBody] ActualizarCategoriaDto dto)
    {
        var resultado = await _servicio.ActualizarAsync(dto);
        return RespuestaOk(resultado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return RespuestaOk(new { mensaje = "Categoría eliminada" });
    }
}
