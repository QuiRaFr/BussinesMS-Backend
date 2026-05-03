---
name: crear-validaciones
description: Agrega o corrige el sistema de validaciones de negocio. Usa constraint tipado IEntidadActivable en lugar de reflexión. Excepciones de dominio tipadas. Sin throw new Exception genérico.
---

# Skill: crear-validaciones

## ⚠️ REGLAS CRÍTICAS

### REGLA 1 — Sin reflexión en ValidacionEntidad
`VerificarActivo` NUNCA usa `GetProperty("IsActive")`. Usa constraint `IEntidadActivable`.

### REGLA 2 — Sin `throw new Exception` genérico
Siempre usar excepciones del namespace `BussinesMS.Dominio.Excepciones`.
El middleware `ErrorHandlingMiddleware` las convierte al código HTTP correcto automáticamente.

### REGLA 3 — Repositorio nunca retorna DTOs
Métodos de validación como `ExisteNombreAsync` pertenecen al repositorio y retornan `bool`. Correcto.
Pero `ObtenerRaicesAsync`, `ObtenerSubcategoriasAsync`, etc. deben retornar entidades, no DTOs.

---

## Paso 0 — Verificar IEntidadActivable existe

**Ubicación**: `src/BussinesMS.Dominio/Interfaces/IEntidadActivable.cs`

Si NO existe, crearlo:
```csharp
namespace BussinesMS.Dominio.Interfaces;

public interface IEntidadActivable
{
    bool IsActive { get; set; }
}
```

Luego verificar que `EntidadBase` implementa `IEntidadActivable`:
```csharp
// src/BussinesMS.Dominio/Entidades/Compartido/EntidadBase.cs
using BussinesMS.Dominio.Interfaces;

public abstract class EntidadBase : IEntidadActivable
{
    public bool IsActive { get; set; } = true;
    // ... resto de propiedades
}
```

---

## Paso 1 — ValidacionEntidad.cs (versión correcta completa)

**Ubicación**: `src/BussinesMS.Aplicacion/Helpers/ValidacionEntidad.cs`

```csharp
using BussinesMS.Dominio.Entidades.Sistema;
using BussinesMS.Dominio.Excepciones;
using BussinesMS.Dominio.Interfaces;

namespace BussinesMS.Aplicacion.Helpers;

public static class ValidacionEntidad
{
    /// <summary>
    /// Verifica que la entidad no sea null. Lanza EntidadNoEncontradaException si es null.
    /// </summary>
    public static void VerificarExiste<T>(T? entidad, string nombre) where T : class
    {
        if (entidad == null)
            throw new EntidadNoEncontradaException(nombre, 0);
    }

    /// <summary>
    /// Verifica que la entidad existe Y está activa.
    /// ✅ Usa constraint IEntidadActivable — sin reflexión.
    /// </summary>
    public static void VerificarActivo<T>(T? entidad, string nombre)
        where T : class, IEntidadActivable
    {
        if (entidad == null)
            throw new EntidadNoEncontradaException(nombre, 0);

        if (!entidad.IsActive)
            throw new ValidacionException($"{nombre} no está activo");
    }

    /// <summary>
    /// Verifica que no exista un registro duplicado.
    /// Lanza EntidadDuplicadaException si existe=true.
    /// </summary>
    public static void VerificarNoDuplicado(bool existe, string nombreEntidad, string nombre)
    {
        if (existe)
            throw new EntidadDuplicadaException(nombreEntidad, nombre);
    }

    /// <summary>
    /// Específico para categorías jerárquicas.
    /// Verifica que la categoría padre existe y es categoría raíz (no subcategoría).
    /// </summary>
    public static void VerificarCategoriaRaiz(Categoria? categoria, int? id)
    {
        if (categoria == null)
            throw new EntidadNoEncontradaException("Categoría", id ?? 0);

        if (categoria.ParentId != null)
            throw new CategoriaInvalidaException(
                "No se puede crear subcategoría de una subcategoría. Seleccione una categoría raíz.");
    }
}
```

---

## Paso 2 — Excepciones de Dominio (verificar que existen)

**Ubicación**: `src/BussinesMS.Dominio/Excepciones/ExcepcionesDominio.cs`

Si no existen o están incompletas:

```csharp
namespace BussinesMS.Dominio.Excepciones;

/// <summary>Base para excepciones de dominio con código HTTP y código de error.</summary>
public class ExcepcionDominio : Exception
{
    public int CodigoHttp { get; set; } = 400;
    public string CodigoError { get; set; } = "ERROR";

    public ExcepcionDominio(string mensaje, int codigoHttp = 400, string codigoError = "ERROR")
        : base(mensaje)
    {
        CodigoHttp = codigoHttp;
        CodigoError = codigoError;
    }
}

/// <summary>404 — Entidad no encontrada por ID.</summary>
public class EntidadNoEncontradaException : ExcepcionDominio
{
    public EntidadNoEncontradaException(string nombreEntidad, int id)
        : base($"{nombreEntidad} con ID {id} no encontrada", 404, "ENTIDAD_NO_ENCONTRADA") { }
}

/// <summary>400 — Validación de negocio fallida.</summary>
public class ValidacionException : ExcepcionDominio
{
    public ValidacionException(string mensaje)
        : base(mensaje, 400, "VALIDACION_ERROR") { }
}

/// <summary>409 — Ya existe un registro con ese nombre/campo.</summary>
public class EntidadDuplicadaException : ExcepcionDominio
{
    public EntidadDuplicadaException(string nombreEntidad, string nombre)
        : base($"Ya existe {nombreEntidad} con el nombre '{nombre}'", 409, "ENTIDAD_DUPLICADA") { }
}

/// <summary>400 — Jerarquía de categoría inválida.</summary>
public class CategoriaInvalidaException : ExcepcionDominio
{
    public CategoriaInvalidaException(string mensaje)
        : base(mensaje, 400, "CATEGORIA_INVALIDA") { }
}
```

---

## Paso 3 — ErrorHandlingMiddleware (verificar que atrapa ExcepcionDominio)

**Ubicación**: `src/BussinesMS.API/Middlewares/ErrorHandlingMiddleware.cs`

Verificar que el middleware tiene el caso `ExcepcionDominio`:

```csharp
switch (exception)
{
    case ExcepcionDominio exDominio:
        statusCode = exDominio.CodigoHttp;
        errorCode = exDominio.CodigoError;
        mensaje = exDominio.Message;
        break;
    // ... otros casos
}
```

Si falta ese case, agrégarlo. Esto hace que:
- `EntidadNoEncontradaException` → retorna HTTP 404
- `EntidadDuplicadaException` → retorna HTTP 409
- `ValidacionException` → retorna HTTP 400
- `CategoriaInvalidaException` → retorna HTTP 400

---

## Paso 4 — Agregar validaciones al Repositorio

Para cada módulo que necesite validación de duplicados:

### En la Interfaz (I[Entidad]Repository.cs)
```csharp
// Validación de nombre único
Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null);
```

### En la Implementación ([Entidad]Repository.cs)
```csharp
public async Task<bool> ExisteNombreAsync(string nombre, int? excludeId = null)
{
    var query = _context.[Entidades]
        .Where(x => x.Nombre.ToLower() == nombre.ToLower() && x.IsActive);

    if (excludeId.HasValue)
        query = query.Where(x => x.Id != excludeId.Value);

    return await query.AnyAsync();
}
```

Para campo único distinto de Nombre (ej: Codigo, Nit):
```csharp
Task<bool> ExisteCodigoAsync(string codigo, int? excludeId = null);

public async Task<bool> ExisteCodigoAsync(string codigo, int? excludeId = null)
{
    var query = _context.[Entidades]
        .Where(x => x.Codigo.ToLower() == codigo.ToLower() && x.IsActive);

    if (excludeId.HasValue)
        query = query.Where(x => x.Id != excludeId.Value);

    return await query.AnyAsync();
}
```

---

## Paso 5 — Agregar validaciones al Service

### En CrearAsync
```csharp
public async Task<[Entidad]Dto> CrearAsync(Crear[Entidad]Dto dto)
{
    try
    {
        // 1. Normalizar campos opcionales (0 → null)
        if (dto.CampoOpcional.HasValue && dto.CampoOpcional.Value == 0)
            dto.CampoOpcional = null;

        // 2. Validar FK si hay referencia a otra entidad
        if (dto.EntidadPadreId.HasValue)
        {
            var padre = await _otraRepo.ObtenerPorIdAsync(dto.EntidadPadreId.Value);
            ValidacionEntidad.VerificarExiste(padre, "EntidadPadre");
        }

        // 3. Validar duplicado
        ValidacionEntidad.VerificarNoDuplicado(
            await _repo.ExisteNombreAsync(dto.Nombre),
            "[Entidad]", dto.Nombre);

        // 4. Crear
        var entidad = _mapper.Map<[Entidad]>(dto);
        var creada = await _repo.CrearAsync(entidad);

        _logger.LogInformation("[Entidad] creada: {Nombre}", creada.Nombre);
        return _mapper.Map<[Entidad]Dto>(creada);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al crear [Entidad]");
        throw;  // ✅ re-throw — el middleware lo convierte al JSON correcto
    }
}
```

### En ActualizarAsync
```csharp
public async Task<[Entidad]Dto> ActualizarAsync(Actualizar[Entidad]Dto dto)
{
    try
    {
        // 1. Normalizar
        if (dto.CampoOpcional.HasValue && dto.CampoOpcional.Value == 0)
            dto.CampoOpcional = null;

        // 2. Verificar que existe y está activa
        var existente = await _repo.ObtenerPorIdAsync(dto.Id);
        ValidacionEntidad.VerificarActivo(existente, "[Entidad]");  // type-safe

        // 3. Validar FK si aplica
        if (dto.EntidadPadreId.HasValue)
        {
            var padre = await _otraRepo.ObtenerPorIdAsync(dto.EntidadPadreId.Value);
            ValidacionEntidad.VerificarExiste(padre, "EntidadPadre");
        }

        // 4. Validar duplicado excluyendo el propio registro
        ValidacionEntidad.VerificarNoDuplicado(
            await _repo.ExisteNombreAsync(dto.Nombre, dto.Id),
            "[Entidad]", dto.Nombre);

        // 5. Actualizar propiedades
        existente!.Nombre = dto.Nombre;
        // ...otras propiedades

        var actualizada = await _repo.ActualizarAsync(existente);

        _logger.LogInformation("[Entidad] actualizada: {Nombre}", actualizada.Nombre);
        return _mapper.Map<[Entidad]Dto>(actualizada);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al actualizar [Entidad] {Id}", dto.Id);
        throw;
    }
}
```

### En EliminarAsync
```csharp
public async Task EliminarAsync(int id)
{
    try
    {
        var existente = await _repo.ObtenerPorIdAsync(id);
        ValidacionEntidad.VerificarActivo(existente, "[Entidad]");  // type-safe

        await _repo.EliminarAsync(id);
        _logger.LogInformation("[Entidad] eliminada: {Id}", id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar [Entidad] {Id}", id);
        throw;
    }
}
```

---

## Tabla de Excepciones y respuestas HTTP

| Excepción | HTTP | errorCode | Cuándo usar |
|-----------|------|-----------|-------------|
| `EntidadNoEncontradaException` | 404 | ENTIDAD_NO_ENCONTRADA | ID no existe en BD |
| `ValidacionException` | 400 | VALIDACION_ERROR | Campo inválido, entidad inactiva |
| `EntidadDuplicadaException` | 409 | ENTIDAD_DUPLICADA | Nombre/campo ya existe |
| `CategoriaInvalidaException` | 400 | CATEGORIA_INVALIDA | Subcategoría de subcategoría |
| `Exception` no controlada | 500 | ERROR_INTERNO | Error inesperado del sistema |

---

## Respuestas JSON de ejemplo

```json
// 404 - No encontrada
{ "success": false, "message": "Categoría con ID 99 no encontrada", "errorCode": "ENTIDAD_NO_ENCONTRADA" }

// 409 - Duplicado
{ "success": false, "message": "Ya existe categoría con el nombre 'Snacks'", "errorCode": "ENTIDAD_DUPLICADA" }

// 400 - Validación
{ "success": false, "message": "Categoría no está activo", "errorCode": "VALIDACION_ERROR" }

// 400 - Categoría inválida
{ "success": false, "message": "No se puede crear subcategoría de una subcategoría.", "errorCode": "CATEGORIA_INVALIDA" }

// 500 - Error interno
{ "success": false, "message": "Error interno del servidor", "errorCode": "ERROR_INTERNO" }
```

---

## Checklist final

- [ ] `IEntidadActivable` existe en `BussinesMS.Dominio/Interfaces/`
- [ ] `EntidadBase` implementa `IEntidadActivable`
- [ ] `ValidacionEntidad.VerificarActivo` usa constraint `where T : class, IEntidadActivable` (sin reflexión)
- [ ] `ExcepcionesDominio.cs` tiene las 5 excepciones tipadas
- [ ] `ErrorHandlingMiddleware` atrapa `ExcepcionDominio` y retorna el código HTTP correcto
- [ ] `Program.cs` tiene `app.UseErrorHandling()` antes de `UseAuthorization()`
- [ ] Ningún Service lanza `throw new Exception(...)` genérico
- [ ] Ningún repositorio retorna DTOs