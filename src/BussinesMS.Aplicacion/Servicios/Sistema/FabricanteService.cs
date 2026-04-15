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

public class FabricanteService : IFabricanteService
{
    private readonly IFabricanteRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<FabricanteService> _logger;

    public FabricanteService(IFabricanteRepository repo, IMapper mapper, ILogger<FabricanteService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<FabricanteDto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
    {
        try
        {
            var baseQuery = _repo.AsQueryable()
                .Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var filterLower = query.Filter.ToLower();
                baseQuery = baseQuery.Where(f => 
                    (f.Nombre != null && f.Nombre.ToLower().Contains(filterLower)));
            }

            (var filteredQuery, var totalCount) = baseQuery.ApplyFilters(query);

            var entidades = await filteredQuery.ToListAsync();

            var dtos = entidades.Select(f => new FabricanteDto
            {
                Id = f.Id,
                Nombre = f.Nombre,
                Descripcion = f.Descripcion,
                Activo = f.IsActive,
                CreatedAt = f.CreatedAt
            }).ToList();

            return new PagedResultDto<FabricanteDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener fabricantes");
            throw;
        }
    }

    public async Task<FabricanteDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var fabricante = await _repo.ObtenerPorIdAsync(id);
            if (fabricante == null || !fabricante.IsActive)
                return null;

            return new FabricanteDto
            {
                Id = fabricante.Id,
                Nombre = fabricante.Nombre,
                Descripcion = fabricante.Descripcion,
                Activo = fabricante.IsActive,
                CreatedAt = fabricante.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener fabricante {Id}", id);
            throw;
        }
    }

    public async Task<FabricanteDto> CrearAsync(CrearFabricanteDto dto)
    {
        try
        {
            var entidad = _mapper.Map<Fabricante>(dto);
            var creada = await _repo.CrearAsync(entidad);

            _logger.LogInformation("Fabricante creado: {Nombre}", creada.Nombre);

            return new FabricanteDto
            {
                Id = creada.Id,
                Nombre = creada.Nombre,
                Descripcion = creada.Descripcion,
                Activo = creada.IsActive,
                CreatedAt = creada.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear fabricante");
            throw;
        }
    }

    public async Task<FabricanteDto> ActualizarAsync(ActualizarFabricanteDto dto)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(dto.Id);
            ValidacionEntidad.VerificarActivo(existente, "Fabricante");

            existente.Nombre = dto.Nombre;
            existente.Descripcion = dto.Descripcion;
            existente.IsActive = dto.Activo;

            var actualizada = await _repo.ActualizarAsync(existente);

            _logger.LogInformation("Fabricante actualizado: {Nombre}", actualizada.Nombre);

            return new FabricanteDto
            {
                Id = actualizada.Id,
                Nombre = actualizada.Nombre,
                Descripcion = actualizada.Descripcion,
                Activo = actualizada.IsActive,
                CreatedAt = actualizada.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar fabricante {Id}", dto.Id);
            throw;
        }
    }

    public async Task EliminarAsync(int id)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(id);
            ValidacionEntidad.VerificarActivo(existente, "Fabricante");

            await _repo.EliminarAsync(id);
            _logger.LogInformation("Fabricante eliminado: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar fabricante {Id}", id);
            throw;
        }
    }
}