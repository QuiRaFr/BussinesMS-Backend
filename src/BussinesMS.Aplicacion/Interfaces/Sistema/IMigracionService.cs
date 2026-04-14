using BussinesMS.Aplicacion.DTOs.Sistema.Migracion;
using Microsoft.AspNetCore.Http;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IMigracionService
{
    Task<ResultadoMigracionDto> CargarCategoriasDesdeCsvAsync(IFormFile archivo);
}