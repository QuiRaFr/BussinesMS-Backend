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

public class DescripcionTamanioService : IDescripcionTamanioService
{
    private readonly IDescripcionTamanioRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<DescripcionTamanioService> _logger;

    public DescripcionTamanioService(IDescripcionTamanioRepository repo, IMapper mapper, ILogger<DescripcionTamanioService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<DescripcionTamanioDto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
    {
        try
        {
            var baseQuery = _repo.AsQueryable()
                .Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var filterLower = query.Filter.ToLower();
                baseQuery = baseQuery.Where(t => 
                    (t.Nombre != null && t.Nombre.ToLower().Contains(filterLower)));
            }

            (var filteredQuery, var totalCount) = baseQuery.ApplyFilters(query);

            var entidades = await filteredQuery.ToListAsync();

            var dtos = entidades.Select(t => new DescripcionTamanioDto
            {
                Id = t.Id,
                Nombre = t.Nombre,
                Activo = t.IsActive,
                CreatedAt = t.CreatedAt
            }).ToList();

            return new PagedResultDto<DescripcionTamanioDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tamaños");
            throw;
        }
    }

    public async Task<DescripcionTamanioDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var tamanio = await _repo.ObtenerPorIdAsync(id);
            if (tamanio == null || !tamanio.IsActive)
                return null;

            return new DescripcionTamanioDto
            {
                Id = tamanio.Id,
                Nombre = tamanio.Nombre,
                Activo = tamanio.IsActive,
                CreatedAt = tamanio.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tamaño {Id}", id);
            throw;
        }
    }

    public async Task<DescripcionTamanioDto> CrearAsync(CrearDescripcionTamanioDto dto)
    {
        try
        {
            var entidad = _mapper.Map<DescripcionTamanio>(dto);
            var creado = await _repo.CrearAsync(entidad);

            _logger.LogInformation("Tamaño creado: {Nombre}", creado.Nombre);

            return new DescripcionTamanioDto
            {
                Id = creado.Id,
                Nombre = creado.Nombre,
                Activo = creado.IsActive,
                CreatedAt = creado.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear tamaño");
            throw;
        }
    }

    public async Task<DescripcionTamanioDto> ActualizarAsync(ActualizarDescripcionTamanioDto dto)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(dto.Id);
            ValidacionEntidad.VerificarActivo(existente, "Tamaño");

            existente.Nombre = dto.Nombre;
            existente.IsActive = dto.Activo;

            var actualizado = await _repo.ActualizarAsync(existente);

            _logger.LogInformation("Tamaño actualizado: {Nombre}", actualizado.Nombre);

            return new DescripcionTamanioDto
            {
                Id = actualizado.Id,
                Nombre = actualizado.Nombre,
                Activo = actualizado.IsActive,
                CreatedAt = actualizado.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar tamaño {Id}", dto.Id);
            throw;
        }
    }

    public async Task EliminarAsync(int id)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(id);
            ValidacionEntidad.VerificarActivo(existente, "Tamaño");

            await _repo.EliminarAsync(id);
            _logger.LogInformation("Tamaño eliminado: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar tamaño {Id}", id);
            throw;
        }
    }
}