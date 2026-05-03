using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface ICategoriaRepository : IRepositorio<Categoria>
{
    Task<List<CategoriaDto>> ObtenerTodosDtoAsync();
    Task<List<Categoria>> ObtenerRaicesAsync();
    Task<List<Categoria>> ObtenerSubcategoriasAsync(int parentId);
    Task<Categoria?> ObtenerPorNombreAsync(string nombre);
    Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null);
    Task<bool> ExisteNombreConParentAsync(string nombre, int? parentId, int? excludeId = null);
    Task<Categoria?> ObtenerPorNombreYParentAsync(string nombre, int? parentId);
    Task<Categoria> ReactivarAsync(int id);
}
