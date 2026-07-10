using AutoMapper;
using BussinesMS.Aplicacion.Comun;
using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using BussinesMS.Dominio.Entidades.Sistema;
using Microsoft.Extensions.Logging;
using BussinesMS.Aplicacion.Helpers;

namespace BussinesMS.Aplicacion.Servicios.Sistema;

public class TipoPresentacionService : ITipoPresentacionService
{
    private readonly ITipoPresentacionRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<TipoPresentacionService> _logger;

    public TipoPresentacionService(
        ITipoPresentacionRepository repo,
        IMapper mapper,
        ILogger<TipoPresentacionService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<TipoPresentacionDto>> ObtenerTodosAsync()
    {
        try
        {
            var entidades = await _repo.ObtenerTodosAsync();
            return _mapper.Map<List<TipoPresentacionDto>>(entidades);
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
            var entidad = await _repo.ObtenerPorIdAsync(id);
            if (entidad == null || !entidad.IsActive) return null;
            return _mapper.Map<TipoPresentacionDto>(entidad);
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
            ValidacionEntidad.VerificarNoDuplicado(
                await _repo.ExisteNombreAsync(dto.Nombre),
                "TipoPresentacion", dto.Nombre);

            // Orden=1 reservado para Unidad
            if (dto.Orden == 1)
                throw new InvalidOperationException("El orden 1 está reservado para 'Unidad'.");

            var entidad = new TipoPresentacion
            {
                Nombre = dto.Nombre,
                Orden = dto.Orden
            };

            var creada = await _repo.CrearAsync(entidad);
            _logger.LogInformation("TipoPresentacion creado: {Nombre}", creada.Nombre);
            return _mapper.Map<TipoPresentacionDto>(creada);
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
            ValidacionEntidad.VerificarActivo(existente, "TipoPresentacion");

            // No se puede cambiar nombre ni orden de Unidad
            if (await _repo.EsUnidadAsync(dto.Id))
                throw new InvalidOperationException("El tipo 'Unidad' no puede modificarse.");

            ValidacionEntidad.VerificarNoDuplicado(
                await _repo.ExisteNombreAsync(dto.Nombre, dto.Id),
                "TipoPresentacion", dto.Nombre);

            if (dto.Orden == 1)
                throw new InvalidOperationException("El orden 1 está reservado para 'Unidad'.");

            existente!.Nombre = dto.Nombre;
            existente.Orden = dto.Orden;
            existente.IsActive = dto.IsActive;

            var actualizada = await _repo.ActualizarAsync(existente);
            _logger.LogInformation("TipoPresentacion actualizado: {Nombre}", actualizada.Nombre);
            return _mapper.Map<TipoPresentacionDto>(actualizada);
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
            ValidacionEntidad.VerificarActivo(existente, "TipoPresentacion");

            if (await _repo.EsUnidadAsync(id))
                throw new InvalidOperationException("El tipo 'Unidad' no puede eliminarse.");

            await _repo.EliminarAsync(id);
            _logger.LogInformation("TipoPresentacion eliminado: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar tipo de presentación {Id}", id);
            throw;
        }
    }
}