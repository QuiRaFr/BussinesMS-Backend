---
name: agregar-reactivacion-modulo
description: Agrega lógica de reactivación automática a un módulo CRUD existente que usa soft-delete. 
Usar cuando un módulo necesita que al crear un registro con nombre duplicado inactivo, 
se reactive automáticamente en lugar de lanzar error.
---

# Skill: agregar-reactivacion-modulo

## ¿Cuándo usar este skill?
Solo cuando el módulo cumple estas condiciones:
- Ya tiene soft-delete (IsActive)
- El repositorio tiene o puede tener ObtenerPorNombreAsync sin filtro IsActive
- El negocio requiere reactivar en lugar de rechazar duplicados inactivos

## Archivos a modificar (solo estos 3)

### 1 — Interfaz del Service
**Ubicación**: `src/BussinesMS.Aplicacion/Interfaces/[Sistema]/I[NombreEntidad]Service.cs`

Cambiar:
```csharp
// ❌ ANTES
Task<[NombreEntidad]Dto> CrearAsync(Crear[NombreEntidad]Dto dto);

// ✅ DESPUÉS
Task<([NombreEntidad]Dto Entidad, bool FueReactivada)> CrearAsync(Crear[NombreEntidad]Dto dto);
```

### 2 — Implementación del Service
**Ubicación**: `src/BussinesMS.Aplicacion/Servicios/[Sistema]/[NombreEntidad]Service.cs`

Cambiar el método `CrearAsync`:
```csharp
// ❌ ANTES
public async Task<[NombreEntidad]Dto> CrearAsync(Crear[NombreEntidad]Dto dto)
{
    try
    {
        ValidacionEntidad.VerificarNoDuplicado(
            await _repo.ExisteNombreAsync(dto.Nombre),
            "[NombreEntidad]", dto.Nombre);

        var entidad = _mapper.Map<[NombreEntidad]>(dto);
        var creada = await _repo.CrearAsync(entidad);
        _logger.LogInformation("[NombreEntidad] creada: {Nombre}", creada.Nombre);
        return _mapper.Map<[NombreEntidad]Dto>(creada);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al crear [NombreEntidad]");
        throw;
    }
}

// ✅ DESPUÉS
public async Task<([NombreEntidad]Dto Entidad, bool FueReactivada)> CrearAsync(Crear[NombreEntidad]Dto dto)
{
    try
    {
        var duplicado = await _repo.ObtenerPorNombreAsync(dto.Nombre);

        if (duplicado != null)
        {
            if (duplicado.IsActive)
                throw new InvalidOperationException($"El/La [NombreEntidad] '{dto.Nombre}' ya existe.");

            var reactivada = await _repo.ReactivarAsync(duplicado.Id);
            _logger.LogInformation("[NombreEntidad] reactivada: {Nombre}", reactivada.Nombre);
            return (_mapper.Map<[NombreEntidad]Dto>(reactivada), true);
        }

        var entidad = _mapper.Map<[NombreEntidad]>(dto);
        var creada = await _repo.CrearAsync(entidad);
        _logger.LogInformation("[NombreEntidad] creada: {Nombre}", creada.Nombre);
        return (_mapper.Map<[NombreEntidad]Dto>(creada), false);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al crear [NombreEntidad]");
        throw;
    }
}
```

### 3 — Controller
**Ubicación**: `src/BussinesMS.API/Controllers/[NombreEntidad]sController.cs`

Cambiar el método `Crear`:
```csharp
// ❌ ANTES
[HttpPost]
public async Task<IActionResult> Crear([FromBody] Crear[NombreEntidad]Dto dto)
{
    var resultado = await _servicio.CrearAsync(dto);
    return RespuestaCreado(resultado, "[NombreEntidad] creada exitosamente.");
}

// ✅ DESPUÉS
[HttpPost]
public async Task<IActionResult> Crear([FromBody] Crear[NombreEntidad]Dto dto)
{
    var (entidad, fueReactivada) = await _servicio.CrearAsync(dto);

    return fueReactivada
        ? RespuestaOk(new { mensaje = $"El/La [NombreEntidad] '{entidad.Nombre}' estaba desactivada y fue reactivada.", data = entidad })
        : RespuestaCreado(entidad, "[NombreEntidad] creada exitosamente.");
}
```

## Verificar en el Repositorio
**Ubicación**: `src/BussinesMS.Infraestructura/Repositorios/[Sistema]/[NombreEntidad]Repository.cs`

Verificar que existan estos dos métodos. Si no existen, agregarlos:

```csharp
// Busca sin filtrar por IsActive — OBLIGATORIO para detectar inactivos
public async Task<[NombreEntidad]?> ObtenerPorNombreAsync(string nombre)
{
    return await _context.[NombreEntidad]s
        .Where(c => c.Nombre.ToLower() == nombre.ToLower())
        .FirstOrDefaultAsync();
}

// Reactiva el registro inactivo
public async Task<[NombreEntidad]> ReactivarAsync(int id)
{
    var entidad = await _context.[NombreEntidad]s.FindAsync(id);
    var usuarioId = _currentUser.GetUsuarioId() ?? 1;
    entidad!.IsActive = true;
    entidad.UpdatedAt = DateTime.UtcNow;
    entidad.UpdatedByUsuarioId = usuarioId;
    entidad.DeletedAt = null;
    entidad.DeletedByUsuarioId = null;
    _context.[NombreEntidad]s.Update(entidad);
    await _context.SaveChangesAsync();
    return entidad;
}
```

También agregar a la interfaz del repositorio si no están:
```csharp
// En I[NombreEntidad]Repository
Task<[NombreEntidad]?> ObtenerPorNombreAsync(string nombre);
Task<[NombreEntidad]> ReactivarAsync(int id);
```

## Resumen de cambios

| Archivo | Cambio |
|---|---|
| `I[NombreEntidad]Service` | Firma de CrearAsync → tupla |
| `[NombreEntidad]Service` | Lógica de reactivación en CrearAsync |
| `[NombreEntidad]sController` | Manejo de FueReactivada en Crear |
| `[NombreEntidad]Repository` | Agregar ObtenerPorNombreAsync y ReactivarAsync si no existen |
| `I[NombreEntidad]Repository` | Agregar firmas si no existen |

## Respuestas al frontend después del cambio

| Caso | HTTP | Respuesta |
|---|---|---|
| Nombre activo duplicado | 400 | `{ success: false, message: "ya existe" }` |
| Reactivada | 200 | `{ mensaje: "fue reactivada", data: {...} }` |
| Creada nueva | 201 | `{ success: true, message: "creada", data: {...} }` |