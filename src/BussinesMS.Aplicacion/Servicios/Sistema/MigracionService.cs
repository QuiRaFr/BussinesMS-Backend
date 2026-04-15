using BussinesMS.Aplicacion.DTOs.Sistema.Migracion;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using BussinesMS.Dominio.Entidades.Sistema;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BussinesMS.Aplicacion.Servicios.Sistema;

public class MigracionService : IMigracionService
{
    private readonly ICategoriaRepository _categoriaRepo;
    private readonly IFabricanteRepository _fabricanteRepo;
    private readonly ILogger<MigracionService> _logger;

    public MigracionService(
        ICategoriaRepository categoriaRepo,
        IFabricanteRepository fabricanteRepo,
        ILogger<MigracionService> logger)
    {
        _categoriaRepo = categoriaRepo;
        _fabricanteRepo = fabricanteRepo;
        _logger = logger;
    }

    public async Task<ResultadoMigracionDto> MigrarDatosDesdeCsvAsync(IFormFile archivo)
    {
        var resultado = new ResultadoMigracionDto();

        try
        {
            _logger.LogInformation("Iniciando lectura del archivo CSV...");
            
            using var reader = new StreamReader(archivo.OpenReadStream());
            var contenidoCompleto = await reader.ReadToEndAsync();
            var lineas = contenidoCompleto.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (lineas.Length == 0)
            {
                resultado.Success = false;
                resultado.Mensaje = "El archivo está vacío";
                return resultado;
            }

            _logger.LogInformation($"Total de líneas en archivo: {lineas.Length}");
            resultado.FilasProcesadas = lineas.Length - 1;

            // ============================================================
            // EXTRAER DATOS ÚNICOS DEL CSV
            // ============================================================
            var categoriasUnicas = new HashSet<string>();
            var subcategoriasUnicas = new HashSet<(string Categoria, string Subcategoria)>();
            var fabricantesUnicos = new HashSet<string>();
            var saboresUnicos = new HashSet<string>();
            var tamaniosUnicos = new HashSet<string>();
            var presentacionesUnicas = new HashSet<string>();

            for (int i = 1; i < lineas.Length; i++)
            {
                var linea = lineas[i].Trim();
                if (string.IsNullOrWhiteSpace(linea)) continue;
                if (linea.Replace(";", "").Replace(" ", "").Length == 0) continue;

                var partes = linea.Split(';');

                // Categorías raíz
                if (partes.Length > 1 && !string.IsNullOrWhiteSpace(partes[1]))
                    categoriasUnicas.Add(partes[1].Trim().ToUpper());

                // Subcategorías
                if (partes.Length > 2 && !string.IsNullOrWhiteSpace(partes[1]) && !string.IsNullOrWhiteSpace(partes[2]))
                    subcategoriasUnicas.Add((partes[1].Trim().ToUpper(), partes[2].Trim().ToUpper()));

                // Fabricantes
                if (partes.Length > 6 && !string.IsNullOrWhiteSpace(partes[6]))
                    fabricantesUnicos.Add(partes[6].Trim().ToUpper());

                // Sabores/Descripciones
                if (partes.Length > 4 && !string.IsNullOrWhiteSpace(partes[4]))
                    saboresUnicos.Add(partes[4].Trim().ToUpper());

                // Tamaños
                if (partes.Length > 5 && !string.IsNullOrWhiteSpace(partes[5]))
                    tamaniosUnicos.Add(partes[5].Trim().ToUpper());

                // Presentaciones (unidad, display, caja)
                if (partes.Length > 7 && !string.IsNullOrWhiteSpace(partes[7]))
                    presentacionesUnicas.Add("UNIDAD");
                if (partes.Length > 8 && !string.IsNullOrWhiteSpace(partes[8]))
                    presentacionesUnicas.Add("DISPLAY");
                if (partes.Length > 9 && !string.IsNullOrWhiteSpace(partes[9]))
                    presentacionesUnicas.Add("CAJA");
            }

            _logger.LogInformation($"Datos únicos - Categorías: {categoriasUnicas.Count}, Subcategorías: {subcategoriasUnicas.Count}, Fabricantes: {fabricantesUnicos.Count}, Sabores: {saboresUnicos.Count}, Tamaños: {tamaniosUnicos.Count}, Presentaciones: {presentacionesUnicas.Count}");

            // ============================================================
            // MIGRACIÓN DE CATEGORÍAS
            // ============================================================
            var categoriasCreadas = new Dictionary<string, int>();

            foreach (var cat in categoriasUnicas)
            {
                var existente = await _categoriaRepo.AsQueryable()
                    .FirstOrDefaultAsync(c => c.Nombre.ToUpper() == cat && c.ParentId == null);

                if (existente != null)
                {
                    categoriasCreadas[cat] = existente.Id;
                    resultado.Omitidas++;
                }
                else
                {
                    var nueva = new Categoria
                    {
                        Nombre = cat,
                        ParentId = null,
                        CreatedAt = DateTime.UtcNow,
                        CreatedByUsuarioId = 1,
                        IsActive = true
                    };
                    await _categoriaRepo.CrearAsync(nueva);
                    categoriasCreadas[cat] = nueva.Id;
                    resultado.CategoriasCreadas.Add(cat);
                    resultado.Creadas++;
                }
            }

            foreach (var (cat, sub) in subcategoriasUnicas)
            {
                if (!categoriasCreadas.TryGetValue(cat, out var catId)) continue;

                var existente = await _categoriaRepo.AsQueryable()
                    .FirstOrDefaultAsync(c => c.ParentId == catId && c.Nombre.ToUpper() == sub);

                if (existente != null)
                {
                    resultado.Omitidas++;
                }
                else
                {
                    var nuevaSub = new Categoria
                    {
                        Nombre = sub,
                        ParentId = catId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedByUsuarioId = 1,
                        IsActive = true
                    };
                    await _categoriaRepo.CrearAsync(nuevaSub);
                    resultado.SubcategoriasCreadas.Add(sub);
                    resultado.Creadas++;
                }
            }

            _logger.LogInformation($"Categorias procesadas: {resultado.CategoriasCreadas.Count} creadas, {resultado.SubcategoriasCreadas.Count} subcategorias");

            // ============================================================
            // MIGRACIÓN DE FABRICANTES
            // ============================================================
            foreach (var fab in fabricantesUnicos)
            {
                if (string.IsNullOrWhiteSpace(fab)) continue;

                var existente = await _fabricanteRepo.AsQueryable()
                    .FirstOrDefaultAsync(f => f.Nombre.ToUpper() == fab);

                if (existente != null)
                {
                    resultado.Omitidas++;
                }
                else
                {
                    var nuevo = new Fabricante
                    {
                        Nombre = fab,
                        CreatedAt = DateTime.UtcNow,
                        CreatedByUsuarioId = 1,
                        IsActive = true
                    };
                    await _fabricanteRepo.CrearAsync(nuevo);
                    resultado.FabricantesCreados.Add(fab);
                    resultado.Creadas++;
                }
            }

            _logger.LogInformation($"Fabricantes procesados: {resultado.FabricantesCreados.Count} creados");

            // ============================================================
            // RESULTADO FINAL
            // ============================================================
            resultado.Success = true;
            resultado.Mensaje = $"Migración completada. " +
                $"Categorías: {resultado.CategoriasCreadas.Count}, " +
                $"Subcategorías: {resultado.SubcategoriasCreadas.Count}, " +
                $"Fabricantes: {resultado.FabricantesCreados.Count}, " +
                $"Omitidos (ya existían): {resultado.Omitidas}";
            
            _logger.LogInformation("Migración completada: {Mensaje}", resultado.Mensaje);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar CSV: {Message}", ex.Message);
            resultado.Success = false;
            resultado.Mensaje = $"Error: {ex.Message}";
        }

        return resultado;
    }
}