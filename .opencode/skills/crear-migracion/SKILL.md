---
name: crear-migracion
description: Agrega lógica de migración para nuevos módulos al MigracionService existente.
---

# Skill: crear-migracion

## Propósito

Agregar la lógica de migración de un nuevo módulo al `MigracionService` existente. El servicio ya tiene categorías y fabricantes, y se extenderá para nuevos módulos.

## Cuándo Usar

- Cuando el usuario confirma que quiere agregar un módulo al sistema de migraciones
- Después de crear un nuevo módulo con skill `crear-modulo-crud`

## Ubicación del Archivo

**Archivo:** `src/BussinesMS.Aplicacion/Servicios/Sistema/MigracionService.cs`

## Estructura Actual

El servicio ya procesa:
- Categorías (columna 2: categoria)
- Subcategorías (columnas 2-3: categoria + subcategoria)  
- Fabricantes (columna 7: fabrica)

## Cómo Agregar un Nuevo Módulo

### Paso 1: Agregar propiedades al DTO

En `src/BussinesMS.Aplicacion/DTOs/Sistema/Migracion/FilaCsv.cs`:

```csharp
public class ResultadoMigracionDto
{
    // ... existing
    public List<string> NuevoModuloCreado { get; set; } = new();
}
```

### Paso 2: Agregar lógica en MigracionService

En `MigracionService.cs`, método `MigrarDatosDesdeCsvAsync`:

```csharp
// ============================================================
// MIGRACIÓN DE [NOMBRE_MODULO]
// ============================================================
var nuevoModuloUnicos = new HashSet<string>();

// Extraer únicos del CSV (same pattern)
for (int i = 1; i < lineas.Length; i++)
{
    var linea = lineas[i].Trim();
    if (string.IsNullOrWhiteSpace(linea)) continue;
    
    var partes = linea.Split(';');
    // Columna específica del CSV
    if (partes.Length > [COLUMNA] && !string.IsNullOrWhiteSpace(partes[COLUMNA]))
        nuevoModuloUnicos.Add(partes[COLUMNA].Trim().ToUpper());
}

// Insertar solo los que NO existen
foreach (var item in nuevoModuloUnicos)
{
    var existente = await _repositorio.AsQueryable()
        .FirstOrDefaultAsync(x => x.Nombre.ToUpper() == item);

    if (existente != null)
    {
        resultado.Omitidas++;
    }
    else
    {
        var nuevo = new [Entidad]
        {
            Nombre = item,
            CreatedAt = DateTime.UtcNow,
            CreatedByUsuarioId = 1,
            IsActive = true
        };
        await _repositorio.CrearAsync(nuevo);
        resultado.NuevoModuloCreado.Add(item);
        resultado.Creadas++;
    }
}
```

## Reglas del Sistema de Migraciones (CRÍTICO)

1. **Un solo endpoint**: `POST api/Sistema/Migraciones`
2. **Sin parámetros**: No necesita especificar tipo
3. **Inserta solo lo nuevo**: Omite duplicados automáticamente
4. **Idempotente**: Si re-ejecutás el CSV, no pasa nada
5. **El servicio crece**: Cada nuevo módulo se agrega aquí

## Ejemplo de Columnas del CSV

| Índice | Columna CSV | Entidad |
|--------|------------|--------|
| 1 | codigo de barras | Producto.CodigoBarras |
| 2 | categoria | Categoria (raíz) |
| 3 | subcategoria | Categoria (hijo) |
| 4 | nombre producto | Producto |
| 5 | sabor o descripcion | DescripcionSabor |
| 6 | peso tamanio | DescripcionTamanio |
| 7 | fabrica | Fabricante |
| 8 | unidad | TipoPresentacion (1=Unidad) |
| 9 | displey | TipoPresentacion |
| 10 | caja | TipoPresentacion |

## Archivo CSV de Datos

El sistema usa: `src/BussinesMS.API/Configuracion/Data/BDMS.csv`

Contiene 601 productos con la estructura de columnas arriba.