using AutoMapper;
using BussinesMS.Aplicacion.Comun;
using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Helpers;
using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using BussinesMS.Dominio.Entidades.Sistema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BussinesMS.Aplicacion.Servicios.Sistema;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoriaService> _logger;

    public CategoriaService(ICategoriaRepository repo, IMapper mapper, ILogger<CategoriaService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<CategoriaDto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
    {
        try
        {
            var baseQuery = _repo.AsQueryable()
                .Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var filterLower = query.Filter.ToLower();
                baseQuery = baseQuery.Where(c => 
                    (c.Nombre != null && c.Nombre.ToLower().Contains(filterLower)));
            }

            if (query.FiltroTipo.HasValue)
            {
                baseQuery = query.FiltroTipo.Value switch
                {
                    TipoCategoriaFiltro.Categoria => baseQuery.Where(x => x.ParentId == null),
                    TipoCategoriaFiltro.Subcategoria => baseQuery.Where(x => x.ParentId != null),
                    _ => baseQuery
                };
            }

            (var filteredQuery, var totalCount) = baseQuery.ApplyFilters(query);

            var entidades = await filteredQuery
                .Include(x => x.Parent)
                .ToListAsync();

            var dtos = entidades.Select(c => new CategoriaDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                ParentId = c.ParentId,
                NombrePadre = c.Parent != null ? c.Parent.Nombre : null,
                CreatedAt = c.CreatedAt,
                IsActive = c.IsActive
            }).ToList();

            return new PagedResultDto<CategoriaDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categorías");
            throw;
        }
    }

    public async Task<CategoriaDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var categoria = await _repo.ObtenerPorIdAsync(id);
            if (categoria == null || !categoria.IsActive)
                return null;

            return new CategoriaDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                ParentId = categoria.ParentId,
                NombrePadre = categoria.Parent?.Nombre,
                CreatedAt = categoria.CreatedAt,
                IsActive = categoria.IsActive
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categoría {Id}", id);
            throw;
        }
    }

    public async Task<List<CategoriaDto>> ObtenerRaicesAsync()
    {
        try
        {
            var entidades = await _repo.ObtenerRaicesAsync();
            return _mapper.Map<List<CategoriaDto>>(entidades);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categorías raíz");
            throw;
        }
    }

    public async Task<List<CategoriaDto>> ObtenerSubcategoriasAsync(int parentId)
    {
        try
        {
            ValidacionEntidad.VerificarExiste<Categoria>(
                await _repo.ObtenerPorIdAsync(parentId), "Categoría padre");

            var entidades = await _repo.ObtenerSubcategoriasAsync(parentId);
            return _mapper.Map<List<CategoriaDto>>(entidades);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener subcategorías de {ParentId}", parentId);
            throw;
        }
    }

public async Task<CategoriaDto> CrearAsync(CrearCategoriaDto dto)
    {
        try
        {
            if (dto.ParentId.HasValue && dto.ParentId.Value == 0)
                dto.ParentId = null;

            if (dto.ParentId.HasValue)
            {
                var padre = await _repo.ObtenerPorIdAsync(dto.ParentId.Value);
                ValidacionEntidad.VerificarCategoriaRaiz(padre, dto.ParentId);
            }

            var duplicado = await _repo.ObtenerPorNombreYParentAsync(dto.Nombre, dto.ParentId);
            if (duplicado != null)
            {
                if (!duplicado.IsActive)
                {
                    var reactivada = await _repo.ReactivarAsync(duplicado.Id);
                    _logger.LogInformation("Categoría reactivada: {Nombre}", reactivada.Nombre);
                    return _mapper.Map<CategoriaDto>(reactivada);
                }
                ValidacionEntidad.VerificarNoDuplicado(true, "categoría", dto.Nombre);
            }

            var entidad = _mapper.Map<Categoria>(dto);
            var creada = await _repo.CrearAsync(entidad);

            _logger.LogInformation("Categoría creada: {Nombre}", creada.Nombre);

            return _mapper.Map<CategoriaDto>(creada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear categoría");
            throw;
        }
    }

    public async Task<CategoriaDto> ActualizarAsync(ActualizarCategoriaDto dto)
    {
        try
        {
            if (dto.ParentId.HasValue && dto.ParentId.Value == 0)
                dto.ParentId = null;

            var existente = await _repo.ObtenerPorIdAsync(dto.Id);
            ValidacionEntidad.VerificarActivo(existente, "Categoría");

            if (dto.ParentId.HasValue)
            {
                var padre = await _repo.ObtenerPorIdAsync(dto.ParentId.Value);
                ValidacionEntidad.VerificarCategoriaRaiz(padre, dto.ParentId);
            }

            ValidacionEntidad.VerificarNoDuplicado(
                await _repo.ExisteNombreConParentAsync(dto.Nombre, dto.ParentId, dto.Id),
                "categoría", dto.Nombre);

            existente.Nombre = dto.Nombre;
            existente.ParentId = dto.ParentId;

            var actualizada = await _repo.ActualizarAsync(existente);

            _logger.LogInformation("Categoría actualizada: {Nombre}", actualizada.Nombre);

            return _mapper.Map<CategoriaDto>(actualizada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar categoría {Id}", dto.Id);
            throw;
        }
    }

    public async Task EliminarAsync(int id)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(id);
            ValidacionEntidad.VerificarActivo(existente, "Categoría");

            await _repo.EliminarAsync(id);
            _logger.LogInformation("Categoría eliminada: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar categoría {Id}", id);
            throw;
        }
    }
}