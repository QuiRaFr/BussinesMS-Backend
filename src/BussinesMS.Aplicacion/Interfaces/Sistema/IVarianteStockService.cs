using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IVarianteStockService
{
    Task<PagedResultDto<VarianteStockDto>> ObtenerStockAsync(GenericPaginationQueryDto query);
    Task<VarianteStockDetalleDto?> ObtenerStockDetalleAsync(int varianteId);
}
