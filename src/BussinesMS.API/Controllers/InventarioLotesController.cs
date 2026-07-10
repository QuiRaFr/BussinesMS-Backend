using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using Microsoft.AspNetCore.Mvc;

namespace BussinesMS.API.Controllers;

[ApiController]
[Route("api/Sistema/[controller]")]
[Produces("application/json")]
public class InventarioLotesController : BaseController
{
    private readonly IInventarioLoteService _servicio;

    public InventarioLotesController(IInventarioLoteService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] GenericPaginationQueryDto query, [FromQuery] int? categoriaId, [FromQuery] int? almacenId)
    {
        var resultado = await _servicio.ObtenerTodosAsync(query, categoriaId, almacenId);
        return RespuestaOk(resultado);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var resultado = await _servicio.ObtenerPorIdAsync(id);
        return resultado == null
            ? RespuestaError("Lote de inventario no encontrado", 404)
            : RespuestaOk(resultado);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearInventarioLoteAlmacenDto dto)
    {
        var resultado = await _servicio.CrearAsync(dto);
        return RespuestaCreado(resultado, "Lote de inventario creado");
    }

    [HttpPut]
    public async Task<IActionResult> Actualizar([FromBody] ActualizarInventarioLoteAlmacenDto dto)
    {
        var resultado = await _servicio.ActualizarAsync(dto);
        return RespuestaOk(resultado, "Lote de inventario actualizado");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return RespuestaOk(new { mensaje = "Lote de inventario eliminado" });
    }

    [HttpPost("{id}/ajuste")]
    public async Task<IActionResult> AjustarStock(int id, [FromBody] AjusteInventarioDto dto)
    {
        var resultado = await _servicio.AjustarStockAsync(id, dto);
        return RespuestaOk(resultado, "Stock ajustado exitosamente");
    }
}
