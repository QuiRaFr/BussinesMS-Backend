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

## 5. Mapeos AutoMapper (OBLIGATORIO)
Agregar en MappingProfile.cs los mapeos para la entidad.

## 6. Manejo de Errores (OBLIGATORIO)
El servicio debe incluir:
- ILogger en el constructor
- try-catch en cada método público
- Logging de errores