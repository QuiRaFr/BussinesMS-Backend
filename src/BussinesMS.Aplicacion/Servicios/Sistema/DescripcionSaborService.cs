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

public class DescripcionSaborService : IDescripcionSaborService
{
    private readonly IDescripcionSaborRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<DescripcionSaborService> _logger;

    public DescripcionSaborService(IDescripcionSaborRepository repo, IMapper mapper, ILogger<DescripcionSaborService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<DescripcionSaborDto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
    {
        try
        {
            var baseQuery = _repo.AsQueryable()
                .Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var filterLower = query.Filter.ToLower();
                baseQuery = baseQuery.Where(s => 
                    (s.Nombre != null && s.Nombre.ToLower().Contains(filterLower)));
            }

            (var filteredQuery, var totalCount) = baseQuery.ApplyFilters(query);

            var entidades = await filteredQuery.ToListAsync();

            var dtos = entidades.Select(s => new DescripcionSaborDto
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Activo = s.IsActive,
                CreatedAt = s.CreatedAt
            }).ToList();

            return new PagedResultDto<DescripcionSaborDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener sabores");
            throw;
        }
    }

    public async Task<DescripcionSaborDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var sabor = await _repo.ObtenerPorIdAsync(id);
            if (sabor == null || !sabor.IsActive)
                return null;

            return new DescripcionSaborDto
            {
                Id = sabor.Id,
                Nombre = sabor.Nombre,
                Activo = sabor.IsActive,
                CreatedAt = sabor.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener sabor {Id}", id);
            throw;
        }
    }

    public async Task<DescripcionSaborDto> CrearAsync(CrearDescripcionSaborDto dto)
    {
        try
        {
            var entidad = _mapper.Map<DescripcionSabor>(dto);
            var creada = await _repo.CrearAsync(entidad);

            _logger.LogInformation("Sabor creado: {Nombre}", creada.Nombre);

            return new DescripcionSaborDto
            {
                Id = creada.Id,
                Nombre = creada.Nombre,
                Activo = creada.IsActive,
                CreatedAt = creada.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear sabor");
            throw;
        }
    }

    public async Task<DescripcionSaborDto> ActualizarAsync(ActualizarDescripcionSaborDto dto)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(dto.Id);
            ValidacionEntidad.VerificarActivo(existente, "Sabor");

            existente.Nombre = dto.Nombre;
            existente.IsActive = dto.Activo;

            var actualizada = await _repo.ActualizarAsync(existente);

            _logger.LogInformation("Sabor actualizado: {Nombre}", actualizada.Nombre);

            return new DescripcionSaborDto
            {
                Id = actualizada.Id,
                Nombre = actualizada.Nombre,
                Activo = actualizada.IsActive,
                CreatedAt = actualizada.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar sabor {Id}", dto.Id);
            throw;
        }
    }

    public async Task EliminarAsync(int id)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(id);
            ValidacionEntidad.VerificarActivo(existente, "Sabor");

            await _repo.EliminarAsync(id);
            _logger.LogInformation("Sabor eliminado: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar sabor {Id}", id);
            throw;
        }
    }
}