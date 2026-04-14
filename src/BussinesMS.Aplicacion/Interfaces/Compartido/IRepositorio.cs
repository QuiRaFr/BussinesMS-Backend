using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Aplicacion.Interfaces.Compartido;

public interface IRepositorio<T> where T : EntidadBase
{
    IQueryable<T> AsQueryable();
    Task<List<T>> ObtenerTodosAsync();
    Task<T?> ObtenerPorIdAsync(int id);
    Task<T> CrearAsync(T entidad);
    Task<T> ActualizarAsync(T entidad);
    Task EliminarAsync(int id);
}