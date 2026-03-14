# Expense Control App – Nueva arquitectura (Clean Architecture)

Este documento describe la solución rediseñada bajo `src/` y `tests/`, con Clean Architecture, CQRS ligero (MediatR), JWT + refresh tokens, y preparada para producción.

## Estructura de la solución

```
ExpenseControl.sln
├── src/
│   ├── ExpenseControl.Api          # Web API, controllers v1, middleware
│   ├── ExpenseControl.Application  # Casos de uso, CQRS, DTOs, validaciones (FluentValidation), interfaces
│   ├── ExpenseControl.Domain       # Entidades, enums, reglas de negocio puras
│   ├── ExpenseControl.Infrastructure # EF Core, PostgreSQL, JWT, AuthService, repositorios
│   └── ExpenseControl.Shared       # Result, excepciones, constantes (ErrorCodes)
└── tests/
    ├── ExpenseControl.UnitTests
    └── ExpenseControl.IntegrationTests
```

## Requisitos

- .NET 10 SDK (o .NET 9 ajustando `<TargetFramework>` en los .csproj)
- PostgreSQL 15+
- Variables de entorno o `appsettings.Development.json` para conexión y JWT (nunca commitear secretos reales)

## Configuración

- **Conexión**: `ConnectionStrings__DefaultConnection` o `appsettings.json` / `appsettings.Development.json`
- **JWT**: `Jwt__SecretKey`, `Jwt__Issuer`, `Jwt__Audience`, `Jwt__AccessTokenExpiryMinutes`
- Copiar `.env.example` a `.env` y rellenar; en producción usar Secret Manager o variables de entorno.

## Compilación y ejecución

```bash
# Desde la raíz del repositorio
dotnet restore ExpenseControl.sln
dotnet build src/ExpenseControl.Api/ExpenseControl.Api.csproj

# Crear la primera migración (solo una vez)
dotnet ef migrations add Initial --project src/ExpenseControl.Infrastructure --startup-project src/ExpenseControl.Api

# Aplicar migraciones (o se aplican al arrancar si usas MigrateAsync())
dotnet ef database update --project src/ExpenseControl.Infrastructure --startup-project src/ExpenseControl.Api

# Ejecutar la API
cd src/ExpenseControl.Api && dotnet run
```

La API escucha en **http://localhost:5070** (configurado en `src/ExpenseControl.Api/Properties/launchSettings.json`). Swagger: **http://localhost:5070/swagger**.

## API v1

- **Base URL**: `/api/v1`
- **Auth** (todos excepto login/register/refresh/revoke requieren `Authorization: Bearer {accessToken}`):
  - `POST /api/v1/auth/login` – body: `{ "email", "password" }` → access + refresh token
  - `POST /api/v1/auth/register` – body: `{ "firstName", "lastName", "email", "password", "confirmPassword" }` → access + refresh token
  - `POST /api/v1/auth/refresh` – body: `{ "refreshToken" }` → nuevos tokens
  - `POST /api/v1/auth/revoke` – body: `{ "refreshToken" }` → revoca el refresh token (logout)
- **Users**: `GET /api/v1/users/me` – perfil del usuario autenticado
- **Categories**: `GET/POST /api/v1/categories`, `GET/PUT/DELETE /api/v1/categories/{id}` – CRUD con ownership y paginación (`?page=1&pageSize=20&type=0|1&includeInactive=false`)
- **Accounts**: `GET/POST /api/v1/accounts`, `GET/PUT/DELETE /api/v1/accounts/{id}` – CRUD con ownership y paginación
- **Transactions**: `GET/POST /api/v1/transactions`, `GET/PUT/DELETE /api/v1/transactions/{id}` – CRUD con ownership, paginación y filtros (`?type=0|1|2&accountId=&categoryId=&fromDate=&toDate=`)
- **Dashboard**: `GET /api/v1/dashboard?fromDate=&toDate=` – KPIs (ingresos, gastos, balance, mensual) y últimas transacciones
- **Health**: `GET /health` – liveness; `GET /health/ready` – readiness (incluye base de datos)

- **Swagger**: en desarrollo, **http://localhost:5070/swagger**.

## Bootstrap del primer administrador

No hay usuario `admin` con contraseña por defecto en código. Opciones:

1. **Migración/seed idempotente**: crear un endpoint protegido por una clave de bootstrap (variable de entorno) que solo funcione si no existe ningún usuario, y cree el primer admin con contraseña segura.
2. **Script SQL/seed**: ejecutar un script idempotente que inserte el primer usuario (hash BCrypt) solo si la tabla está vacía.

Se documentará en el README principal cuando se implemente.

## Decisiones técnicas (resumen)

- **PKs UUID**: entidades principales usan `Guid` para no exponer secuenciales y facilitar distribución.
- **Refresh tokens**: almacenados en BD, rotatorios y revocables; acceso de corta duración (ej. 15 min).
- **Sin EnsureCreated()**: solo migraciones EF Core para esquema.
- **MediatR**: comandos y queries en Application; validación con FluentValidation en pipeline.
- **Result pattern**: respuestas de dominio/aplicación consistentes; middleware global de excepciones devuelve ProblemDetails.
- **Ownership**: todas las consultas de datos de usuario deben filtrar por `UserId` (a implementar en repositorios/handlers).

## Implementado (Fases 2–4)

- CRUD Categorías, Cuentas y Transacciones con ownership y paginación (`PagedResult<T>`).
- Registro de usuario y `GET /api/v1/users/me`.
- Dashboard con KPIs y transacciones recientes.
- Health checks (`/health`, `/health/ready`) y security headers (X-Content-Type-Options, X-Frame-Options, Referrer-Policy).

## Próximos pasos opcionales

- Presupuestos y metas de ahorro (entidades/CRUD).
- Reportes (PDF/Excel).
- Auditoría (AuditLog en create/update/delete), correlation ID, rate limiting en login/registro.
- CI/CD (GitHub Actions ya preparado en `.github/workflows`).

## Referencias

- ADR: `docs/ADR-001-architectural-redesign.md`
- `.env.example` en la raíz del repositorio.
