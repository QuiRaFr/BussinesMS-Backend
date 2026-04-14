using AutoMapper;
using BussinesMS.Aplicacion.Comun;
using BussinesMS.Aplicacion.DTOs.Auth;
using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.Interfaces.Auth;
using BussinesMS.Dominio.Entidades.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaEntity = BussinesMS.Dominio.Entidades.Auth.Sistema;

namespace BussinesMS.Aplicacion.Servicios.Auth;

public class SistemaService : ISistemaService
{
    private readonly ISistemaRepository _repositorio;
    private readonly IMapper _mapper;
    private readonly ILogger<SistemaService> _logger;

    public SistemaService(ISistemaRepository repositorio, IMapper mapper, ILogger<SistemaService> logger)
    {
        _repositorio = repositorio;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<SistemaDto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
    {
        try
        {
            var baseQuery = _repositorio.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var filterLower = query.Filter.ToLower();
                baseQuery = baseQuery.Where(s => s.Nombre!.ToLower().Contains(filterLower));
            }

            (var filteredQuery, var totalCount) = baseQuery.ApplyFilters(query);

            var sistemas = await filteredQuery.ToListAsync();
            var dtos = _mapper.Map<List<SistemaDto>>(sistemas);

            return new PagedResultDto<SistemaDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener sistemas");
            throw;
        }
    }

    public async Task<SistemaDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var sistema = await _repositorio.ObtenerPorIdAsync(id);
            return sistema == null ? null : _mapper.Map<SistemaDto>(sistema);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener sistema {Id}", id);
            throw;
        }
    }

    public async Task<SistemaDto> CrearAsync(CrearSistemaDto dto)
    {
        try
        {
            var sistema = _mapper.Map<SistemaEntity>(dto);
            sistema.CreatedAt = DateTime.UtcNow;
            var resultado = await _repositorio.CrearAsync(sistema);
            return _mapper.Map<SistemaDto>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear sistema");
            throw;
        }
    }
}