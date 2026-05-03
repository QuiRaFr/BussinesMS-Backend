---
name: crear-servicio
description: Crea solo la capa de servicio (lógica de negocio). Para cuando la entidad ya existe.
---

# Skill: crear-servicio

## Propósito
Crea únicamente servicio con lógica de negocio.

## Cuándo Usar
- "solo servicio", "lógica de negocio"
- Entidad y repositorio ya existen
- Para procesos complejos

## Estructura

### 1. Aplicacion - DTO (si no existe)
**Ubicación**: `src/BussinesMS.Aplicacion/DTOs/`

### 2. Aplicacion - Interfaz Service
**Ubicación**: `src/BussinesMS.Aplicacion/Interfaces/`
```csharp
public interface I[NombreEntidad]Service
{
    Task<...> Metodo1Async(...);
    Task<...> Metodo2Async(...);
}
```

### 3. Aplicacion - Servicio
**Ubicación**: `src/BussinesMS.Aplicacion/Servicios/`
**CRITICAL**: Inyecta repositorios, NO DbContext
```csharp
public class [NombreEntidad]Service : I[NombreEntidad]Service
{
    private readonly I[NombreEntidad]Repository _repo;
    
    public [NombreEntidad]Service(I[NombreEntidad]Repository repo)
    {
        _repo = repo;  // Solo repositorio
    }
    
    public async Task<...> MetodoAsync(...)
    {
        // Lógica de negocio aquí
        // NUNCA usar DbContext directamente
    }
}
```

### 4. Registro en Program.cs
```csharp
builder.Services.AddScoped<I[NombreEntidad]Service, [NombreEntidad]Service>();
```

## Ejemplo
```
Usuario: "Crear servicio para calcular inventario"
→ Skill: crear-servicio
→ Verificar: Entidad Inventario existe
→ Crear:
  - IInventarioService.cs
  - InventarioService.cs (lógica de cálculo)
```

## Reglas (CRITICAL)

1. **Service NUNCA conoce DbContext**
2. **Inyectar solo repositorios**
3. **Lógica de negocio en service**
4. **Si hay transacciones, usar IUnitOfWork**

## 4.1. Regla de Transacciones (CRITICAL)

Cuando el servicio tiene MÚLTIPLES OPERACIONES (subprocesos), DEBE usar IUnitOfWork:

### Cuándo Usar Transactions:
- Crear + Actualizar (ej: crear producto, luego generar código)
- Crear + Crear (ej: venta + detalles de venta)
- Actualizar + Actualizar (ej: múltiples actualizaciones)
- Crear + Eliminar

### Patrón de Implementación:

```csharp
using BussinesMS.Aplicacion.Interfaces.Auth;

public class [NombreEntidad]Service : I[NombreEntidad]Service
{
    private readonly I[NombreEntidad]Repository _repo;
    private readonly IUnitOfWork _uow;
    
    public [NombreEntidad]Service(
        I[NombreEntidad]Repository repo,
        IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }
    
    public async Task<...> MetodoConTransaccionAsync(...)
    {
        await _uow.BeginTransactionAsync();
        try
        {
            // Operación 1
            var entidad = new Entidad { ... };
            var creada = await _repo.CrearAsync(entidad);
            
            // Operación 2 (subproceso)
            creada.CampoGenerado = $"COD-{creada.Id}";
            await _repo.ActualizarAsync(creada);
            
            await _uow.CommitAsync();
            return _mapper.Map<Dto>(creada);
        }
        catch
        {
            await _uow.RollbackAsync();
            throw;
        }
    }
}
```

### Registro en Program.cs:
```csharp
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
```

## 5. Mapeos AutoMapper (OBLIGATORIO)
Agregar en MappingProfile.cs los mapeos para la entidad.

## 6. Manejo de Errores (OBLIGATORIO)
El servicio debe incluir:
- ILogger en el constructor
- try-catch en cada método público
- Logging de errores

## 7. Validaciones de Negocio
Agregar validaciones usando `ValidacionEntidad`:
```csharp
// Verificar existe
ValidacionEntidad.VerificarExiste(entidad, "Entidad");

// Verificar activo
ValidacionEntidad.VerificarActivo(entidad, "Entidad");

// Verificar duplicado
ValidacionEntidad.VerificarNoDuplicado(
    await _repo.ExisteNombreAsync(dto.Nombre),
    "entidad", dto.Nombre);
```

## 8. Registro en Program.cs
```csharp
builder.Services.AddScoped<I[NombreEntidad]Service, [NombreEntidad]Service>();
```