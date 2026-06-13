using AutoMapper;
using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Helpers;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using BussinesMS.Dominio.Entidades.Sistema;
using Microsoft.Extensions.Logging;

namespace BussinesMS.Aplicacion.Servicios.Sistema;

public class ProductoPresentacionService : IProductoPresentacionService
{
    private readonly IProductoPresentacionRepository _repo;
    private readonly ITipoPresentacionRepository _tipoRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductoPresentacionService> _logger;

    public ProductoPresentacionService(
        IProductoPresentacionRepository repo,
        ITipoPresentacionRepository tipoRepo,
        IMapper mapper,
        ILogger<ProductoPresentacionService> logger)
    {
        _repo = repo;
        _tipoRepo = tipoRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<ProductoPresentacionDto>> ObtenerPorVarianteAsync(int varianteId)
    {
        try
        {
            var entidades = await _repo.ObtenerPorVarianteAsync(varianteId);
            return entidades.Select(e => MapearConEquivalencia(e, entidades)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener presentaciones de variante {VarianteId}", varianteId);
            throw;
        }
    }

    public async Task<ProductoPresentacionDto?> ObtenerPorIdAsync(int id)
    {
        try
        {
            var entidad = await _repo.ObtenerPorIdAsync(id);
            if (entidad == null || !entidad.IsActive) return null;

            var todas = await _repo.ObtenerPorVarianteAsync(entidad.VarianteId);
            return MapearConEquivalencia(entidad, todas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener presentación {Id}", id);
            throw;
        }
    }

    public async Task<ProductoPresentacionDto> CrearAsync(CrearProductoPresentacionDto dto)
    {
        try
        {
            // Validar tipo existe
            var tipo = await _tipoRepo.ObtenerPorIdAsync(dto.TipoPresentacionId);
            if (tipo == null || !tipo.IsActive)
                throw new InvalidOperationException($"TipoPresentacion {dto.TipoPresentacionId} no existe o no está activo.");

            // No duplicar tipo por variante
            if (await _repo.ExisteCombinacionAsync(dto.VarianteId, dto.TipoPresentacionId))
                throw new InvalidOperationException($"La variante ya tiene una presentación de tipo '{tipo.Nombre}'.");

            // Solo 1 default por variante
            if (dto.EsDefaultReporte && await _repo.ExisteDefaultReporteAsync(dto.VarianteId))
                throw new InvalidOperationException("La variante ya tiene una presentación marcada como default de reporte.");

            // Unidad siempre CantidadDePadre=1 y PadreId=null
            if (await _tipoRepo.EsUnidadAsync(dto.TipoPresentacionId))
            {
                dto.CantidadDePadre = 1;
                dto.PresentacionPadreId = null;
            }
            else
            {
                if (dto.CantidadDePadre <= 1)
                    throw new InvalidOperationException("CantidadDePadre debe ser mayor a 1 para presentaciones que no son Unidad.");
                if (dto.PresentacionPadreId == null)
                    throw new InvalidOperationException("Debe indicar PresentacionPadreId para presentaciones que no son Unidad.");
            }

            var entidad = new ProductoPresentacion
            {
                VarianteId = dto.VarianteId,
                TipoPresentacionId = dto.TipoPresentacionId,
                NombrePersonalizado = dto.NombrePersonalizado,
                CantidadDePadre = dto.CantidadDePadre,
                PresentacionPadreId = dto.PresentacionPadreId,
                EsDefaultReporte = dto.EsDefaultReporte,
                CodigoBarras = dto.CodigoBarras
            };

            var creada = await _repo.CrearAsync(entidad);
            _logger.LogInformation("ProductoPresentacion creada: Variante={VarianteId} Tipo={Tipo}",
                creada.VarianteId, tipo.Nombre);

            var todas = await _repo.ObtenerPorVarianteAsync(creada.VarianteId);
            return MapearConEquivalencia(creada, todas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear presentación");
            throw;
        }
    }

    public async Task<ProductoPresentacionDto> ActualizarAsync(ActualizarProductoPresentacionDto dto)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(dto.Id);
            ValidacionEntidad.VerificarActivo(existente, "ProductoPresentacion");

            var tipo = await _tipoRepo.ObtenerPorIdAsync(dto.TipoPresentacionId);
            if (tipo == null || !tipo.IsActive)
                throw new InvalidOperationException($"TipoPresentacion {dto.TipoPresentacionId} no existe o no está activo.");

            if (await _repo.ExisteCombinacionAsync(existente!.VarianteId, dto.TipoPresentacionId, dto.Id))
                throw new InvalidOperationException($"La variante ya tiene una presentación de tipo '{tipo.Nombre}'.");

            if (dto.EsDefaultReporte && await _repo.ExisteDefaultReporteAsync(existente.VarianteId, dto.Id))
                throw new InvalidOperationException("La variante ya tiene una presentación marcada como default de reporte.");

            // Unidad no puede cambiar CantidadDePadre ni PadreId
            if (await _tipoRepo.EsUnidadAsync(dto.TipoPresentacionId))
            {
                dto.CantidadDePadre = 1;
                dto.PresentacionPadreId = null;
            }
            else
            {
                if (dto.CantidadDePadre <= 1)
                    throw new InvalidOperationException("CantidadDePadre debe ser mayor a 1 para presentaciones que no son Unidad.");
                if (dto.PresentacionPadreId == null)
                    throw new InvalidOperationException("Debe indicar PresentacionPadreId para presentaciones que no son Unidad.");
            }

            existente.TipoPresentacionId = dto.TipoPresentacionId;
            existente.NombrePersonalizado = dto.NombrePersonalizado;
            existente.CantidadDePadre = dto.CantidadDePadre;
            existente.PresentacionPadreId = dto.PresentacionPadreId;
            existente.EsDefaultReporte = dto.EsDefaultReporte;
            existente.CodigoBarras = dto.CodigoBarras;
            existente.IsActive = dto.IsActive;

            var actualizada = await _repo.ActualizarAsync(existente);
            _logger.LogInformation("ProductoPresentacion actualizada: {Id}", actualizada.Id);

            var todas = await _repo.ObtenerPorVarianteAsync(actualizada.VarianteId);
            return MapearConEquivalencia(actualizada, todas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar presentación {Id}", dto.Id);
            throw;
        }
    }

    public async Task EliminarAsync(int id)
    {
        try
        {
            var existente = await _repo.ObtenerPorIdAsync(id);
            ValidacionEntidad.VerificarActivo(existente, "ProductoPresentacion");

            // No se puede eliminar Unidad si tiene hijas
            if (existente!.PresentacionPadreId == null)
            {
                var todas = await _repo.ObtenerPorVarianteAsync(existente.VarianteId);
                var tieneHijas = todas.Any(x => x.PresentacionPadreId == id && x.IsActive);
                if (tieneHijas)
                    throw new InvalidOperationException("No se puede eliminar la Unidad mientras tenga presentaciones hijas.");
            }

            await _repo.EliminarAsync(id);
            _logger.LogInformation("ProductoPresentacion eliminada: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar presentación {Id}", id);
            throw;
        }
    }

    // Convierte stockEnUnidades al desglose según EsDefaultReporte
    public async Task<StockConversionDto> ConvertirStockAsync(int varianteId, int stockEnUnidades)
    {
        try
        {
            var presentaciones = await _repo.ObtenerPorVarianteAsync(varianteId);
            if (!presentaciones.Any())
                return new StockConversionDto { VarianteId = varianteId, StockEnUnidades = stockEnUnidades };

            // Calcular equivalencia absoluta en unidades para cada presentación
            var equivalencias = CalcularEquivalencias(presentaciones);

            // Encontrar el default — si no hay, usar Unidad
            var defaultPres = presentaciones.FirstOrDefault(x => x.EsDefaultReporte)
                           ?? presentaciones.OrderBy(x => x.TipoPresentacion!.Orden).First();

            // Obtener cadena desde default hacia abajo hasta Unidad ordenada desc por equivalencia
            var cadena = equivalencias
                .Where(kvp => kvp.Value <= equivalencias[defaultPres.Id])
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => presentaciones.First(p => p.Id == kvp.Key))
                .ToList();

            // Desglozar
            var desglose = new List<StockPresentacionDto>();
            var resto = stockEnUnidades;

            foreach (var pres in cadena)
            {
                var equiv = equivalencias[pres.Id];
                var cantidad = resto / equiv;
                resto = resto % equiv;

                desglose.Add(new StockPresentacionDto
                {
                    NombreMostrar = pres.NombrePersonalizado ?? pres.TipoPresentacion!.Nombre,
                    Cantidad = cantidad,
                    EquivalenciaEnUnidades = equiv,
                    EsDefault = pres.EsDefaultReporte
                });
            }

            return new StockConversionDto
            {
                VarianteId = varianteId,
                StockEnUnidades = stockEnUnidades,
                Desglose = desglose
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al convertir stock de variante {VarianteId}", varianteId);
            throw;
        }
    }

    // ─── Métodos privados ────────────────────────────────────────────────────

    // Recorre la cadena PadreId → calcula cuántas unidades equivale cada presentación
    private static Dictionary<int, int> CalcularEquivalencias(List<ProductoPresentacion> presentaciones)
    {
        var resultado = new Dictionary<int, int>();

        foreach (var pres in presentaciones)
        {
            resultado[pres.Id] = CalcularEquivalenciaRecursiva(pres, presentaciones);
        }

        return resultado;
    }

    private static int CalcularEquivalenciaRecursiva(
        ProductoPresentacion pres,
        List<ProductoPresentacion> todas)
    {
        // Unidad: PadreId null → equivalencia = 1
        if (pres.PresentacionPadreId == null)
            return 1;

        var padre = todas.FirstOrDefault(x => x.Id == pres.PresentacionPadreId);
        if (padre == null)
            return pres.CantidadDePadre;

        // Equivalencia = CantidadDePadre × equivalencia del padre
        return pres.CantidadDePadre * CalcularEquivalenciaRecursiva(padre, todas);
    }

    private static ProductoPresentacionDto MapearConEquivalencia(
        ProductoPresentacion entidad,
        List<ProductoPresentacion> todas)
    {
        var equivalencias = CalcularEquivalencias(todas);

        return new ProductoPresentacionDto
        {
            Id = entidad.Id,
            VarianteId = entidad.VarianteId,
            TipoPresentacionId = entidad.TipoPresentacionId,
            TipoNombre = entidad.TipoPresentacion?.Nombre ?? string.Empty,
            NombrePersonalizado = entidad.NombrePersonalizado ?? string.Empty,
            NombreMostrar = entidad.NombrePersonalizado ?? entidad.TipoPresentacion?.Nombre ?? string.Empty,
            CantidadDePadre = entidad.CantidadDePadre,
            PresentacionPadreId = entidad.PresentacionPadreId,
            EquivalenciaEnUnidades = equivalencias.TryGetValue(entidad.Id, out var eq) ? eq : 1,
            EsDefaultReporte = entidad.EsDefaultReporte,
            CodigoBarras = entidad.CodigoBarras,
            Orden = entidad.TipoPresentacion?.Orden ?? 0,
            IsActive = entidad.IsActive,
            CreatedAt = entidad.CreatedAt
        };
    }
}