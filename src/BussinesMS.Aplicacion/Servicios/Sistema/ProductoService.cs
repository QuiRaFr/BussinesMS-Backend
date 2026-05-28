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

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _repo;
    private readonly ICategoriaRepository _categoriaRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductoService> _logger;
    private readonly ISistemaUnitOfWork _uow;

    public ProductoService(
        IProductoRepository repo,
        ICategoriaRepository categoriaRepo,
        IMapper mapper,
        ILogger<ProductoService> logger,
        ISistemaUnitOfWork uow)
    {
        _repo = repo;
        _categoriaRepo = categoriaRepo;
        _mapper = mapper;
        _logger = logger;
        _uow = uow;
    }

    public async Task<PagedResultDto<ProductoDto>> ObtenerTodosAsync(ProductoPaginationQueryDto query)
    {
        try
        {
            IQueryable<Producto> baseQuery = _repo.AsQueryable()
                .Where(x => x.IsActive)
                .Include(x => x.Categoria)
                .Include(x => x.Fabricante);

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var f = query.Filter.ToLower();
                baseQuery = baseQuery.Where(x =>
                    x.Nombre.ToLower().Contains(f) ||
                    x.CodigoInterno.ToLower().Contains(f));
            }

            // CategoriaId: trae productos cuya categoría ES esa categoría padre
            // La categoría padre tiene ParentId == null, y las subcategorías tienen ParentId == categoriaId
            // CategoriaId=1 (GALLETAS) → productos cuya categoría tiene ParentId == 1
            if (query.CategoriaId.HasValue)
            {
                baseQuery = baseQuery.Where(x =>
                    x.Categoria!.ParentId == query.CategoriaId.Value);
            }

            // SubcategoriaId=31 → productos en esa subcategoría exacta
            if (query.SubcategoriaId.HasValue)
            {
                baseQuery = baseQuery.Where(x =>
                    x.CategoriaId == query.SubcategoriaId.Value);
            }

            (var filteredQuery, var totalCount) = baseQuery
                .OrderBy(x => x.Nombre)
                .ApplyFilters(query);

            var entidades = await filteredQuery.ToListAsync();
            var dtos = _mapper.Map<List<ProductoDto>>(entidades);

            return new PagedResultDto<ProductoDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos");
            throw;
        }
    }

    public async Task<ProductoDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var entidad = await _repo.ObtenerConDetallesAsync(id);
            if (entidad == null || !entidad.IsActive) return null;

            return _mapper.Map<ProductoDto>(entidad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener producto {Id}", id);
            throw;
        }
    }
    public async Task<ProductoDto> CrearAsync(CrearProductoDto dto)
    {
        try
        {
            ValidacionEntidad.VerificarNoDuplicado(
                await _repo.ExisteNombreAsync(dto.Nombre),
                "Producto", dto.Nombre);

            // ✅ Validar que la categoría enviada sea una subcategoría
            var categoria = await _categoriaRepo.ObtenerPorIdAsync(dto.SubcategoriaId);
            if (categoria == null || !categoria.IsActive)
                throw new InvalidOperationException($"La categoría con Id {dto.SubcategoriaId} no existe o no está activa.");

            if (categoria.ParentId == null)
                throw new InvalidOperationException($"'{categoria.Nombre}' es una categoría padre. Debe seleccionar una subcategoría.");

            await _uow.BeginTransactionAsync();
            try
            {
                var entidad = new Producto
                {
                    Nombre = dto.Nombre,
                    CategoriaId = dto.SubcategoriaId,  // sigue guardando en CategoriaId
                    FabricanteId = dto.FabricanteId
                };

                var creada = await _repo.CrearAsync(entidad);

                var maxId = await _repo.AsQueryable()
                    .Where(x => x.CodigoInterno != null)
                    .MaxAsync(x => (int?)x.Id) ?? 0;

                creada.CodigoInterno = $"PROD-{maxId + 1:D5}";
                await _repo.ActualizarAsync(creada);
                await _uow.CommitAsync();

                _logger.LogInformation("Producto creado: {Codigo} - {Nombre}", creada.CodigoInterno, creada.Nombre);
                return _mapper.Map<ProductoDto>(creada);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear producto");
            throw;
        }
    }

    public async Task<ProductoDto> ActualizarAsync(ActualizarProductoDto dto)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(dto.Id);
            ValidacionEntidad.VerificarActivo(existente, "Producto");

            ValidacionEntidad.VerificarNoDuplicado(
                await _repo.ExisteNombreAsync(dto.Nombre, dto.Id),
                "Producto", dto.Nombre);

            // ✅ Misma validación en actualizar
            var categoria = await _categoriaRepo.ObtenerPorIdAsync(dto.SubcategoriaId);
            if (categoria == null || !categoria.IsActive)
                throw new InvalidOperationException($"La categoría con Id {dto.SubcategoriaId} no existe o no está activa.");

            if (categoria.ParentId == null)
                throw new InvalidOperationException($"'{categoria.Nombre}' es una categoría padre. Debe seleccionar una subcategoría.");

            existente!.Nombre = dto.Nombre;
            existente.CategoriaId = dto.SubcategoriaId;
            existente.FabricanteId = dto.FabricanteId;
            existente.IsActive = dto.IsActive;

            var actualizada = await _repo.ActualizarAsync(existente);

            _logger.LogInformation("Producto actualizado: {Codigo} - {Nombre}", actualizada.CodigoInterno, actualizada.Nombre);
            return _mapper.Map<ProductoDto>(actualizada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar producto {Id}", dto.Id);
            throw;
        }
    }

    public async Task EliminarAsync(int id)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(id);
            ValidacionEntidad.VerificarActivo(existente, "Producto");

            await _repo.EliminarAsync(id);
            _logger.LogInformation("Producto eliminado: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto {Id}", id);
            throw;
        }
    }
}