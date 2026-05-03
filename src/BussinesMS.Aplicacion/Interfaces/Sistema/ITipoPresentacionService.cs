using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface ITipoPresentacionService
{
    Task<PagedResultDto<TipoPresentacionDto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
    Task<TipoPresentacionDto?> ObtenerPorIdAsync(int id);
    Task<TipoPresentacionDto> CrearAsync(CrearTipoPresentacionDto dto);
    Task<TipoPresentacionDto> ActualizarAsync(ActualizarTipoPresentacionDto dto);
    Task EliminarAsync(int id);
}