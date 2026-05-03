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
    private readonly IDescripcionSaborRepository _saborRepo;
    private readonly IDescripcionTamanioRepository _tamanioRepo;
    private readonly ITipoPresentacionRepository _presentacionRepo;
    private readonly IProductoRepository _productoRepo;
    private readonly IProductoVarianteRepository _varianteRepo;
    private readonly ILogger<MigracionService> _logger;

    public MigracionService(
        ICategoriaRepository categoriaRepo,
        IFabricanteRepository fabricanteRepo,
        IDescripcionSaborRepository saborRepo,
        IDescripcionTamanioRepository tamanioRepo,
        ITipoPresentacionRepository presentacionRepo,
        IProductoRepository productoRepo,
        IProductoVarianteRepository varianteRepo,
        ILogger<MigracionService> logger)
    {
        _categoriaRepo = categoriaRepo;
        _fabricanteRepo = fabricanteRepo;
        _saborRepo = saborRepo;
        _tamanioRepo = tamanioRepo;
        _presentacionRepo = presentacionRepo;
        _productoRepo = productoRepo;
        _varianteRepo = varianteRepo;
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
            var productosUnicos = new HashSet<(string Nombre, string? CodigoBarras, string Categoria, string Fabricante)>();

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

                // Productos (nombre + código de barras + categoría + fabricante)
                if (partes.Length > 3 && !string.IsNullOrWhiteSpace(partes[3]))
                {
                    var nombreProd = partes[3].Trim();
                    var codigoBarras = partes.Length > 0 ? partes[0].Trim() : null;
                    var categoria = partes.Length > 1 ? partes[1].Trim() : "";
                    var fabricante = partes.Length > 6 ? partes[6].Trim() : "";

                    if (!string.IsNullOrWhiteSpace(nombreProd))
                        productosUnicos.Add((nombreProd, codigoBarras, categoria, fabricante));
                }
            }

            _logger.LogInformation($"Datos únicos - Categorías: {categoriasUnicas.Count}, Subcategorías: {subcategoriasUnicas.Count}, Fabricantes: {fabricantesUnicos.Count}, Sabores: {saboresUnicos.Count}, Tamaños: {tamaniosUnicos.Count}, Presentaciones: {presentacionesUnicas.Count}, Productos: {productosUnicos.Count}");

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
            var fabricantesCreados = new Dictionary<string, int>();

            foreach (var fab in fabricantesUnicos)
            {
                if (string.IsNullOrWhiteSpace(fab)) continue;

                var existente = await _fabricanteRepo.AsQueryable()
                    .FirstOrDefaultAsync(f => f.Nombre.ToUpper() == fab);

                if (existente != null)
                {
                    fabricantesCreados[fab.ToUpper()] = existente.Id;
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
                    fabricantesCreados[fab.ToUpper()] = nuevo.Id;
                    resultado.FabricantesCreados.Add(fab);
                    resultado.Creadas++;
                }
            }

            _logger.LogInformation($"Fabricantes procesados: {resultado.FabricantesCreados.Count} creados");

            // ============================================================
            // MIGRACIÓN DE SABORES
            // ============================================================
            foreach (var sabor in saboresUnicos)
            {
                if (string.IsNullOrWhiteSpace(sabor)) continue;

                var existente = await _saborRepo.AsQueryable()
                    .FirstOrDefaultAsync(s => s.Nombre.ToUpper() == sabor);

                if (existente != null)
                {
                    resultado.Omitidas++;
                }
                else
                {
                    var nuevo = new DescripcionSabor
                    {
                        Nombre = sabor,
                        CreatedAt = DateTime.UtcNow,
                        CreatedByUsuarioId = 1,
                        IsActive = true
                    };
                    await _saborRepo.CrearAsync(nuevo);
                    resultado.SaboresCreados.Add(sabor);
                    resultado.Creadas++;
                }
            }

            _logger.LogInformation($"Sabores procesados: {resultado.SaboresCreados.Count} creados");

            // ============================================================
            // MIGRACIÓN DE TAMAÑOS
            // ============================================================
            foreach (var tamanio in tamaniosUnicos)
            {
                if (string.IsNullOrWhiteSpace(tamanio)) continue;

                var existente = await _tamanioRepo.AsQueryable()
                    .FirstOrDefaultAsync(t => t.Nombre.ToUpper() == tamanio);

                if (existente != null)
                {
                    resultado.Omitidas++;
                }
                else
                {
                    var nuevo = new DescripcionTamanio
                    {
                        Nombre = tamanio,
                        CreatedAt = DateTime.UtcNow,
                        CreatedByUsuarioId = 1,
                        IsActive = true
                    };
                    await _tamanioRepo.CrearAsync(nuevo);
                    resultado.TamaniosCreados.Add(tamanio);
                    resultado.Creadas++;
                }
            }

            _logger.LogInformation($"Tamaños procesados: {resultado.TamaniosCreados.Count} creados");

            // ============================================================
            // MIGRACIÓN DE TIPOS DE PRESENTACIÓN
            // ============================================================
            var factoresPresentacion = new Dictionary<string, int>
            {
                { "UNIDAD", 1 },
                { "DISPLAY", 12 },
                { "CAJA", 6 }
            };

            foreach (var presentacion in presentacionesUnicas)
            {
                if (string.IsNullOrWhiteSpace(presentacion)) continue;
                if (!factoresPresentacion.TryGetValue(presentacion.ToUpper(), out var factor)) continue;

                var existente = await _presentacionRepo.AsQueryable()
                    .FirstOrDefaultAsync(p => p.Nombre.ToUpper() == presentacion.ToUpper());

                if (existente != null)
                {
                    resultado.Omitidas++;
                }
                else
                {
                    var nuevo = new TipoPresentacion
                    {
                        Nombre = presentacion.ToUpper() == "UNIDAD" ? "Unidad" :
                                 presentacion.ToUpper() == "DISPLAY" ? "Display" : "Caja",
                        Factor = factor,
                        CreatedAt = DateTime.UtcNow,
                        CreatedByUsuarioId = 1,
                        IsActive = true
                    };
                    await _presentacionRepo.CrearAsync(nuevo);
                    resultado.PresentacionesCreadas.Add(nuevo.Nombre);
                    resultado.Creadas++;
                }
            }

            _logger.LogInformation($"Presentaciones procesadas: {resultado.PresentacionesCreadas.Count} creadas");

            // ============================================================
            // MIGRACIÓN DE PRODUCTOS
            // ============================================================
            foreach (var (nombre, codigoBarras, categoria, fabricante) in productosUnicos)
            {
                if (string.IsNullOrWhiteSpace(nombre)) continue;

                var nombreUpper = nombre.ToUpper();

                var existente = await _productoRepo.AsQueryable()
                    .FirstOrDefaultAsync(p => p.Nombre.ToUpper() == nombreUpper);

                if (existente != null)
                {
                    resultado.Omitidas++;
                }
                else
                {
                    int catId = 0;
                    int fabId = 0;

                    if (!string.IsNullOrWhiteSpace(categoria) && categoriasCreadas.TryGetValue(categoria.ToUpper(), out var cId))
                        catId = cId;

                    if (!string.IsNullOrWhiteSpace(fabricante) && fabricantesCreados.TryGetValue(fabricante.ToUpper(), out var fId))
                        fabId = fId;

                    if (catId == 0 || fabId == 0)
                    {
                        resultado.Omitidas++;
                        continue;
                    }

                    var nuevo = new Producto
                    {
                        Nombre = nombre,
                        CategoriaId = catId,
                        FabricanteId = fabId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedByUsuarioId = 1,
                        IsActive = true
                    };
                    var creado = await _productoRepo.CrearAsync(nuevo);
                    
                    creado.CodigoInterno = $"PROD-{creado.Id:D5}";
                    await _productoRepo.ActualizarAsync(creado);
                    
                    resultado.ProductosCreados.Add(nombre);
                    resultado.Creadas++;
                }
            }

            _logger.LogInformation($"Productos procesados: {resultado.ProductosCreados.Count} creados");

            // ============================================================
            // MIGRACIÓN DE PRODUCTO VARIANTES
            // ============================================================
            var productosExistentes = await _productoRepo.ObtenerTodosAsync();
            var saboresExistentes = await _saborRepo.ObtenerTodosAsync();
            var tamaniosExistentes = await _tamanioRepo.ObtenerTodosAsync();

            var productoPorNombre = productosExistentes.ToDictionary(p => p.Nombre.ToUpper(), p => p.Id);
            var saborPorNombre = saboresExistentes.ToDictionary(s => s.Nombre.ToUpper(), s => s.Id);
            var tamanioPorNombre = tamaniosExistentes.ToDictionary(t => t.Nombre.ToUpper(), t => t.Id);

            var variantesProcesadas = new HashSet<(int ProductoId, int SaborId, int TamanioId)>();

            for (int i = 1; i < lineas.Length; i++)
            {
                var linea = lineas[i].Trim();
                if (string.IsNullOrWhiteSpace(linea)) continue;
                if (linea.Replace(";", "").Replace(" ", "").Length == 0) continue;

                var partes = linea.Split(';');
                if (partes.Length < 6) continue;

                var nombreProducto = partes[3].Trim();
                var saborNombre = partes[4].Trim();
                var tamanioNombre = partes[5].Trim();
                var codigoBarras = partes[0].Trim();

                if (string.IsNullOrWhiteSpace(nombreProducto)) continue;

                var nombreUpper = nombreProducto.ToUpper();

                if (!productoPorNombre.TryGetValue(nombreUpper, out var productoId))
                    continue;

                int saborId = 0;
                int tamanioId = 0;

                if (!string.IsNullOrWhiteSpace(saborNombre))
                    saborPorNombre.TryGetValue(saborNombre.ToUpper(), out saborId);

                if (!string.IsNullOrWhiteSpace(tamanioNombre))
                    tamanioPorNombre.TryGetValue(tamanioNombre.ToUpper(), out tamanioId);

                if (productoId == 0 || saborId == 0 || tamanioId == 0)
                {
                    resultado.Omitidas++;
                    continue;
                }

                var claveVariante = (productoId, saborId, tamanioId);
                if (variantesProcesadas.Contains(claveVariante))
                {
                    resultado.Omitidas++;
                    continue;
                }

                var existeCombinacion = await _varianteRepo.ExisteCombinacionAsync(productoId, saborId, tamanioId);
                if (existeCombinacion)
                {
                    variantesProcesadas.Add(claveVariante);
                    resultado.Omitidas++;
                    continue;
                }

                var nuevaVariante = new ProductoVariante
                {
                    ProductoId = productoId,
                    SaborId = saborId,
                    TamanioId = tamanioId,
                    CodigoBarras = string.IsNullOrWhiteSpace(codigoBarras) ? null : codigoBarras,
                    PrecioVentaActual = 0,
                    CodigoAlmacen = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUsuarioId = 1,
                    IsActive = true
                };

                await _varianteRepo.CrearAsync(nuevaVariante);
                variantesProcesadas.Add(claveVariante);
                resultado.ProductoVariantesCreados.Add($"{nombreProducto} - {saborNombre} - {tamanioNombre}");
                resultado.Creadas++;
            }

            _logger.LogInformation($"Producto variantes procesados: {resultado.ProductoVariantesCreados.Count} creados");

            // ============================================================
            // RESULTADO FINAL
            // ============================================================
            resultado.Success = true;
            resultado.Mensaje = $"Migración completada. " +
                $"Categorías: {resultado.CategoriasCreadas.Count}, " +
                $"Subcategorías: {resultado.SubcategoriasCreadas.Count}, " +
                $"Fabricantes: {resultado.FabricantesCreados.Count}, " +
                $"Sabores: {resultado.SaboresCreados.Count}, " +
                $"Tamaños: {resultado.TamaniosCreados.Count}, " +
                $"Presentaciones: {resultado.PresentacionesCreadas.Count}, " +
                $"Productos: {resultado.ProductosCreados.Count}, " +
                $"ProductoVariantes: {resultado.ProductoVariantesCreados.Count}, " +
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