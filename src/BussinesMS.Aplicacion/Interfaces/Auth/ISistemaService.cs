using BussinesMS.Aplicacion.DTOs.Auth;
using BussinesMS.Aplicacion.DTOs.Plantillas;

namespace BussinesMS.Aplicacion.Interfaces.Auth;

public interface ISistemaService
{
    Task<PagedResultDto<SistemaDto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
    Task<SistemaDto?> ObtenerPorIdAsync(int id);
    Task<SistemaDto> CrearAsync(CrearSistemaDto sistema);
}