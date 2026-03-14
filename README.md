# Expense Control App

Sistema de control de gastos personales desarrollado con **ASP.NET Core** y tecnologías web modernas. Permite registrar ingresos y egresos, categorizarlos, visualizar un dashboard con KPIs y gráficos, y generar reportes en PDF y Excel.

## Dos versiones en este repositorio

| Versión | Ubicación | Uso |
|--------|-----------|-----|
| **Monolito actual** | Raíz del repo (`ExpenseControlApp.csproj`, `Controllers/`, `wwwroot/`, etc.) | App actual con SPA, dashboard, reportes. Ejecutar con `dotnet run` desde la raíz. |
| **Nueva arquitectura (Clean Architecture)** | `src/` y `tests/` | Solución rediseñada por capas (Domain, Application, Infrastructure, Api), CQRS con MediatR, JWT + refresh tokens, migraciones EF Core, sin `EnsureCreated()`. Ver **[docs/README-NEW-ARCHITECTURE.md](docs/README-NEW-ARCHITECTURE.md)** y **[docs/ADR-001-architectural-redesign.md](docs/ADR-001-architectural-redesign.md)**. |

Para compilar y ejecutar la **nueva arquitectura**: `dotnet build src/ExpenseControl.Api/ExpenseControl.Api.csproj` y `dotnet run --project src/ExpenseControl.Api`. La API escucha en **http://localhost:5070** (puerto 5070 para no chocar con el monolito en 5050 ni con puertos bloqueados por el navegador). Swagger: **http://localhost:5070/swagger**. La API v1 expone `/api/v1/auth/login`, `/api/v1/auth/register`, `/api/v1/auth/refresh`, `/api/v1/auth/revoke`, más CRUD de categorías, cuentas, transacciones y dashboard. No hay usuario admin por defecto; el bootstrap del primer administrador se documenta en la nueva guía.

---

## Tabla de contenidos

- [Características](#características)
- [Tecnologías](#tecnologías)
- [Estructura del proyecto](#estructura-del-proyecto)
- [Requisitos](#requisitos)
- [Configuración](#configuración)
- [Ejecución local](#ejecución-local)
- [Ejecución con Docker](#ejecución-con-docker)
- [API](#api)
- [Credenciales por defecto](#credenciales-por-defecto)
- [Seguridad](#seguridad)
- [Interfaz de usuario](#interfaz-de-usuario)
- [Desarrollo](#desarrollo)
- [Soporte](#soporte)

---

## Características

### Funcionalidad principal

| Funcionalidad | Descripción |
|---------------|-------------|
| **Autenticación** | Registro, login y perfiles de usuario con JWT |
| **Transacciones** | Alta, edición, eliminación y filtrado de ingresos y gastos |
| **Categorías** | Categorías predefinidas y personalizables (ingreso/egreso) |
| **Dashboard** | KPIs en tiempo real, tendencias mensuales y gráfico de categorías |
| **Reportes** | Exportación a PDF y Excel con rango de fechas |
| **Visualización** | Gráficos interactivos (Chart.js) y tabla de transacciones recientes |

### Detalles técnicos

- Autenticación JWT (Bearer).
- API REST documentada con Swagger/OpenAPI.
- Interfaz responsive (Bootstrap 5.3) y SPA con JavaScript.
- Base de datos creada automáticamente al primer arranque con datos semilla.
- Soporte para despliegue con Docker y Docker Compose.

---

## Tecnologías

### Backend

| Tecnología | Versión / Detalle |
|------------|-------------------|
| **Framework** | .NET 10.0 (multi-target posible con .NET 9) |
| **Base de datos** | PostgreSQL 15+ (Npgsql.EntityFrameworkCore.PostgreSQL 9.0.2) |
| **ORM** | Entity Framework Core 9.0.8 |
| **Autenticación** | JWT Bearer (Microsoft.AspNetCore.Authentication.JwtBearer 9.0.0) |
| **Documentación API** | Swashbuckle.AspNetCore 7.2.0 |
| **Contraseñas** | BCrypt.Net-Next 4.0.3 |
| **Reportes** | ClosedXML (Excel), iText7 (PDF) |

### Frontend

| Tecnología | Uso |
|------------|-----|
| **Bootstrap** | 5.3.0 – layout y componentes |
| **Font Awesome** | 6.4.0 – iconos |
| **Chart.js** | Gráficos (tendencias mensuales, categorías) |
| **JavaScript** | Vanilla ES6+ – lógica SPA y llamadas a la API |

### Infraestructura

- **Docker** y **Docker Compose** para app + PostgreSQL.
- **Entity Framework** con `EnsureCreated()` para inicializar la base en desarrollo (migraciones en `Migrations/`).

---

## Estructura del proyecto

```
appControlGastos/
├── Controllers/           # Controladores API
│   ├── AuthController.cs
│   ├── CategoriesController.cs
│   ├── DashboardController.cs
│   ├── ReportsController.cs
│   └── TransactionsController.cs
├── Data/
│   └── ExpenseDbContext.cs
├── Migrations/            # Migraciones EF Core
├── Models/
│   ├── Category.cs
│   ├── Transaction.cs
│   ├── User.cs
│   └── DTOs/
│       ├── AuthResponse.cs
│       ├── DashboardDto.cs
│       ├── LoginRequest.cs
│       ├── RegisterRequest.cs
│       └── TransactionDto.cs
├── Services/              # Lógica de negocio
│   ├── AuthService.cs / IAuthService.cs
│   ├── DashboardService.cs / IDashboardService.cs
│   ├── ReportService.cs / IReportService.cs
│   ├── TransactionService.cs / ITransactionService.cs
│   └── UserService.cs / IUserService.cs
├── Properties/
│   └── launchSettings.json
├── wwwroot/
│   ├── css/
│   │   └── app.css
│   ├── js/
│   │   └── app.js
│   └── index.html
├── appsettings.json
├── appsettings.Development.json
├── docker-compose.yml
├── Dockerfile
├── ExpenseControlApp.csproj
├── Program.cs
├── QUICKSTART.md
└── README.md
```

---

## Requisitos

- **.NET 10 SDK** (o .NET 9 si se ajusta el `<TargetFramework>` en el `.csproj`).
- **PostgreSQL 15+** en ejecución (puerto 5432) para desarrollo local.
- **Docker** y **Docker Compose** (opcional) para ejecutar todo en contenedores.

---

## Configuración

### Cadena de conexión

En **appsettings.json** o **appsettings.Development.json**:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ExpenseControlDB;Username=postgres;Password=Pass@word"
  }
}
```

Ajusta `Host`, `Database`, `Username` y `Password` según tu entorno.

### JWT (appsettings.json)

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "ExpenseControlApp",
    "Audience": "ExpenseControlAppUsers",
    "ExpiryInMinutes": 60
  }
}
```

En producción se recomienda usar variables de entorno o un gestor de secretos y una clave más larga y aleatoria.

---

## Ejecución local

1. **PostgreSQL** en marcha (puerto 5432). La aplicación crea la base `ExpenseControlDB` en el primer arranque si no existe.

2. **Restaurar y ejecutar:**

   ```bash
   cd appControlGastos
   dotnet restore
   dotnet run --launch-profile http
   ```

3. **URLs:**
   - **Web (SPA):** http://localhost:5050  
   - **Swagger:** http://localhost:5050/swagger  

El perfil `http` usa el puerto **5050** para evitar conflicto con AirPlay en macOS (puerto 5000).

---

## Ejecución con Docker

```bash
docker-compose up --build
```

- **Web:** http://localhost:5000  
- **Swagger:** http://localhost:5000/swagger  
- **PostgreSQL** queda en el puerto 5432; la app se conecta al servicio `db` con la cadena definida en `docker-compose.yml`.

---

## API

Base URL: `/api`.

### Autenticación (`/api/auth`)

| Método | Ruta | Descripción |
|--------|------|-------------|
| POST   | `/api/auth/login`    | Login (email, password) → token + usuario |
| POST   | `/api/auth/register` | Registro de usuario |
| GET    | `/api/auth/me`       | Perfil del usuario actual (Bearer) |

### Transacciones (`/api/transactions`)

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET    | `/api/transactions` | Listado con filtros (fechas, categoría, tipo) |
| POST   | `/api/transactions` | Crear transacción |

Requieren cabecera `Authorization: Bearer {token}`.

### Dashboard (`/api/dashboard`)

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET    | `/api/dashboard` | KPIs, transacciones recientes, tendencias y categorías |

Requiere Bearer.

### Categorías (`/api/categories`)

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET    | `/api/categories` | Listar categorías |
| POST   | `/api/categories` | Crear categoría (según permisos) |

Requiere Bearer.

### Reportes (`/api/reports`)

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET    | `/api/reports/pdf?startDate=...&endDate=...`   | Descargar PDF |
| GET    | `/api/reports/excel?startDate=...&endDate=...` | Descargar Excel |

Requieren Bearer.

---

## Credenciales por defecto

Usuario administrador creado por defecto (seed):

- **Email:** `admin@expensecontrol.com`  
- **Contraseña:** `admin123`

---

## Seguridad

- Autenticación JWT con validación de emisor, audiencia y firma.
- Contraseñas hasheadas con BCrypt.
- CORS configurado (en desarrollo con política “AllowAll”; en producción conviene restringir orígenes).
- Consultas a base de datos mediante EF Core (parámetros, reducción de riesgo de inyección SQL).
- Validación de modelos y datos en controladores y DTOs.

---

## Interfaz de usuario

- **Login:** pantalla centrada (vertical y horizontal); sin barra superior en la vista de login.
- **Resto de la app:** barra azul con “Expense Control”, menú (Dashboard, Transactions, Reports, Categories) y dropdown de usuario con nombre y Logout.
- **Dashboard:** tarjetas de KPIs (ingresos, gastos, balance actual, balance mensual), gráfico de tendencias mensuales, gráfico de categorías de gastos y tabla de transacciones recientes.
- **Transacciones:** filtros por fechas, categoría y tipo; tabla con acciones (editar/eliminar según implementación).
- **Reportes:** selección de rango de fechas y botones para generar PDF y Excel.
- **Categorías:** listado y alta de categorías (nombre, tipo, color, icono).

---

## Desarrollo

### Comandos útiles

```bash
dotnet restore
dotnet build
dotnet run --launch-profile http
```

### Añadir funcionalidad

1. Modelos en `Models/` y DTOs en `Models/DTOs/`.
2. Servicios en `Services/` (interfaz + implementación).
3. Controladores en `Controllers/` y rutas bajo `/api/...`.
4. Ajustes en `wwwroot/js/app.js` y `wwwroot/index.html` para la SPA.

### Documentación rápida

- Guía de arranque: **QUICKSTART.md**.

---

## Soporte

Para dudas o incidencias, abre un issue en el repositorio o contacta al equipo de desarrollo.

---

## Licencia

Este proyecto está bajo la licencia MIT (ver archivo LICENSE si existe).
