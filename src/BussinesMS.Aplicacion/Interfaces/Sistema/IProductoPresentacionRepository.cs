using BussinesMS.Dominio.Entidades.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IProductoPresentacionRepository
{
    IQueryable<ProductoPresentacion> AsQueryable();
    Task<List<ProductoPresentacion>> ObtenerPorVarianteAsync(int varianteId);
    Task<ProductoPresentacion?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteCombinacionAsync(int varianteId, int tipoPresentacionId, int? excludeId = null);
    Task<bool> ExisteDefaultReporteAsync(int varianteId, int? excludeId = null);
    Task<ProductoPresentacion> CrearAsync(ProductoPresentacion entidad);
    Task<ProductoPresentacion> ActualizarAsync(ProductoPresentacion entidad);
    Task EliminarAsync(int id);
    Task EliminarPorVarianteAsync(int varianteId);
    
}