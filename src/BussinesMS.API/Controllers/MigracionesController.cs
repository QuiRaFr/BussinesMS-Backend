using BussinesMS.Aplicacion.DTOs.Sistema.Migracion;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using Microsoft.AspNetCore.Mvc;

namespace BussinesMS.API.Controllers;

[ApiController]
[Route("api/Sistema/[controller]")]
[Produces("application/json")]
public class MigracionesController : BaseController
{
    private readonly IMigracionService _servicio;

    public MigracionesController(IMigracionService servicio)
    {
        _servicio = servicio;
    }

    [HttpPost]
    public async Task<IActionResult> MigrarDatos(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return RespuestaError("No se ha proporcionado ningún archivo");

        var resultado = await _servicio.MigrarDatosDesdeCsvAsync(archivo);
        return RespuestaOk(resultado);
    }
}