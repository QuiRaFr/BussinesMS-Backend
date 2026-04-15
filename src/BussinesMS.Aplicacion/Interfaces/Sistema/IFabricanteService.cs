using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IFabricanteService
{
    Task<PagedResultDto<FabricanteDto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
    Task<FabricanteDto?> ObtenerPorIdAsync(int id);
    Task<FabricanteDto> CrearAsync(CrearFabricanteDto dto);
    Task<FabricanteDto> ActualizarAsync(ActualizarFabricanteDto dto);
    Task EliminarAsync(int id);
}