using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.DTOs.Plantillas;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IProductoVarianteService
{
    Task<PagedResultDto<ProductoVarianteDto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
    Task<ProductoVarianteDto?> ObtenerPorIdAsync(int id);
    Task<ProductoVarianteDto?> ObtenerPorCodigoBarrasAsync(string codigoBarras);
    Task<CompraInfoVarianteDto?> ObtenerCompraInfoAsync(int id);
    Task<ProductoVarianteDto> CrearAsync(CrearProductoVarianteDto dto);
    Task<ProductoVarianteDto> ActualizarAsync(ActualizarProductoVarianteDto dto);
    Task EliminarAsync(int id);
}