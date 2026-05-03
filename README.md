**Resumen y cobertura actual (servicio `Taller_Mecanico_Users`)
**
- **Dominio:** Entidad `UsuarioLogin`, ValueObjects y `ErrorCodes` implementados. (Ver Domain folder)
- **Persistencia (Data):** `UsuarioLoginRepository` implementado usando `DbConnection` y SQL directo; operaciones básicas: GetByEmail, GetAll, GetById, Add, Update.
- **Casos de uso (UseCases):** `ChangePasswordUseCase` presente. Otros UseCases básicos listados pero incompletos (crear usuario, reset, listados avanzados).
- **Framework:** `ISqlConnectionFactory` (devuelve `DbConnection`), `IAuthenticationHelper` y DTOs soportan desacoplamiento. (Ver Framework folder)
- **App:** `AuthController` con `POST /api/auth/login` (valida credenciales bcrypt y responde con datos básicos). `RequirePasswordChangeMiddleware` existe pero requiere que la autenticación (claims/session) esté implementada por el consumidor.
- **Infra / Docker:** `docker-compose.yml` y `App/Dockerfile` añadidos para ejecutar servicio contra el Postgres del monolito.

**Mapa respecto a los requerimientos del docente**
- Login + Manejo de Sesiones: Parcial — login funcional pero NO hay manejo de sesión (no se emite cookie ni token). Falta: JWT/Cookie-based sessions, expiración, refresh (si se requiere).
- Control de bitácora por sesiones: Faltante — no hay captura ni almacenado de eventos/auditoría al ejecutar CRUDs.
- CRUD tabla Usuario: Parcial — repositorio existe, pero no hay endpoints HTTP públicos para Create/Read/Update/Delete con validaciones y políticas solicitadas (username protocol, no mostrar password, etc.).
- Cambio de contraseña y Primer inicio: Parcial — `ChangePasswordUseCase` y middleware para forzar cambio están; falta UI/endpoint para cambiar contraseña y marcar primer inicio; falta política de contraseñas al validar.
- Generación de reportes (PDF/Excel + gráfico): Faltante — no implementado.
- Transacción y generación automática de reporte tras inserción: Faltante — requiere trigger en flujo de negocio.
- Muchos a Muchos: No evaluado — depende de relaciones requeridas (ej: usuarios/roles) que aún faltan.

**Qué debería haber (entregables mínimos por módulo)**
- **Domain:** entidades completas (`UsuarioLogin`, `UsuarioRole` si aplica), ValueObjects y reglas de negocio (validaciones de contraseña, username generator policy).
- **Ports:** repositorios (IUsuarioLoginRepository) y contratos para servicios auxiliares (IMailSender, IReportGenerator).
- **Data:** implementaciones Npgsql/DbCommand con transacciones y audit fields (createdBy, updatedBy, timestamps). Métodos para reporting queries.
- **UseCases:** CreateUser, UpdateUser, DeleteUser (soft delete), Login (issue session), ChangePassword, ForceChangeOnFirstLogin, GenerateReports (sync/async), Audit logging use case.
- **App:** Endpoints REST protegidos: `/api/auth/login`, `/api/users` (CRUD), `/api/users/change-password`, `/api/reports/*`. Middleware de autenticación/authorization, mail sender config, swagger (opcional).
- **Infra:** Docker compose + migraciones SQL / scripts init; README con pasos.

**Gaps prioritarios (para obtener la funcionalidad requerida)**
1. Implementar issuance de sesión (JWT or Cookie) en `AuthController` y middleware que populen claims para `RequirePasswordChangeMiddleware`.
2. Añadir endpoints CRUD en `App/Controllers/UsersController.cs` que usen UseCases y respeten políticas de password/username.
3. Implementar auditoría: cada operación Create/Update/Delete debe escribir un registro en bitácora y/o llenar campos `creadopor`, `actualizadopor`. Registrar origen por claim de sesión.
4. Implementar generación de reportes: Reporte1 (lista SQL entre tablas) y Reporte2 (resumen + gráfico). Exportar PDF y Excel.
5. Integración con la solución principal: documentar rutas y contratos y añadir pipeline para consumir `Auth` y `Users` desde la app principal.

**Tareas para 2 integrantes**

- **Integrante A — Reportes (Responsable: Reportes)**
  - **Objetivo:** Implementar los dos reportes solicitados y la exportación a PDF/Excel.
  - **Tareas concretas:**
    - Crear `IReportGenerator` en `Framework` y `ReportService` en `App`.
    - Implementar Reporte 1: consulta SQL que use al menos 2 tablas (ej: `usuariologin` JOIN `empleado/cliente`), endpoint `GET /api/reports/users-list` con filtros (fecha creación, rol). Exportar a PDF y XLSX.
    - Implementar Reporte 2: consulta resumida con dataset para gráfico (ej: usuarios por rol / activos vs inactivos). Generar gráfico (Chart) embebido en PDF y datos en Excel. Endpoint `GET /api/reports/users-summary` con filtros (rango fechas, rol).
    - Implementar generación automática: al insertar un usuario (dentro de la transacción de `AddAsync`), disparar la generación de un reporte sumario y guardarlo en `reports` (o enviar por mail). Debe ejecutarse en la misma transacción o en una operación post-commit garantizada (preferible: generar en la misma transacción y guardar metadatos).
  - **Criterios de aceptación:** PDF y XLSX descargables desde endpoints; pie de página con fecha/hora y usuario que generó; archivos con logo y colores. Reporte2 incluye gráfico. Implementado tests de integración para endpoints.
  - **Estimación:** 5–7 días.

- **Integrante B — Integración y core Users (Responsable: Integración)**
  - **Objetivo:** Completar integración del servicio con la solución principal y terminar features core (auth/session, CRUD, auditoría).
  - **Tareas concretas:**
    - Implementar emisión de sesiones: elegir JWT (recomendado) o cookies; emitir token en `AuthController` y añadir middleware `UseAuthentication` + `UseAuthorization`. Documentar contrato (claim names, expiración).
    - Crear `UsersController` con endpoints CRUD (`POST /api/users`, `GET /api/users`, `GET /api/users/{id}`, `PUT /api/users/{id}`, `DELETE /api/users/{id}`) que llamen a UseCases y al repositorio.
    - Implementar política de `username` y generación de contraseña segura: `GenerateUsername()` y `GenerateSecurePassword()`; envío de credenciales por correo (implementar `IMailSender` y una implementación de desarrollo `Smtp`/`PickupDirectory`).
    - Forzar primer inicio de sesión: marcar `requiereCambioPassword` al crear; el middleware debe redirigir/denegar hasta que cambie.
    - Implementar auditoría/bitácora: cada UseCase que modifica datos debe registrar en tabla `audit_logs` o poblar campos `creadopor/actualizadopor`.
    - Probar integración end-to-end desde la solución principal: documentar pasos para consumir la API.
  - **Criterios de aceptación:** Autenticación funcional con tokens; CRUD funcional y protegido por roles; pruebas E2E básicas; auditoría registrada en BD.
  - **Estimación:** 7–10 días.

**Artefactos a entregar**
- `Design/TEAM_TASKS.md` (este archivo)
- Endpoints documentados en README con ejemplos `curl` y Postman collection
- Migraciones SQL para tablas adicionales (`audit_logs`, `reports`)
- Tests de integración para login, change-password, creación de usuario y generación de reportes.

**Siguientes pasos inmediatos (próximo sprint)**
1. Integrante B: Implementar JWT issuance y middleware (2 días).
2. Integrante B: CRUD básico y auditoría (3 días).
3. Integrante A: Implementar reporte 1 y endpoint (3 días).
4. Integrante A: Implementar reporte 2 con gráfico y export a Excel (3 días).

---

Archivo generado: `Design/TEAM_TASKS.md` (en este repo).