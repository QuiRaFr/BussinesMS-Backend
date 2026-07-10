using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface ICategoriaRepository : IRepositorio<Categoria>
{
    Task<List<CategoriaDto>> ObtenerTodosDtoAsync();
    Task<Categoria?> ObtenerPorNombreAsync(string nombre);
    Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null);
    Task<Categoria> ReactivarAsync(int id);
}
