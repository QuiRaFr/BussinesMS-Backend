using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface ICategoriaService
{
    Task<PagedResultDto<CategoriaDto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
    Task<CategoriaDto?> ObtenerPorIdAsync(int id);
    Task<(CategoriaDto Categoria, bool FueReactivada)> CrearAsync(CrearCategoriaDto dto);
    Task<CategoriaDto> ActualizarAsync(ActualizarCategoriaDto dto);
    Task EliminarAsync(int id);
}
