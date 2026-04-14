using AutoMapper;
using BussinesMS.Aplicacion.Comun;
using BussinesMS.Aplicacion.DTOs.Auth;
using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.Interfaces.Auth;
using BussinesMS.Dominio.Entidades.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BussinesMS.Aplicacion.Servicios.Auth;

public class AlmacenService : IAlmacenService
{
    private readonly IAlmacenRepository _repositorio;
    private readonly IMapper _mapper;
    private readonly ILogger<AlmacenService> _logger;

    public AlmacenService(IAlmacenRepository repositorio, IMapper mapper, ILogger<AlmacenService> logger)
    {
        _repositorio = repositorio;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<AlmacenDto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
    {
        try
        {
            var baseQuery = _repositorio.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var filterLower = query.Filter.ToLower();
                baseQuery = baseQuery.Where(a => a.Nombre!.ToLower().Contains(filterLower));
            }

            (var filteredQuery, var totalCount) = baseQuery.ApplyFilters(query);

            var almacenes = await filteredQuery.ToListAsync();
            var dtos = _mapper.Map<List<AlmacenDto>>(almacenes);

            _logger.LogInformation("Se encontraron {Count} almacenes", dtos.Count);

            return new PagedResultDto<AlmacenDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener almacenes");
            throw;
        }
    }

    public async Task<AlmacenDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var almacen = await _repositorio.ObtenerPorIdAsync(id);
            return almacen == null ? null : _mapper.Map<AlmacenDto>(almacen);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener almacén {Id}", id);
            throw;
        }
    }

    public async Task<AlmacenDto> CrearAsync(CrearAlmacenDto dto)
    {
        try
        {
            var almacen = _mapper.Map<Almacen>(dto);
            almacen.CreatedAt = DateTime.UtcNow;
            var resultado = await _repositorio.CrearAsync(almacen);
            return _mapper.Map<AlmacenDto>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear almacén");
            throw;
        }
    }
}