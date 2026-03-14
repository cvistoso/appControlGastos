# ADR-001: Rediseño arquitectónico y evolución del sistema

## Estado
Aceptado

## Contexto

El sistema "Expense Control App" actual es una aplicación monolítica con:
- Un único proyecto ASP.NET Core con Controllers, Data, Models, Services y wwwroot.
- Uso de `EnsureCreated()` para la base de datos.
- Credenciales por defecto (admin/admin123) en seed.
- JWT sin refresh tokens ni revocación.
- Sin separación clara de capas ni CQRS.
- Sin auditoría, sin rate limiting ni políticas de seguridad avanzadas.
- Frontend SPA con JavaScript vanilla poco modular.

## Diagnóstico de debilidades

### Arquitectura
- **Acoplamiento**: Lógica de negocio en servicios que dependen directamente de EF y de la API.
- **Sin capas**: No existe Domain puro ni Application como capa de casos de uso.
- **EnsureCreated()**: Inadecuado para producción; no hay historial de esquema ni migraciones controladas.
- **Sin CQRS**: Lecturas y escrituras mezcladas; difícil optimizar y escalar.
- **Sin Result pattern**: Errores devueltos de forma inconsistente.

### Seguridad
- **Credenciales por defecto**: admin@expensecontrol.com / admin123 en seed.
- **JWT sin refresh**: Solo access token; no hay revocación ni rotación.
- **Secretos en appsettings**: JWT SecretKey en archivos versionados.
- **Sin rate limiting**: Login/registro expuestos a fuerza bruta.
- **Sin confirmación de email** ni recuperación de contraseña.
- **CORS AllowAll**: Inadecuado para producción.
- **Sin security headers** (X-Content-Type-Options, etc.).

### Datos
- **PKs integer**: Menos adecuado para distribución y seguridad que UUID.
- **Sin soft delete** en transacciones.
- **Sin auditoría**: No se registra quién/cuándo modificó qué.
- **Sin created_by/updated_by** en varias entidades.
- **Timezone**: Riesgo de mezclar UTC y local.

### Backend
- **Entidades expuestas**: DTOs no separados claramente; riesgo de exponer entidades EF.
- **Validación**: DataAnnotations en modelos; sin FluentValidation centralizado.
- **Sin middleware global de excepciones**: Errores no normalizados.
- **Sin versionado de API**: Rutas /api/... sin versión.
- **Paginación/filtros**: No estandarizados.

### Frontend / UX
- **Monolítico**: Un solo app.js grande; sin módulos ni servicios separados.
- **Sin manejo robusto de errores** ni estados de carga/vacío consistentes.
- **Sin refresh token flow** ni renovación automática.
- **Accesibilidad y diseño** mejorables.

### Operación / DevOps
- **Logging**: Básico; sin Serilog ni correlation ID.
- **Sin health checks** diferenciados (liveness/readiness).
- **CI/CD**: No documentado ni implementado.
- **Migraciones**: No usadas; EnsureCreated en arranque.

---

## Mejoras propuestas (agrupadas)

### Arquitectura
- Clean Architecture con Domain, Application, Infrastructure, Api, Shared.
- CQRS ligero con MediatR.
- Result pattern para errores de dominio/aplicación.
- Repositorios solo donde aporten valor; interfaces en Application.
- Configuración por módulos y extensiones de DI.
- Eliminar EnsureCreated(); usar solo migraciones EF Core.

### Seguridad
- Secrets por variables de entorno / User Secrets / Secret Manager.
- JWT access (corta duración) + refresh tokens rotatorios y revocables.
- Sin credenciales por defecto débiles; bootstrap seguro del primer admin.
- Rate limiting en login, registro y endpoints sensibles.
- CORS restrictivo por ambiente.
- Security headers (X-Content-Type-Options, X-Frame-Options, Referrer-Policy, CSP).
- Políticas de contraseña y confirmación de correo.
- Validación de ownership en todas las consultas.
- Swagger solo en desarrollo/no producción.

### Datos
- PKs UUID (Guid) para entidades principales.
- created_at, updated_at, deleted_at; created_by, updated_by donde aplique.
- Soft delete en transacciones y categorías.
- Tabla de auditoría y correlation ID.
- Índices y unique constraints bien definidos.
- UTC en backend; conversión en frontend.
- Seeds idempotentes y seguros.

### Backend
- API versionada /api/v1/...
- DTOs separados; nunca exponer entidades EF.
- FluentValidation y middleware global de excepciones.
- Paginación estándar (page, pageSize, total, totalPages).
- OpenAPI documentado con ejemplos.
- ProblemDetails para errores.
- Roles (Admin, User) y políticas de autorización.

### Frontend / UX
- Módulos: auth, api client, state, components.
- Manejo de errores, loaders, empty states, toasts.
- Refresh token y renovación automática.
- Pantallas completas: login, registro, recuperar contraseña, dashboard, transacciones CRUD, categorías, cuentas, presupuestos, reportes, perfil.
- Filtros guardables, tablas paginadas, confirmaciones, responsive, accesibilidad básica.

### Operación / DevOps
- Logging estructurado (Serilog), Correlation ID.
- Health checks (app, db).
- Dockerfile multi-stage, docker-compose por ambiente.
- .env.example y appsettings por ambiente.
- Migraciones en arranque controlado o job dedicado.
- GitHub Actions: build, test, lint, publish.
- README técnico y scripts (run, migrate, seed, test).

---

## Decisiones

1. **Nueva estructura bajo `src/`**: Se crea `src/` con proyectos ExpenseControl.Domain, .Application, .Infrastructure, .Api, .Shared; y `tests/` para unit e integration. El código actual puede coexistir hasta completar la migración.
2. **UUID como PK**: Se usan Guids para User, Transaction, Category, Account, etc., para mejor distribución y no exponer secuenciales.
3. **MediatR para CQRS**: Comandos y queries en Application; handlers en Application o Infrastructure según dependencias.
4. **Refresh tokens en BD**: Tabla RefreshTokens con revocación y rotación.
5. **Migraciones únicas**: No usar EnsureCreated() en la aplicación principal; solo migraciones.
6. **Bootstrap del primer admin**: Flujo documentado (variable de entorno o endpoint protegido una sola vez) sin contraseña por defecto en código.

---

## Consecuencias

- Mayor esfuerzo inicial de estructura y migración.
- Mejor mantenibilidad, seguridad y preparación para producción.
- Equipo debe seguir convenciones de capas y CQRS.
- Migración del frontend puede ser progresiva (mantener SPA actual mejorado o migrar a Razor/otro stack más adelante).
