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
    private readonly IInventarioLoteRepository _loteRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<VarianteStockService> _logger;

    public VarianteStockService(
        IProductoVarianteRepository varianteRepo,
        IInventarioLoteRepository loteRepo,
        IMapper mapper,
        ILogger<VarianteStockService> logger)
    {
        _varianteRepo = varianteRepo;
        _loteRepo = loteRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<VarianteStockDto>> ObtenerStockAsync(GenericPaginationQueryDto query)
    {
        try
        {
            var loteQuery = _loteRepo.AsQueryable().Where(l => l.IsActive);

            var baseQuery = from v in _varianteRepo.AsQueryable().Where(v => v.IsActive)
                            join l in loteQuery on v.Id equals l.VarianteId into loteGroup
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
                                StockDisponible = loteGroup.Sum(l => l.StockDisponible),
                                CantidadVendida = loteGroup.Sum(l => l.CantidadVendida),
                                CantidadTrasladada = loteGroup.Sum(l => l.CantidadTrasladada),
                                CantidadVencida = loteGroup.Sum(l => l.CantidadVencida)
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

            var lotes = await _loteRepo.AsQueryable()
                .Where(l => l.VarianteId == varianteId && l.IsActive)
                .OrderBy(l => l.FechaVencimiento)
                .ToListAsync();

            var lotesDto = _mapper.Map<List<InventarioLoteDto>>(lotes);

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
                StockDisponible = lotes.Sum(l => l.StockDisponible),
                CantidadVendida = lotes.Sum(l => l.CantidadVendida),
                CantidadTrasladada = lotes.Sum(l => l.CantidadTrasladada),
                CantidadVencida = lotes.Sum(l => l.CantidadVencida),
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
