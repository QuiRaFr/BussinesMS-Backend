using AutoMapper;
using BussinesMS.Aplicacion.Comun;
using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BussinesMS.Aplicacion.Servicios.Sistema;

public class MovimientoInventarioService : IMovimientoInventarioService
{
    private readonly IMovimientoInventarioRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<MovimientoInventarioService> _logger;

    public MovimientoInventarioService(
        IMovimientoInventarioRepository repo,
        IMapper mapper,
        ILogger<MovimientoInventarioService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<MovimientoInventarioDto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
    {
        try
        {
            var todos = await _repo.ObtenerTodosAsync();

            var totalCount = todos.Count;
            var paginados = todos
                .Skip((query.GetPageValue() - 1) * query.GetPageSizeValue())
                .Take(query.GetPageSizeValue())
                .ToList();

            return new PagedResultDto<MovimientoInventarioDto>
            {
                Items = _mapper.Map<List<MovimientoInventarioDto>>(paginados),
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener movimientos de inventario");
            throw;
        }
    }

    public async Task<PagedResultDto<MovimientoInventarioDto>> ObtenerFiltradosAsync(
        MovimientoInventarioFiltroDto filtro, GenericPaginationQueryDto query)
    {
        try
        {
            var todos = await _repo.ObtenerFiltradosAsync(
                filtro.LoteId, filtro.VarianteId, filtro.AlmacenOrigenId,
                filtro.AlmacenDestinoId, filtro.TipoMovimiento,
                filtro.FechaDesde, filtro.FechaHasta);

            var totalCount = todos.Count;
            var paginados = todos
                .Skip((query.GetPageValue() - 1) * query.GetPageSizeValue())
                .Take(query.GetPageSizeValue())
                .ToList();

            return new PagedResultDto<MovimientoInventarioDto>
            {
                Items = _mapper.Map<List<MovimientoInventarioDto>>(paginados),
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener movimientos filtrados");
            throw;
        }
    }

    public async Task<MovimientoInventarioDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var entidad = await _repo.ObtenerPorIdAsync(id);
            return entidad == null ? null : _mapper.Map<MovimientoInventarioDto>(entidad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener movimiento {Id}", id);
            throw;
        }
    }
}
