using AutoMapper;
using BussinesMS.Aplicacion.Comun;
using BussinesMS.Aplicacion.DTOs.Auth;
using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.Interfaces.Auth;
using BussinesMS.Dominio.Entidades.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BussinesMS.Aplicacion.Servicios.Auth;

public class RolService : IRolService
{
    private readonly IRolRepository _repositorio;
    private readonly IMapper _mapper;
    private readonly ILogger<RolService> _logger;

    public RolService(IRolRepository repositorio, IMapper mapper, ILogger<RolService> logger)
    {
        _repositorio = repositorio;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<RolDto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
    {
        try
        {
            var baseQuery = _repositorio.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var filterLower = query.Filter.ToLower();
                baseQuery = baseQuery.Where(r => r.Nombre!.ToLower().Contains(filterLower));
            }

            (var filteredQuery, var totalCount) = baseQuery.ApplyFilters(query);

            var roles = await filteredQuery.ToListAsync();
            var dtos = _mapper.Map<List<RolDto>>(roles);

            return new PagedResultDto<RolDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener roles");
            throw;
        }
    }

    public async Task<RolDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var rol = await _repositorio.ObtenerPorIdAsync(id);
            return rol == null ? null : _mapper.Map<RolDto>(rol);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener rol {Id}", id);
            throw;
        }
    }

    public async Task<RolDto> CrearAsync(CrearRolDto dto)
    {
        try
        {
            var rol = _mapper.Map<Rol>(dto);
            rol.CreatedAt = DateTime.UtcNow;
            var resultado = await _repositorio.CrearAsync(rol);
            return _mapper.Map<RolDto>(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear rol");
            throw;
        }
    }

    public async Task<RolDto> ActualizarMenuIdsAsync(int id, List<int> menuIds)
    {
        try
        {
            var rol = await _repositorio.ObtenerPorIdAsync(id);
            if (rol == null)
                throw new Exception("Rol no encontrado");
            
            rol.MenuIds = JsonSerializer.Serialize(menuIds);
            await _repositorio.ActualizarAsync(rol);
            
            return _mapper.Map<RolDto>(rol);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar menuIds del rol {Id}", id);
            throw;
        }
    }
}