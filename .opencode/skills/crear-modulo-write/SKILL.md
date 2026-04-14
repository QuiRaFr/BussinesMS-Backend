---
name: crear-modulo-write
description: Crea un módulo de solo escritura (POST/PUT/DELETE) usando el patrón de SistemaVentas.
---

# Skill: crear-modulo-write

## Propósito
Crea un módulo de solo escritura - sin consultas.

## Cuándo Usar
- "solo escritura", "solo operaciones"
- Módulos de procesamiento
- POST/PUT/DELETE sin GET

## Ubicación según el Sistema

El proyecto está organizado por sistemas. Debes colocar los archivos en la carpeta correcta según el sistema al que pertenece el módulo:

### Si el módulo es del sistema "Auth" (autenticación):
- Entidades → `src/BussinesMS.Dominio/Entidades/Auth/`
- DTOs → `src/BussinesMS.Aplicacion/DTOs/Auth/`
- Interfaces → `src/BussinesMS.Aplicacion/Interfaces/Auth/`
- Servicios → `src/BussinesMS.Aplicacion/Servicios/Auth/`
- Repositorios → `src/BussinesMS.Infraestructura/Repositorios/Auth/`
- Controller → `src/BussinesMS.API/Controllers/`

### Si el módulo es del sistema "Sistema" (negocio principal):
- Entidades → `src/BussinesMS.Dominio/Entidades/Sistema/`
- DTOs → `src/BussinesMS.Aplicacion/DTOs/Sistema/`
- Interfaces → `src/BussinesMS.Aplicacion/Interfaces/Sistema/`
- Servicios → `src/BussinesMS.Aplicacion/Servicios/Sistema/`
- Repositorios → `src/BussinesMS.Infraestructura/Repositorios/Sistema/`
- Controller → `src/BussinesMS.API/Controllers/`

### Si el módulo es del sistema "Navidad" (temporada navideña):
- Entidades → `src/BussinesMS.Dominio/Entidades/Navidad/`
- DTOs → `src/BussinesMS.Aplicacion/DTOs/Navidad/`
- Interfaces → `src/BussinesMS.Aplicacion/Interfaces/Navidad/`
- Servicios → `src/BussinesMS.Aplicacion/Servicios/Navidad/`
- Repositorios → `src/BussinesMS.Infraestructura/Repositorios/Navidad/`
- Controller → `src/BussinesMS.API/Controllers/`

**Importante**: Los archivos Compartidos (EntidadBase, IRepositorio<T>, PagedResultDto, etc.) permanecen en su ubicación actual.

## Estructura a Créar

### 1. Dominio - Entidad
**Ubicación**: `src/BussinesMS.Dominio/Entidades/`

### 2. Aplicacion - DTOs
**Ubicación**: `src/BussinesMS.Aplicacion/DTOs/`

**IMPORTANTE - Separación de DTOs**:

Para cada entidad, crear:
- `[NombreEntidad]Dto` - Para lectura (GET) - incluye Id
- `Crear[NombreEntidad]Dto` - Para creación (POST) - sin Id
- `Actualizar[NombreEntidad]Dto` - Para actualización (PUT) - con Id

**Ejemplo**:
```csharp
// ProductoDto.cs
public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}

public class CrearProductoDto
{
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}

public class ActualizarProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
}
```

### 3. Aplicacion - Interfaz Repository
**Solo métodos write**:
- `CrearAsync(entidad)`
- `ActualizarAsync(entidad)`
- `EliminarAsync(int id)`

### 4. Aplicacion - Interfaz Service

### 5. Aplicacion - Servicio
**Usa IUnitOfWork** para transacciones si es necesario
**Inyecta**: Repositorios, NO DbContext

### 6. Infraestructura - Repository
**Inyecta**: DbContext

### 7. Controller
**Ruta SEGÚN el sistema**:
- Sistema: `[Route("api/Sistema/[controller]")]`
- Navidad: `[Route("api/Navidad/[controller]")]`

**Solo write**:
- `POST /api/Sistema/[nombreentidad]`
- `PUT /api/Sistema/[nombreentidad]/{id}`
- `DELETE /api/Sistema/[nombreentidad]/{id}`

## Ejemplo

```
Usuario: "Crear módulo Ventas solo escritura"  
→ Skill: crear-modulo-write
→ Crear:
  - Venta.cs
  - IVentaRepository (write only)
  - VentaService (con IUnitOfWork)
  - VentasController (POST/PUT/DELETE)
```

## Reglas

1. **NO crear métodos GET**: Sin ObtenerTodos/ObtenerPorId
2. **Service usa IUnitOfWork** para transacciones
3. **Service NO conoce DbContext**
4. **Repository SÍ conoce DbContext**

## 8. Mapeos AutoMapper (OBLIGATORIO)
Agregar en MappingProfile.cs:
```csharp
CreateMap<[Entidad], [Entidad]Dto>();
CreateMap<Crear[Entidad]Dto, [Entidad]>();
CreateMap<Actualizar[Entidad]Dto, [Entidad]>();
```

## 9. Manejo de Errores (OBLIGATORIO)
Agregar try-catch con logging en el servicio.