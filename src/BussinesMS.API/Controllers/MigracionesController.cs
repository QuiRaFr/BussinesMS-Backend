using BussinesMS.Aplicacion.DTOs.Sistema.Migracion;
using BussinesMS.Aplicacion.Interfaces.Sistema;
using BussinesMS.Dominio.Entidades.Sistema;
using BussinesMS.Infraestructura.Persistencia;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BussinesMS.API.Controllers;

[ApiController]
[Route("api/Sistema/[controller]")]
[Produces("application/json")]
public class MigracionesController : BaseController
{
    private readonly IMigracionService _servicio;
    private readonly SistemaDbContext _context;

    public MigracionesController(IMigracionService servicio, SistemaDbContext context)
    {
        _servicio = servicio;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> MigrarDatos(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return RespuestaError("No se ha proporcionado ningún archivo");

        var resultado = await _servicio.MigrarDatosDesdeCsvAsync(archivo);
        return RespuestaOk(resultado);
    }

    [HttpDelete("LimpiarDatos")]
    public async Task<IActionResult> LimpiarDatos()
    {
        try
        {
            _context.ProductoPresentaciones.RemoveRange(_context.ProductoPresentaciones);
            _context.ProductoVariantes.RemoveRange(_context.ProductoVariantes);
            _context.Productos.RemoveRange(_context.Productos);
            _context.TipoPresentaciones.RemoveRange(_context.TipoPresentaciones);
            await _context.SaveChangesAsync();
            return RespuestaOk(new { success = true, message = "Datos eliminados correctamente" });
        }
        catch (Exception ex)
        {
            return RespuestaError($"Error al limpiar datos: {ex.Message}");
        }
    }

    [HttpPost("ActualizarVariantes")]
    public async Task<IActionResult> ActualizarVariantesDesdeCsv(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return RespuestaError("No se ha proporcionado ningún archivo");

        try
        {
            using var reader = new StreamReader(archivo.OpenReadStream());
            var contenido = await reader.ReadToEndAsync();
            var lineas = contenido.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (lineas.Length == 0)
                return RespuestaError("El archivo está vacío");

            var productos = await _context.Productos
                .Where(p => p.IsActive)
                .ToDictionaryAsync(p => p.Nombre.ToUpper(), p => p.Id);

            var sabores = await _context.DescripcionSabores
                .Where(s => s.IsActive)
                .ToDictionaryAsync(s => s.Nombre.ToUpper(), s => s.Id);

            var tamanios = await _context.DescripcionTamanios
                .Where(t => t.IsActive)
                .ToDictionaryAsync(t => t.Nombre.ToUpper(), t => t.Id);

            var variantes = await _context.ProductoVariantes
                .Include(v => v.Producto)
                .Include(v => v.Sabor)
                .Include(v => v.Tamanio)
                .Where(v => v.IsActive)
                .ToListAsync();

            int actualizadas = 0;
            int omitidas = 0;

            for (int i = 1; i < lineas.Length; i++)
            {
                var linea = lineas[i].Trim();
                if (string.IsNullOrWhiteSpace(linea)) continue;
                if (linea.Replace(";", "").Replace(" ", "").Length == 0) continue;

                var partes = linea.Split(';');
                if (partes.Length < 11) continue;

                var nombreProducto = partes[3].Trim();
                var saborNombre = partes[4].Trim();
                var tamanioNombre = partes[5].Trim();
                var unidadStr = partes[7].Trim();
                var displayStr = partes[8].Trim();
                var cajaStr = partes[9].Trim();
                var tipoVentaStr = partes.Length > 10 ? partes[10].Trim().ToLower() : "u";

                if (string.IsNullOrWhiteSpace(nombreProducto)) continue;
                if (string.IsNullOrWhiteSpace(saborNombre)) continue;
                if (string.IsNullOrWhiteSpace(tamanioNombre)) continue;

                var productoId = productos.GetValueOrDefault(nombreProducto.ToUpper(), 0);
                var saborId = sabores.GetValueOrDefault(saborNombre.ToUpper(), 0);
                var tamanioId = tamanios.GetValueOrDefault(tamanioNombre.ToUpper(), 0);

                if (productoId == 0 || saborId == 0 || tamanioId == 0)
                {
                    omitidas++;
                    continue;
                }

                var variante = variantes.FirstOrDefault(v =>
                    v.ProductoId == productoId &&
                    v.SaborId == saborId &&
                    v.TamanioId == tamanioId);

                if (variante == null)
                {
                    omitidas++;
                    continue;
                }

                int? unidad = string.IsNullOrWhiteSpace(unidadStr) ? null : int.TryParse(unidadStr, out var u) ? u : null;
                int? display = string.IsNullOrWhiteSpace(displayStr) ? null : int.TryParse(displayStr, out var d) ? d : null;
                int? caja = string.IsNullOrWhiteSpace(cajaStr) ? null : int.TryParse(cajaStr, out var c) ? c : null;

                variante.Unidad = unidad;
                variante.Display = display;
                variante.Caja = caja;
                variante.TipoVenta = string.IsNullOrWhiteSpace(tipoVentaStr) ? "u" : tipoVentaStr;

                actualizadas++;
            }

            await _context.SaveChangesAsync();

            return RespuestaOk(new
            {
                success = true,
                message = $"Variantes actualizadas: {actualizadas}, Omitidas: {omitidas}",
                actualizadas,
                omitidas
            });
        }
        catch (Exception ex)
        {
            return RespuestaError($"Error al actualizar variantes: {ex.Message}");
        }
    }
}