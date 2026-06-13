using AutoMapper;
using BussinesMS.Aplicacion.Comun;
using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Helpers;
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
                    c.Nombre != null && c.Nombre.ToLower().Contains(filterLower));
            }

            (var filteredQuery, var totalCount) = baseQuery.ApplyFilters(query);

            var entidades = await filteredQuery.ToListAsync();

            var dtos = entidades.Select(c => new CategoriaDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
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
                Descripcion = categoria.Descripcion,
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

    public async Task<(CategoriaDto Categoria, bool FueReactivada)> CrearAsync(CrearCategoriaDto dto)
    {
        try
        {
            var duplicado = await _repo.ObtenerPorNombreAsync(dto.Nombre);

            if (duplicado != null)
            {
                if (duplicado.IsActive)
                    throw new InvalidOperationException($"La categoría '{dto.Nombre}' ya existe.");

                var reactivada = await _repo.ReactivarAsync(duplicado.Id);
                _logger.LogInformation("Categoría reactivada: {Nombre}", reactivada.Nombre);
                return (_mapper.Map<CategoriaDto>(reactivada), true);
            }

            var entidad = _mapper.Map<Categoria>(dto);
            var creada = await _repo.CrearAsync(entidad);
            _logger.LogInformation("Categoría creada: {Nombre}", creada.Nombre);
            return (_mapper.Map<CategoriaDto>(creada), false);
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
            var existente = await _repo.ObtenerPorIdAsync(dto.Id);
            ValidacionEntidad.VerificarActivo(existente, "Categoría");

            var duplicado = await _repo.ObtenerPorNombreAsync(dto.Nombre);
            if (duplicado != null && duplicado.Id != dto.Id)
            {
                if (duplicado.IsActive)
                    throw new InvalidOperationException($"La categoría '{dto.Nombre}' ya existe.");

                throw new InvalidOperationException($"La categoría '{dto.Nombre}' ya existe pero está desactivada, debe reactivarla.");
            }

            existente.Nombre = dto.Nombre;
            existente.Descripcion = dto.Descripcion;

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
