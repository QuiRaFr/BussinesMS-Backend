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

public class ProveedorService : IProveedorService
{
    private readonly IProveedorRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<ProveedorService> _logger;

    public ProveedorService(
        IProveedorRepository repo,
        IMapper mapper,
        ILogger<ProveedorService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<ProveedorDto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
    {
        try
        {
            var baseQuery = _repo.AsQueryable().Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var f = query.Filter.ToLower();
                baseQuery = baseQuery.Where(x =>
                    x.Nombre.ToLower().Contains(f));
            }

            (var filteredQuery, var totalCount) = baseQuery.ApplyFilters(query);
            var entidades = await filteredQuery.ToListAsync();

            return new PagedResultDto<ProveedorDto>
            {
                Items = _mapper.Map<List<ProveedorDto>>(entidades),
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proveedores");
            throw;
        }
    }

    public async Task<ProveedorDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var entidad = await _repo.ObtenerPorIdAsync(id);
            if (entidad == null || !entidad.IsActive) return null;

            return _mapper.Map<ProveedorDto>(entidad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener proveedor {Id}", id);
            throw;
        }
    }

    public async Task<ProveedorDto> CrearAsync(CrearProveedorDto dto)
    {
        try
        {
            ValidacionEntidad.VerificarNoDuplicado(
                await _repo.ExisteNombreAsync(dto.Nombre),
                "Proveedor", dto.Nombre);

            var entidad = _mapper.Map<Proveedor>(dto);
            var creada = await _repo.CrearAsync(entidad);

            _logger.LogInformation("Proveedor creado: {Nombre}", creada.Nombre);

            return _mapper.Map<ProveedorDto>(creada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear proveedor");
            throw;
        }
    }

    public async Task<ProveedorDto> ActualizarAsync(ActualizarProveedorDto dto)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(dto.Id);
            ValidacionEntidad.VerificarActivo(existente, "Proveedor");

            ValidacionEntidad.VerificarNoDuplicado(
                await _repo.ExisteNombreAsync(dto.Nombre, dto.Id),
                "Proveedor", dto.Nombre);

            existente!.Nombre = dto.Nombre;
            existente.Nit = dto.Nit;
            existente.Telefono = dto.Telefono;
            existente.IsActive = dto.IsActive;

            var actualizada = await _repo.ActualizarAsync(existente);

            _logger.LogInformation("Proveedor actualizado: {Nombre}", actualizada.Nombre);

            return _mapper.Map<ProveedorDto>(actualizada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar proveedor {Id}", dto.Id);
            throw;
        }
    }

    public async Task EliminarAsync(int id)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(id);
            ValidacionEntidad.VerificarActivo(existente, "Proveedor");

            await _repo.EliminarAsync(id);
            _logger.LogInformation("Proveedor eliminado: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar proveedor {Id}", id);
            throw;
        }
    }
}
