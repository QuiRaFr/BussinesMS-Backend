using BussinesMS.Aplicacion.DTOs.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IDevolucionClienteService
{
    Task<DevolucionClienteDto> CrearAsync(CrearDevolucionClienteDto dto);
    Task RealizarCambioAsync(RealizarCambioDto dto);
    Task DarDeBajaAsync(DarDeBajaDevolucionDto dto);
    Task<DevolucionClienteDto?> ObtenerPorIdAsync(int id);
}
