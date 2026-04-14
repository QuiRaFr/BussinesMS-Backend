# BussinesMS - Comandos de Entity Framework Core

## Requisitos

- .NET 9 SDK
- SQL Server Express (local)
- Las cadenas de conexión están configuradas en `appsettings.json` (NO necesitas especificarlas en los comandos)

---

## Ejecutar la API

```bash
# Desde la raíz del proyecto
dotnet run --project src/BussinesMS.API
```

La API estará disponible en: **http://localhost:5001**
Swagger: **http://localhost:5001/swagger**

---

## Compilar el proyecto

```bash
# Compilar toda la solución
dotnet build BussinesMS.sln

# Compilar un proyecto específico
dotnet build src/BussinesMS.API/BussinesMS.API.csproj
```

---

## Aplicar migraciones a la BD

Los comandos leen automáticamente la conexión del `appsettings.json`:

```bash
# Aplicar todas las migraciones pendientes
dotnet ef database update -p src/BussinesMS.Infraestructura -s src/BussinesMS.API

# Especificar qué DbContext usar (requerido si hay múltiples)
dotnet ef database update -p src/BussinesMS.Infraestructura -s src/BussinesMS.API --context AuthDbContext
dotnet ef database update -p src/BussinesMS.Infraestructura -s src/BussinesMS.API --context SistemaDbContext
dotnet ef database update -p src/BussinesMS.Infraestructura -s src/BussinesMS.API --context NavidadDbContext
```

---

## Crear una migración

```bash
# Crear migración para AuthDbContext
dotnet ef migrations add NombreMigracion -p src/BussinesMS.Infraestructura -s src/BussinesMS.API --context AuthDbContext -o Persistencia/Migraciones
```

**Ejemplos de nombres**: `InitialCreate`, `AddPermisosToUsuario`, `CreateMenusTable`

---

## Ver migraciones existentes

```bash
dotnet ef migrations list -p src/BussinesMS.Infraestructura -s src/BussinesMS.API --context AuthDbContext
```

---

## Eliminar la última migración

```bash
dotnet ef migrations remove -p src/BussinesMS.Infraestructura -s src/BussinesMS.API --context AuthDbContext
```
**Nota**: Solo funciona si la migración NO ha sido aplicada a la BD.

---

## Revertir la BD a una migración anterior

```bash
dotnet ef database update NombreMigracion -p src/BussinesMS.Infraestructura -s src/BussinesMS.API --context AuthDbContext
```

**Ejemplo**:
```bash
dotnet ef database update InitialCreate -p src/BussinesMS.Infraestructura -s src/BussinesMS.API --context AuthDbContext
```

---

## Eliminar y recrear la base de datos

Para desarrolladores: comando para eliminar la BD, crear migración inicial y aplicar:

```powershell
# 1. Eliminar la base de datos
dotnet ef database drop -p src/BussinesMS.Infraestructura -s src/BussinesMS.API --context AuthDbContext --force

# 2. Eliminar migraciones locales (si están corruptas)
dotnet ef migrations remove -p src/BussinesMS.Infraestructura -s src/BussinesMS.API --context AuthDbContext

# 3. Crear migración inicial
dotnet ef migrations add InitialCreate -p src/BussinesMS.Infraestructura -s src/BussinesMS.API --context AuthDbContext -o Persistencia/Migraciones

# 4. Aplicar migración
dotnet ef database update -p src/BussinesMS.Infraestructura -s src/BussinesMS.API --context AuthDbContext
```

---

## Bases de Datos

El sistema usa 3 bases de datos (configuradas en `appsettings.json`):

| Base de Datos | DbContext | Propósito |
|---------------|-----------|-----------|
| BussinesMS_Auth | `AuthDbContext` | Usuarios, Roles, Almacenes, Permisos, Menús |
| BussinesMS_Sistema | `SistemaDbContext` | Inventario, Ventas, Compras (pendiente) |
| BussinesMS_Navidad | `NavidadDbContext` | Temporada navideña (pendiente) |

---

## Flujo normal de trabajo

```
1. Modificás una entidad en Dominio (agregar/modificar campo)
        ↓
2. dotnet ef migrations add NombreMigracion ... --context AuthDbContext
        ↓
3. Revisás la carpeta Migrations para verificar que el cambio es correcto
        ↓
4. dotnet ef database update ... --context AuthDbContext
        ↓
5. La BD refleja el cambio
```

---

## Errores comunes

| Error | Causa | Solución |
|-------|-------|----------|
| More than one DbContext was found | Múltiples DbContext sin especificar | Usar `--context NombreDbContext` |
| There is already an object named 'X' | Tabla ya existe pero EF no la ve | Eliminar BD y recrear |
| Pending model changes | Modificaste entidad sin crear migración | Crear nueva migración |
| FK constraint conflict | Datos con valores inválidos | Actualizar datos o eliminar FK temporal |

---

## Notas

- Los comandos usan autenticación SQL (`User Id=sa;Password=...`) definida en `appsettings.json`
- El parámetro `--connection` NO es necesario si usas `--context`
- Las migraciones se guardan en `src/BussinesMS.Infraestructura/Persistencia/Migraciones/`