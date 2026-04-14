---
name: crear-entidad
description: Crea solo la entidad del dominio. Para cuando ya existe todo lo demás.
---

# Skill: crear-entidad

## Propósito
Crea únicamente la entidad del dominio.

## Cuándo Usar
- "solo entidad", "solo la clase"
- Entidad existente no existe
- Para pruebas

## Estructura

### Dominio - Entidad
**Ubicación**: `src/BussinesMS.Dominio/Entidades/`
```csharp
namespace BussinesMS.Dominio.Entidades;

public class [NombreEntidad] : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
```

## Ejemplo
```
Usuario: "Crear entidad Cliente"
→ Skill: crear-entidad  
→ Crear: Cliente.cs
```

## IMPORTANTE: Después de crear la entidad

Si esta entidad se usará en un CRUD completo, también se debe:

1. **Agregar mapeos** en `MappingProfile.cs`:
```csharp
CreateMap<[Entidad], [Entidad]Dto>();
CreateMap<[Entidad]Dto, [Entidad]>();
```

2. **Agregar logging** al paquete de Aplicacion si no existe:
```xml
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.0" />
```

## Entidades por Base de Datos

| Base de Datos | Entidades |
|-------------|----------|
| AuthDB | Sistema, Rol, Usuario, Almacen |
| SistemaDB | Categoria, Fabricante, Producto, Proveedor, Compra, Venta, etc. |
| NavidadDB | Pendiente |