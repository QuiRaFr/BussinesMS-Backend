using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface ICategoriaRepository : IRepositorio<Categoria>
{
    Task<List<CategoriaDto>> ObtenerTodosDtoAsync();
    Task<List<CategoriaDto>> ObtenerRaicesAsync();
    Task<List<CategoriaDto>> ObtenerSubcategoriasAsync(int parentId);
}
