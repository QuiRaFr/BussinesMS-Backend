# 🤖 A.R.I.S. - Automated Resource & Inventory System
## Bienvenido a tu Asistente de Desarrollo

---

### 👋 ¡Hola! Soy A.R.I.S.

Soy tu agente automatizado para el desarrollo del sistema BussinesMS. Estoy diseñado para ayudarte a crear módulos, gestionar la arquitectura y mantener el código organizado.

---

### 📋 ¿Qué puedo hacer?

#### 🚀 Crear Módulos (Completos)
- **CRUD completo**: Entidad + DTOs + Repository + Service + Controller
- **Solo lectura**: Queries, filtros, paginación
- **Solo escritura**: Crear, actualizar, eliminar
- **Solo entidad**: Solo la clase de dominio
- **Solo servicio**: Solo lógica de negocio

#### 🔧 Mantenimiento y Modificaciones
- Corregir errores en el código
- Actualizar la arquitectura
- Modificar los skills
- Mejorar patrones existentes

#### 📊 Información del Proyecto
- Ver estado de los módulos
- Ver estructura del proyecto
- Explicar cómo funciona algo

---

### 🎯 ¿Cómo usarme?

#### Para crear un nuevo módulo (usa SUBAGENTE automáticamente):
```
"Crear módulo de Productos con CRUD"
"Agregar entidad Cliente"
"Necesito un servicio de Inventario"
"Crear repositorio de Ventas"
```

#### Para mantenimiento/correcciones (usa AGENTE PRINCIPAL):
```
"Hay un error en el servicio de..."
"Actualizar el skill de crear-modulo-crud"
"La arquitectura necesita cambios en..."
"Revisar y corregir el código de..."
```

#### Para información:
```
"¿Qué módulos existen?"
"¿Cómo está estructurado el proyecto?"
"¿Qué hace el repositorio de Usuarios?"
```

---

### 📁 Estructura del Proyecto

```
BackEnd/
├── AGENTS.md              ← Configuración del agente
├── BIENVENIDA.md          ← Este archivo
├── BussinesMS.sln        ← Solución .NET
└── src/
    ├── BussinesMS.Dominio/
    │   └── Entidades/
    │       ├── Compartido/    ← EntidadBase (compartida)
    │       ├── Auth/          ← Módulos de autenticación
    │       └── Sistema/       ← Módulos del sistema principal
    │
    ├── BussinesMS.Aplicacion/
    │   ├── DTOs/
    │   │   ├── Plantillas/    ← PagedResultDto, etc.
    │   │   ├── Auth/          ← DTOs de Auth
    │   │   └── Sistema/       ← DTOs del sistema
    │   ├── Interfaces/
    │   │   ├── Compartido/    ← IRepositorio<T>
    │   │   ├── Auth/          ← IServicios de Auth
    │   │   └── Sistema/       ← IServicios del sistema
    │   └── Servicios/
    │       ├── Auth/          ← Servicios de Auth
    │       └── Sistema/       ← Servicios del sistema
    │
    ├── BussinesMS.Infraestructura/
    │   ├── Persistencia/      ← DbContexts
    │   └── Repositorios/
    │       ├── Compartido/    ← RepositorioBase
    │       ├── Auth/          ← Repos de Auth
    │       └── Sistema/       ← Repos del sistema
    │
    └── BussinesMS.API/
        └── Controllers/       ← Controladores REST
```

---

### 🗂️ Módulos Existentes

| Módulo | Sistema | Estado |
|--------|---------|--------|
| Sistema | Auth | ✅ Completo |
| Rol | Auth | ✅ Completo |
| Usuario | Auth | ✅ Completo |
| Almacen | Auth | ✅ Completo |
| Menu | Auth | ✅ Completo |
| Permiso | Auth | ✅ Completo |

*(Se actualiza automáticamente)*

---

### ⚡ Reglas de Uso

1. **Para tareas repetitivas**: Solo dime qué necesitas crear y yo uso subagentes automáticamente
2. **Para tareas importantes**: Cuando detecte que necesitas modificar skills o arquitectura, lo haré yo mismo
3. **Para información**: Pregunta lo que quieras sobre el proyecto

### 🔑 Palabras Clave para Detección Automática

| Keywords → | Acción |
|------------|--------|
| crear, agregar, módulo, entidad, servicio, repositorio, controller | → Subagente |
| actualizar skill, modificar, corregir error, cambiar arquitectura, mejorar | → Agente Principal |
| qué es, cómo funciona, estado, estructura, explicar | → Agente Principal (info) |

---

### 🆘 ¿Necesitas ayuda?

Simplemente escribe lo que necesitas en lenguaje natural. Yo manejaré la lógica automáticamente.

**Ejemplos de comandos válidos**:
- "Quiero crear un módulo de clientes"
- "El endpoint de usuarios no funciona"
- "Agregar un nuevo campo a la entidad productos"
- "¿qué módulos tenemos?"
- "actualizar la guía de skills"

---

### 🚀 Cómo Iniciar en Futuras Sesiones

#### Opción A: Usando el script (recomendado)
```powershell
cd "C:\Users\Frank_QR\Desktop\Mishel\ProyectoMS\BackEnd"
.\iniciar-aris.ps1
```
Esto mostrará la bienvenida con instrucciones y luego abrirá opencode.

#### Opción B: Directo con argumento
```powershell
.\iniciar-aris.ps1 -Mensaje "Crear módulo de Productos"
```
Iniciará con una solicitud específica.

#### Opción C: Manual
```bash
cd "C:\Users\Frank_QR\Desktop\Mishel\ProyectoMS\BackEnd"
opencode
```
Luego pregunta: "¿Qué puedes hacer?" para ver la guía.

---

*Última actualización: 13/04/2026*