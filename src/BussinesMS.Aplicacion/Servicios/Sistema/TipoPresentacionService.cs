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

public class TipoPresentacionService : ITipoPresentacionService
{
    private readonly ITipoPresentacionRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<TipoPresentacionService> _logger;

    public TipoPresentacionService(ITipoPresentacionRepository repo, IMapper mapper, ILogger<TipoPresentacionService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<TipoPresentacionDto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
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

            var dtos = entidades.Select(t => new TipoPresentacionDto
            {
                Id = t.Id,
                Nombre = t.Nombre,
                Factor = t.Factor,
                Activo = t.IsActive,
                CreatedAt = t.CreatedAt
            }).ToList();

            return new PagedResultDto<TipoPresentacionDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipos de presentación");
            throw;
        }
    }

    public async Task<TipoPresentacionDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var tipo = await _repo.ObtenerPorIdAsync(id);
            if (tipo == null || !tipo.IsActive)
                return null;

            return new TipoPresentacionDto
            {
                Id = tipo.Id,
                Nombre = tipo.Nombre,
                Factor = tipo.Factor,
                Activo = tipo.IsActive,
                CreatedAt = tipo.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipo de presentación {Id}", id);
            throw;
        }
    }

    public async Task<TipoPresentacionDto> CrearAsync(CrearTipoPresentacionDto dto)
    {
        try
        {
            var entidad = _mapper.Map<TipoPresentacion>(dto);
            var creada = await _repo.CrearAsync(entidad);

            _logger.LogInformation("Tipo de presentación creado: {Nombre}", creada.Nombre);

            return new TipoPresentacionDto
            {
                Id = creada.Id,
                Nombre = creada.Nombre,
                Factor = creada.Factor,
                Activo = creada.IsActive,
                CreatedAt = creada.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear tipo de presentación");
            throw;
        }
    }

    public async Task<TipoPresentacionDto> ActualizarAsync(ActualizarTipoPresentacionDto dto)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(dto.Id);
            ValidacionEntidad.VerificarActivo(existente, "Tipo de presentación");

            existente.Nombre = dto.Nombre;
            existente.Factor = dto.Factor;
            existente.IsActive = dto.Activo;

            var actualizada = await _repo.ActualizarAsync(existente);

            _logger.LogInformation("Tipo de presentación actualizado: {Nombre}", actualizada.Nombre);

            return new TipoPresentacionDto
            {
                Id = actualizada.Id,
                Nombre = actualizada.Nombre,
                Factor = actualizada.Factor,
                Activo = actualizada.IsActive,
                CreatedAt = actualizada.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar tipo de presentación {Id}", dto.Id);
            throw;
        }
    }

    public async Task EliminarAsync(int id)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(id);
            ValidacionEntidad.VerificarActivo(existente, "Tipo de presentación");

            await _repo.EliminarAsync(id);
            _logger.LogInformation("Tipo de presentación eliminado: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar tipo de presentación {Id}", id);
            throw;
        }
    }
}