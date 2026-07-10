using BussinesMS.Aplicacion.DTOs.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface ITipoPresentacionService
{
    Task<List<TipoPresentacionDto>> ObtenerTodosAsync();
    Task<TipoPresentacionDto?> ObtenerPorIdAsync(int id);
    Task<TipoPresentacionDto> CrearAsync(CrearTipoPresentacionDto dto);
    Task<TipoPresentacionDto> ActualizarAsync(ActualizarTipoPresentacionDto dto);
    Task EliminarAsync(int id);
}