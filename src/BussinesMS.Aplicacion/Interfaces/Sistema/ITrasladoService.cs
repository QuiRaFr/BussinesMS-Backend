using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface ITrasladoService
{
    Task<PagedResultDto<TrasladoDto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
    Task<TrasladoDto?> ObtenerPorIdAsync(int id);
    Task<TrasladoDto> CrearAsync(CrearTrasladoDto dto);
}
