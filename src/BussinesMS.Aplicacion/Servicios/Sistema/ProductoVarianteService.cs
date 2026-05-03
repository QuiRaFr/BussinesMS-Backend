using AutoMapper;
using BussinesMS.Aplicacion.Comun;
using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.Helpers;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using BussinesMS.Dominio.Entidades.Sistema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BussinesMS.Aplicacion.Servicios.Sistema;

public class ProductoVarianteService : IProductoVarianteService
{
    private readonly IProductoVarianteRepository _repo;
    private readonly IProductoRepository _productoRepo;
    private readonly IDescripcionSaborRepository _saborRepo;
    private readonly IDescripcionTamanioRepository _tamanioRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductoVarianteService> _logger;

    public ProductoVarianteService(
        IProductoVarianteRepository repo,
        IProductoRepository productoRepo,
        IDescripcionSaborRepository saborRepo,
        IDescripcionTamanioRepository tamanioRepo,
        IMapper mapper,
        ILogger<ProductoVarianteService> logger)
    {
        _repo = repo;
        _productoRepo = productoRepo;
        _saborRepo = saborRepo;
        _tamanioRepo = tamanioRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<ProductoVarianteDto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
    {
        try
        {
            IQueryable<ProductoVariante> baseQuery;

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var f = query.Filter.ToLower();
                baseQuery = _repo.AsQueryable()
                    .Where(x => x.IsActive)
                    .Include(x => x.Producto)
                    .Include(x => x.Sabor)
                    .Include(x => x.Tamanio)
                    .Where(x => 
                        x.Producto!.Nombre.ToLower().Contains(f) ||
                        (x.CodigoBarras != null && x.CodigoBarras.ToLower().Contains(f)) ||
                        x.Sabor!.Nombre.ToLower().Contains(f) ||
                        x.Tamanio!.Nombre.ToLower().Contains(f));
            }
            else
            {
                baseQuery = _repo.AsQueryable()
                    .Where(x => x.IsActive)
                    .Include(x => x.Producto)
                    .Include(x => x.Sabor)
                    .Include(x => x.Tamanio);
            }

            var orderedQuery = baseQuery.OrderBy(x => x.Producto!.Nombre).ThenBy(x => x.Sabor!.Nombre).ThenBy(x => x.Tamanio!.Nombre);
            (var filteredQuery, var totalCount) = orderedQuery.ApplyFilters(query);
            var entidades = await filteredQuery.ToListAsync();

            var dtos = _mapper.Map<List<ProductoVarianteDto>>(entidades);

            return new PagedResultDto<ProductoVarianteDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener variantes de producto");
            throw;
        }
    }

    public async Task<ProductoVarianteDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var entidad = await _repo.ObtenerConDetallesAsync(id);
            if (entidad == null || !entidad.IsActive) return null;

            return _mapper.Map<ProductoVarianteDto>(entidad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener variante de producto {Id}", id);
            throw;
        }
    }

    public async Task<ProductoVarianteDto> CrearAsync(CrearProductoVarianteDto dto)
    {
        try
        {
            var producto = await _productoRepo.ObtenerPorIdAsync(dto.ProductoId);
            ValidacionEntidad.VerificarActivo(producto, "Producto");

            var sabor = await _saborRepo.ObtenerPorIdAsync(dto.SaborId);
            ValidacionEntidad.VerificarActivo(sabor, "Sabor");

            var tamanio = await _tamanioRepo.ObtenerPorIdAsync(dto.TamanioId);
            ValidacionEntidad.VerificarActivo(tamanio, "Tamaño");

            ValidacionEntidad.VerificarNoDuplicado(
                await _repo.ExisteCombinacionAsync(dto.ProductoId, dto.SaborId, dto.TamanioId),
                "Variante", $"producto + sabor + tamaño");

            if (!string.IsNullOrWhiteSpace(dto.CodigoBarras))
            {
                ValidacionEntidad.VerificarNoDuplicado(
                    await _repo.ExisteCodigoBarrasAsync(dto.CodigoBarras),
                    "Código de barras", dto.CodigoBarras!);
            }

            var entidad = _mapper.Map<ProductoVariante>(dto);
            var creada = await _repo.CrearAsync(entidad);

            _logger.LogInformation("Variante de producto creada: Producto {ProductoId} - Sabor {SaborId} - Tamaño {TamanioId}", 
                dto.ProductoId, dto.SaborId, dto.TamanioId);

            var completa = await _repo.ObtenerConDetallesAsync(creada.Id);
            return _mapper.Map<ProductoVarianteDto>(completa);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear variante de producto");
            throw;
        }
    }

    public async Task<ProductoVarianteDto> ActualizarAsync(ActualizarProductoVarianteDto dto)
    {
        try
        {
            var existente = await _repo.ObtenerConDetallesAsync(dto.Id);
            ValidacionEntidad.VerificarActivo(existente, "Variante de producto");

            var producto = await _productoRepo.ObtenerPorIdAsync(dto.ProductoId);
            ValidacionEntidad.VerificarActivo(producto, "Producto");

            var sabor = await _saborRepo.ObtenerPorIdAsync(dto.SaborId);
            ValidacionEntidad.VerificarActivo(sabor, "Sabor");

            var tamanio = await _tamanioRepo.ObtenerPorIdAsync(dto.TamanioId);
            ValidacionEntidad.VerificarActivo(tamanio, "Tamaño");

            ValidacionEntidad.VerificarNoDuplicado(
                await _repo.ExisteCombinacionAsync(dto.ProductoId, dto.SaborId, dto.TamanioId, dto.Id),
                "Variante", $"producto + sabor + tamaño");

            if (!string.IsNullOrWhiteSpace(dto.CodigoBarras))
            {
                ValidacionEntidad.VerificarNoDuplicado(
                    await _repo.ExisteCodigoBarrasAsync(dto.CodigoBarras, dto.Id),
                    "Código de barras", dto.CodigoBarras!);
            }

            existente!.ProductoId = dto.ProductoId;
            existente.CodigoBarras = dto.CodigoBarras;
            existente.SaborId = dto.SaborId;
            existente.TamanioId = dto.TamanioId;
            existente.PrecioVentaActual = dto.PrecioVentaActual;
            existente.CodigoAlmacen = dto.CodigoAlmacen;
            existente.IsActive = dto.IsActive;

            var actualizada = await _repo.ActualizarAsync(existente);

            _logger.LogInformation("Variante de producto actualizada: {Id}", dto.Id);

            return _mapper.Map<ProductoVarianteDto>(actualizada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar variante de producto {Id}", dto.Id);
            throw;
        }
    }

    public async Task EliminarAsync(int id)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(id);
            ValidacionEntidad.VerificarActivo(existente, "Variante de producto");

            await _repo.EliminarAsync(id);
            _logger.LogInformation("Variante de producto eliminada: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar variante de producto {Id}", id);
            throw;
        }
    }
}