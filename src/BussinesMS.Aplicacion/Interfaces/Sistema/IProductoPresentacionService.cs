using BussinesMS.Aplicacion.DTOs.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface IProductoPresentacionService
{
    Task<List<ProductoPresentacionDto>> ObtenerPorVarianteAsync(int varianteId);
    Task<ProductoPresentacionDto?> ObtenerPorIdAsync(int id);
    Task<ProductoPresentacionDto> CrearAsync(CrearProductoPresentacionDto dto);
    Task<ProductoPresentacionDto> ActualizarAsync(ActualizarProductoPresentacionDto dto);
    Task EliminarAsync(int id);
    Task<StockConversionDto> ConvertirStockAsync(int varianteId, int stockEnUnidades);
}