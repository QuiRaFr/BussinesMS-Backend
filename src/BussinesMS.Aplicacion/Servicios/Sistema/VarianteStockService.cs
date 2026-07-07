using AutoMapper;
using BussinesMS.Aplicacion.Comun;
using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BussinesMS.Aplicacion.Servicios.Sistema;

public class VarianteStockService : IVarianteStockService
{
    private readonly IProductoVarianteRepository _varianteRepo;
    private readonly IInventarioLoteAlmacenRepository _loteAlmacenRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<VarianteStockService> _logger;

    public VarianteStockService(
        IProductoVarianteRepository varianteRepo,
        IInventarioLoteAlmacenRepository loteAlmacenRepo,
        IMapper mapper,
        ILogger<VarianteStockService> logger)
    {
        _varianteRepo = varianteRepo;
        _loteAlmacenRepo = loteAlmacenRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<VarianteStockDto>> ObtenerStockAsync(GenericPaginationQueryDto query)
    {
        try
        {
            var loteQuery = _loteAlmacenRepo.AsQueryable().Where(la => la.IsActive);

            var baseQuery = from v in _varianteRepo.AsQueryable().Where(v => v.IsActive)
                            join la in loteQuery on v.Id equals la.Lote.VarianteId into loteGroup
                            select new VarianteStockDto
                            {
                                Id = v.Id,
                                ProductoId = v.ProductoId,
                                NombreProducto = v.Producto!.Nombre,
                                DescripcionProducto = v.DescripcionProducto,
                                CodigoBarras = v.CodigoBarras,
                                PrecioVentaUnitario = v.PrecioVentaUnitario,
                                PrecioVentaMayoreo = v.PrecioVentaMayoreo,
                                PrecioCompra = v.PrecioCompra,
                                CodigoAlmacen = v.CodigoAlmacen,
                                IsActive = v.IsActive,
                                CategoriaId = v.Producto!.CategoriaId,
                                CategoriaNombre = v.Producto!.Categoria!.Nombre,
                                StockDisponible = loteGroup.Sum(la => la.StockDisponible),
                                CantidadVendida = loteGroup.Sum(la => la.CantidadVendida),
                                CantidadTrasladada = loteGroup.Sum(la => la.CantidadTrasladada),
                                CantidadVencida = loteGroup.Sum(la => la.CantidadVencida)
                            };

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var f = query.Filter.ToLower();
                baseQuery = baseQuery.Where(x =>
                    (x.NombreProducto != null && x.NombreProducto.ToLower().Contains(f)) ||
                    (x.DescripcionProducto != null && x.DescripcionProducto.ToLower().Contains(f)) ||
                    (x.CodigoBarras != null && x.CodigoBarras.ToLower().Contains(f)) ||
                    (x.CategoriaNombre != null && x.CategoriaNombre.ToLower().Contains(f)));
            }

            var (filteredQuery, totalCount) = baseQuery.ApplyFilters(query);

            var items = await filteredQuery.ToListAsync();

            return new PagedResultDto<VarianteStockDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener stock de variantes");
            throw;
        }
    }

    public async Task<VarianteStockDetalleDto?> ObtenerStockDetalleAsync(int varianteId)
    {
        try
        {
            var variante = await _varianteRepo.AsQueryable()
                .Where(v => v.Id == varianteId && v.IsActive)
                .Include(v => v.Producto)
                    .ThenInclude(p => p!.Categoria)
                .FirstOrDefaultAsync();

            if (variante == null) return null;

            var lotes = await _loteAlmacenRepo.AsQueryable()
                .Where(la => la.Lote != null && la.Lote.VarianteId == varianteId && la.IsActive)
                .Include(la => la.Lote)
                .OrderBy(la => la.Lote!.FechaVencimiento)
                .ToListAsync();

            var lotesDto = _mapper.Map<List<InventarioLoteAlmacenDto>>(lotes);

            return new VarianteStockDetalleDto
            {
                Id = variante.Id,
                ProductoId = variante.ProductoId,
                NombreProducto = variante.Producto?.Nombre,
                DescripcionProducto = variante.DescripcionProducto,
                CodigoBarras = variante.CodigoBarras,
                PrecioVentaUnitario = variante.PrecioVentaUnitario,
                PrecioVentaMayoreo = variante.PrecioVentaMayoreo,
                PrecioCompra = variante.PrecioCompra,
                CodigoAlmacen = variante.CodigoAlmacen,
                IsActive = variante.IsActive,
                CategoriaId = variante.Producto?.CategoriaId,
                CategoriaNombre = variante.Producto?.Categoria?.Nombre,
                StockDisponible = lotes.Sum(la => la.StockDisponible),
                CantidadVendida = lotes.Sum(la => la.CantidadVendida),
                CantidadTrasladada = lotes.Sum(la => la.CantidadTrasladada),
                CantidadVencida = lotes.Sum(la => la.CantidadVencida),
                Lotes = lotesDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalle de stock de variante {VarianteId}", varianteId);
            throw;
        }
    }
}
