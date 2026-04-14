using BussinesMS.Aplicacion.DTOs.Auth;
using BussinesMS.Aplicacion.DTOs.Plantillas;

namespace BussinesMS.Aplicacion.Interfaces.Auth;

public interface IAlmacenService
{
    Task<PagedResultDto<AlmacenDto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
    Task<AlmacenDto?> ObtenerPorIdAsync(int id);
    Task<AlmacenDto> CrearAsync(CrearAlmacenDto almacen);
}