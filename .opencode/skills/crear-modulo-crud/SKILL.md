---
name: crear-modulo-crud
description: Crea un módulo completo con CRUD usando el patrón de SistemaVentas (service no conoce DB, repositorio sí).
---

# Skill: crear-modulo-crud

## Propósito
Crea un módulo completo siguiendo el patrón de clean architecture donde:
- **Service**: NO conoce DbContext, solo usa repositorios
- **Repository**: SÍ conoce DbContext
- **UnitOfWork**: Para transacciones

## Cuándo Usar
- Cuando necesitas CRUD completo
- Cuando sigas las reglas de business del AGENTS.md
- Este skill se ejecuta automáticamente cuando el usuario quiere crear un nuevo módulo

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
**Archivo**: `[NombreEntidad].cs`
```csharp
namespace BussinesMS.Dominio.Entidades;

public class [NombreEntidad] : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    // ... propiedades según specs
}
```

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

### 3. Aplicacion - Interfaz Repository (genérica)
**Ubicación**: `src/BussinesMS.Aplicacion/Interfaces/`
**Archivo**: `I[NombreEntidad]Repository.cs`
**HEREDA de**: `IRepositorio<[NombreEntidad]>`
**Métodos específicos**:
- `Task<List<[NombreEntidad]Dto>> ObtenerTodosAsync()`
- `Task<[NombreEntidad]?> ObtenerConDetallesAsync(int id)`

### 4. Aplicacion - Interfaz Service
**Ubicación**: `src/BussinesMS.Aplicacion/Interfaces/`
**Archivo**: `I[NombreEntidad]Service.cs`

### 5. Aplicacion - Servicio (NO conoce DbContext)
**Ubicación**: `src/BussinesMS.Aplicacion/Servicios/`
**Inyecta**: IRpositories, NO DbContext
```csharp
public class [NombreEntidad]Service : I[NombreEntidad]Service
{
    private readonly I[NombreEntidad]Repository _repo;
    private readonly IMapper _mapper;

    public [NombreEntidad]Service(I[NombreEntidad]Repository repo, IMapper mapper)
    {
        _repo = repo;  // Solo repositorio
        _mapper = mapper;
    }
}
```

### 6. Aplicacion - Validaciones (opcional)
**Ubicación**: `src/BussinesMS.Aplicacion/Validaciones/`
**Archivo**: `[NombreEntidad]Validator.cs`

### 7. Infraestructura - Repository (SÍ conoce DbContext)
**Ubicación**: `src/BussinesMS.Infraestructura/Repositorios/`
**Archivo**: `[NombreEntidad]Repository.cs`
**Inyecta**: AppDbContext
```csharp
public class [NombreEntidad]Repository : I[NombreEntidad]Repository
{
    private readonly AppDbContext _context;

    public [NombreEntidad]Repository(AppDbContext context)
    {
        _context = context;  // AQUÍ está el DbContext
    }
}
```

### Paginación en GET Todos (OBLIGATORIO)

Los endpoints GET deben usar paginación. Usar las clases de `BussinesMS.Aplicacion.DTOs.Plantillas`:

**Interfaz Service** - Retorna `PagedResultDto<T>`:
```csharp
Task<PagedResultDto<[NombreEntidad]Dto>> ObtenerTodosAsync(GenericPaginationQueryDto query);
```

**Implementación Service** - Usa ApplyFilters:
```csharp
public async Task<PagedResultDto<[NombreEntidad]Dto>> ObtenerTodosAsync(GenericPaginationQueryDto query)
{
    try
    {
        var baseQuery = _repo.AsQueryable();
        
        // Filtrar por texto libre (Filter)
        if (!string.IsNullOrWhiteSpace(query.Filter))
        {
            var filterLower = query.Filter.ToLower();
            baseQuery = baseQuery.Where(x => x.Nombre.ToLower().Contains(filterLower));
        }
        
        // Aplicar paginación, sorting y obtener total
        (var filteredQuery, var totalCount) = baseQuery.ApplyFilters(query);
        
        var entidades = await filteredQuery.ToListAsync();
        var dtos = _mapper.Map<List<[NombreEntidad]Dto>>(entidades);
        
        return new PagedResultDto<[NombreEntidad]Dto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = query.GetPageValue(),
            PageSize = query.GetPageSizeValue()
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al obtener [entidades]");
        throw;
    }
}
```

**Controller** - Query params:
```csharp
[HttpGet]
public async Task<IActionResult> ObtenerTodos([FromQuery] GenericPaginationQueryDto query)
{
    var resultado = await _servicio.ObtenerTodosAsync(query);
    return RespuestaOk(resultado);
}
```

**Parámetros de paginación**:
- `Page` (default: 1)
- `PageSize` (default: 10, max: 100)
- `Filter` - texto libre para búsqueda
- `SortBy` - campo a ordenar
- `SortDirection` - "asc" o "desc"

### 8. Registro en Program.cs
```csharp
builder.Services.AddDbContext<SistemaDbContext>(...);
builder.Services.AddScoped<I[NombreEntidad]Repository, [NombreEntidad]Repository>();
builder.Services.AddScoped<I[NombreEntidad]Service, [NombreEntidad]Service>();
```

### 9. Controller
**Ubicación**: `src/BussinesMS.API/Controllers/`
**Ruta SEGÚN el sistema**:
- Sistema: `[Route("api/Sistema/[controller]")]`
- Navidad: `[Route("api/Navidad/[controller]")]`
**Endpoints**: GET, POST, PUT, DELETE

## Ejemplo Completo

```
Usuario: "Crear módulo Producto con CRUD"
→ Skill: crear-modulo-crud
→ Crear:
  - Producto.cs (Dominio)
  - ProductoDto, CrearProductoDto (Aplicacion/DTOs)
  - IProductoRepository, IProductoService (Aplicacion/Interfaces)
  - ProductoService (Aplicacion/Servicios) - NO conoce DB
  - ProductoRepository (Infraestructura) - SÍ conoce DB
  - ProductoController (API)
```

## Reglas del PATRÓN (CRITICAL)

1. **Service NUNCA inyecta DbContext** - Solo repositorios
2. **Repository SÍ inyecta DbContext** - Para acceso a datos
3. **Transacciones usar IUnitOfWork** - En servicios complejos
4. **Validaciones en Service** - Con ValidacionEntidad helper
5. **Naming siguiendo AGENTS.md** - IServicio, Repositorio, etc.

### 9. Aplicacion - Mapeos AutoMapper (OBLIGATORIO)
**Ubicación**: `src/BussinesMS.Aplicacion/Mapeos/MappingProfile.cs`
**AGREGAR estos mapeos**:
```csharp
CreateMap<[Entidad], [Entidad]Dto>();
CreateMap<[Entidad]Dto, [Entidad]>();
CreateMap<Crear[Entidad]Dto, [Entidad]>();
```

### 10. Servicio - Manejo de Errores con Logging (OBLIGATORIO)
**Agregar en el servicio**:
```csharp
private readonly ILogger<[Entidad]Service> _logger;

public [Entidad]Service(I[Entidad]Repository repo, IMapper mapper, ILogger<[Entidad]Service> logger)
{
    _repo = repo;
    _mapper = mapper;
    _logger = logger;
}

public async Task<List<[Entidad]Dto>> ObtenerTodosAsync()
{
    try
    {
        var entidades = await _repo.ObtenerTodosAsync();
        return _mapper.Map<List<[Entidad]Dto>>(entidades);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al obtener [entidades]");
        throw;
    }
}
```

### 11. Agregar paquetes de logging y EF
En `BussinesMS.Aplicacion.csproj`:
```xml
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
```

## Base de Datos por Módulo

Según el AGENTS.md, el sistema tiene 3 bases de datos:

| Módulo | Base de Datos | DbContext |
|--------|-------------|----------|
| Catálogo (Categoria, Producto, etc.) | SistemaDB | SistemaDbContext |
| Inventario (Lotes, Movimientos) | SistemaDB | SistemaDbContext |
| Proveedores | SistemaDB | SistemaDbContext |
| Ventas | SistemaDB | SistemaDbContext |
| Usuarios, Roles | AuthDB | AuthDbContext |
| Almacenes | AuthDB | AuthDbContext |
| Navideño (pendiente) | NavidadDB | NavidadDbContext |

**IMPORTANTE**: Al crear un módulo, especificar en qué DbContext debe registrarse.