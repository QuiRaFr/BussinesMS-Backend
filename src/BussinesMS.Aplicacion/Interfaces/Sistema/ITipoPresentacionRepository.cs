using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface ITipoPresentacionRepository
{
    IQueryable<TipoPresentacion> AsQueryable();
    Task<List<TipoPresentacion>> ObtenerTodosAsync();
    Task<TipoPresentacion?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null);
    Task<bool> EsUnidadAsync(int id);
    Task<TipoPresentacion> CrearAsync(TipoPresentacion entidad);
    Task<TipoPresentacion> ActualizarAsync(TipoPresentacion entidad);
    Task EliminarAsync(int id);
}