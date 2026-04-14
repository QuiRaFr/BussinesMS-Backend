using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface ICategoriaService
{
    Task<PagedResultDto<CategoriaDto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
    Task<CategoriaDto?> ObtenerPorIdAsync(int id);
    Task<List<CategoriaDto>> ObtenerRaicesAsync();
    Task<List<CategoriaDto>> ObtenerSubcategoriasAsync(int parentId);
    Task<CategoriaDto> CrearAsync(CrearCategoriaDto dto);
    Task<CategoriaDto> ActualizarAsync(ActualizarCategoriaDto dto);
    Task EliminarAsync(int id);
}
