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
    private readonly ILogger<MigracionService> _logger;

    public MigracionService(ICategoriaRepository categoriaRepo, ILogger<MigracionService> logger)
    {
        _categoriaRepo = categoriaRepo;
        _logger = logger;
    }

    public async Task<ResultadoMigracionDto> CargarCategoriasDesdeCsvAsync(IFormFile archivo)
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

            // ============================================================
            // PASO 1: Build in-memory listas de categorías y subcategorías
            // ============================================================
            var listaCategorias = new List<string>();
            var listaSubcategorias = new List<(string Categoria, string Subcategoria)>();
            var categoriasUnicas = new HashSet<string>();
            var subcategoriasUnicas = new HashSet<string>();

            for (int i = 1; i < lineas.Length; i++)
            {
                var linea = lineas[i].Trim();
                if (string.IsNullOrWhiteSpace(linea)) continue;
                if (linea.Replace(";", "").Replace(" ", "").Length == 0) continue;

                var partes = linea.Split(';');
                if (partes.Length >= 3)
                {
                    var cat = partes.Length > 1 ? partes[1].Trim() : "";
                    var sub = partes.Length > 2 ? partes[2].Trim() : "";

                    if (!string.IsNullOrWhiteSpace(cat))
                    {
                        var catUpper = cat.ToUpper();
                        if (categoriasUnicas.Add(catUpper))
                        {
                            listaCategorias.Add(catUpper);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(cat) && !string.IsNullOrWhiteSpace(sub))
                    {
                        var catUpper = cat.ToUpper();
                        var subUpper = sub.ToUpper();
                        var clave = $"{catUpper}|{subUpper}";
                        if (subcategoriasUnicas.Add(clave))
                        {
                            listaSubcategorias.Add((catUpper, subUpper));
                        }
                    }
                }
            }

            resultado.FilasProcesadas = listaCategorias.Count;
            _logger.LogInformation($"Categorías únicas del CSV: {listaCategorias.Count}");
            _logger.LogInformation($"Subcategorías únicas del CSV: {listaSubcategorias.Count}");

            // ============================================================
            // PASO 2: Procesar categorías - preguntar BD y crear si no existe
            // ============================================================
            var categoriasCreadas = new Dictionary<string, int>();

            foreach (var cat in listaCategorias)
            {
                var existente = await _categoriaRepo.AsQueryable()
                    .FirstOrDefaultAsync(c => c.Nombre.ToUpper() == cat);

                if (existente != null)
                {
                    categoriasCreadas[cat] = existente.Id;
                    resultado.Omitidas++;
                    _logger.LogInformation("Categoría ya existe: {Nombre}", cat);
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
                    _logger.LogInformation("Categoría creada: {Nombre}", cat);
                }
            }

            // ============================================================
            // PASO 3: Procesar subcategorías - preguntar BD y crear si no existe
            // ============================================================
            _logger.LogInformation("Procesando subcategorías...");

            foreach (var (cat, sub) in listaSubcategorias)
            {
                if (!categoriasCreadas.TryGetValue(cat, out var catId))
                {
                    _logger.LogWarning("No se encontró categoría padre: {Categoria}", cat);
                    continue;
                }

                var existente = await _categoriaRepo.AsQueryable()
                    .FirstOrDefaultAsync(c => c.ParentId == catId && c.Nombre.ToUpper() == sub);

                if (existente != null)
                {
                    _logger.LogInformation("Subcategoría ya existe: {Sub} -> {Categoria}", sub, cat);
                    continue;
                }

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
                _logger.LogInformation("Subcategoría creada: {Sub} -> {Categoria}", sub, cat);
            }

            resultado.Success = true;
            resultado.Mensaje = $"Proceso completado. Categorías: {resultado.CategoriasCreadas.Count}, Subcategorías: {resultado.SubcategoriasCreadas.Count}, Omitidas: {resultado.Omitidas}";
            
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