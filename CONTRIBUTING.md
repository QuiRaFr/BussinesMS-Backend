# Guía de contribución — BussinesMS Backend

## Requisitos previos
- Tener una cuenta en GitHub
- Tener instalado Git
- Tener instalado .NET 9

---

## 1. Hacer Fork del repositorio

1. Entra a [github.com/QuiRaFr/BussinesMS-Backend](https://github.com/QuiRaFr/BussinesMS-Backend)
2. Click en el botón **Fork** (arriba a la derecha)
3. Esto crea una copia del repo en tu cuenta

---

## 2. Clonar tu fork

```bash
git clone https://github.com/TU_USUARIO/BussinesMS-Backend.git
cd BussinesMS-Backend
```

---

## 3. Conectar con el repo principal

```bash
git remote add upstream https://github.com/QuiRaFr/BussinesMS-Backend.git
```

Verifica que quedó bien:
```bash
git remote -v
```

Deberías ver:
```
origin    https://github.com/TU_USUARIO/BussinesMS-Backend.git
upstream  https://github.com/QuiRaFr/BussinesMS-Backend.git
```

---

## 4. Crear tu rama de trabajo

Siempre crea tu rama desde `develop`:

```bash
git checkout develop
git pull upstream develop
git checkout -b feature/nombre-de-tu-funcionalidad
```

### Convención de nombres para ramas:
| Tipo | Ejemplo |
|------|---------|
| Nueva funcionalidad | `feature/crear-modulo-clientes` |
| Corrección de bug | `fix/error-login` |
| Mejora | `improvement/optimizar-consultas` |

---

## 5. Trabajar y guardar cambios

```bash
git add .
git commit -m "feat: descripcion corta de lo que hiciste"
git push origin feature/nombre-de-tu-funcionalidad
```

### Convención de commits:
| Prefijo | Uso |
|---------|-----|
| `feat:` | Nueva funcionalidad |
| `fix:` | Corrección de bug |
| `chore:` | Tareas de mantenimiento |
| `refactor:` | Refactorización de código |
| `docs:` | Documentación |

---

## 6. Crear Pull Request

1. Entra a tu fork en GitHub
2. Click en **Compare & pull request**
3. Asegúrate que el PR apunta a `develop` del repo principal
4. Escribe una descripción clara de los cambios
5. Click en **Create pull request**

---

## 7. Mantener tu fork actualizado

Antes de empezar a trabajar, siempre sincroniza con el repo principal:

```bash
git fetch upstream
git checkout develop
git merge upstream/develop
git push origin develop
```

---

## Estructura del proyecto

```
BackEnd/
├── .opencode/skills/       # Skills de generacion de codigo
└── src/
    ├── BussinesMS.API/         # Controladores, middlewares, filtros
    ├── BussinesMS.Aplicacion/  # Servicios, DTOs, interfaces
    ├── BussinesMS.Dominio/     # Entidades, excepciones
    └── BussinesMS.Infraestructura/ # Repositorios, persistencia
```

---

## Contacto

Cualquier duda comunicarse con el responsable del repositorio: [@QuiRaFr](https://github.com/QuiRaFr)
