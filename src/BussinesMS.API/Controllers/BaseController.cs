using Microsoft.AspNetCore.Mvc;

namespace BussinesMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult RespuestaOk<T>(T datos, string mensaje = "Operación exitosa")
    {
        return Ok(new
        {
            Success = true,
            Message = mensaje,
            Data = datos
        });
    }

    protected IActionResult RespuestaError(string mensaje, int statusCode = 400)
    {
        return StatusCode(statusCode, new
        {
            Success = false,
            Message = mensaje
        });
    }
}