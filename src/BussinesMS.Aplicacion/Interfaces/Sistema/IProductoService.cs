using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.DTOs.Plantillas;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IProductoService
{
    Task<PagedResultDto<ProductoDto>> ObtenerTodosAsync(ProductoPaginationQueryDto query);
    Task<ProductoDto?> ObtenerPorIdAsync(int id);
    Task<ProductoDto> CrearAsync(CrearProductoDto dto);
    Task<ProductoDto> ActualizarAsync(ActualizarProductoDto dto);
    Task EliminarAsync(int id);
}