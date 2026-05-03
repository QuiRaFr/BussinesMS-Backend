using AutoMapper;
using BussinesMS.Aplicacion.Comun;
using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.DTOs.Plantillas;
using BussinesMS.Aplicacion.Helpers;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using BussinesMS.Dominio.Entidades.Sistema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BussinesMS.Aplicacion.Servicios.Sistema;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductoService> _logger;
    private readonly ISistemaUnitOfWork _uow;

    public ProductoService(
        IProductoRepository repo,
        IMapper mapper,
        ILogger<ProductoService> logger,
        ISistemaUnitOfWork uow)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
        _uow = uow;
    }

    public async Task<PagedResultDto<ProductoDto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
    {
        try
        {
            IQueryable<Producto> baseQuery;

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var f = query.Filter.ToLower();
                baseQuery = _repo.AsQueryable()
                    .Where(x => x.IsActive)
                    .Include(x => x.Categoria)
                    .Include(x => x.Fabricante)
                    .Where(x => 
                        x.Nombre.ToLower().Contains(f) ||
                        x.CodigoInterno.ToLower().Contains(f));
            }
            else
            {
                baseQuery = _repo.AsQueryable()
                    .Where(x => x.IsActive)
                    .Include(x => x.Categoria)
                    .Include(x => x.Fabricante);
            }

            var orderedQuery = baseQuery.OrderBy(x => x.Nombre);
            (var filteredQuery, var totalCount) = orderedQuery.ApplyFilters(query);
            var entidades = await filteredQuery.ToListAsync();

            var dtos = _mapper.Map<List<ProductoDto>>(entidades);

            return new PagedResultDto<ProductoDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = query.GetPageValue(),
                PageSize = query.GetPageSizeValue()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener productos");
            throw;
        }
    }

    public async Task<ProductoDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var entidad = await _repo.ObtenerConDetallesAsync(id);
            if (entidad == null || !entidad.IsActive) return null;

            return _mapper.Map<ProductoDto>(entidad);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener producto {Id}", id);
            throw;
        }
    }

    public async Task<ProductoDto> CrearAsync(CrearProductoDto dto)
    {
        try
        {
            ValidacionEntidad.VerificarNoDuplicado(
                await _repo.ExisteNombreAsync(dto.Nombre),
                "Producto", dto.Nombre);

            await _uow.BeginTransactionAsync();
            try
            {
                var entidad = new Producto
                {
                    Nombre = dto.Nombre,
                    CategoriaId = dto.CategoriaId,
                    FabricanteId = dto.FabricanteId
                };

                var creada = await _repo.CrearAsync(entidad);

                var maxId = await _repo.AsQueryable()
                    .Where(x => x.CodigoInterno != null)
                    .MaxAsync(x => (int?)x.Id) ?? 0;

                var siguienteNumero = maxId + 1;
                creada.CodigoInterno = $"PROD-{siguienteNumero:D5}";
                await _repo.ActualizarAsync(creada);

                await _uow.CommitAsync();

                _logger.LogInformation("Producto creado: {Codigo} - {Nombre}", creada.CodigoInterno, creada.Nombre);

                return _mapper.Map<ProductoDto>(creada);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear producto");
            throw;
        }
    }

    public async Task<ProductoDto> ActualizarAsync(ActualizarProductoDto dto)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(dto.Id);
            ValidacionEntidad.VerificarActivo(existente, "Producto");

            ValidacionEntidad.VerificarNoDuplicado(
                await _repo.ExisteNombreAsync(dto.Nombre, dto.Id),
                "Producto", dto.Nombre);

            existente!.Nombre = dto.Nombre;
            existente.CategoriaId = dto.CategoriaId;
            existente.FabricanteId = dto.FabricanteId;
            existente.IsActive = dto.IsActive;

            var actualizada = await _repo.ActualizarAsync(existente);

            _logger.LogInformation("Producto actualizado: {Codigo} - {Nombre}", actualizada.CodigoInterno, actualizada.Nombre);

            return _mapper.Map<ProductoDto>(actualizada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar producto {Id}", dto.Id);
            throw;
        }
    }

    public async Task EliminarAsync(int id)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(id);
            ValidacionEntidad.VerificarActivo(existente, "Producto");

            await _repo.EliminarAsync(id);
            _logger.LogInformation("Producto eliminado: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto {Id}", id);
            throw;
        }
    }
}