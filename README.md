# AuthService

AuthService is a modular, PostgreSQL-backed authentication and authorization service built with .NET 9 and Entity Framework Core. It showcases how I structure a small but production-minded service with clean layers, DI-driven application code, EF Core migrations, JWT issuance, multi-tenant scoping, and container-friendly packaging. The solution is organized into distinct projects:

- `AuthService.Domain` – core domain models (Users, Roles, Permissions, RefreshTokens), value objects, domain events, and result types.
- `AuthService.Application` – application commands and handlers (register/login, change password, activate/deactivate, refresh tokens, create roles/permissions, assign roles/permissions) plus dependency injection wiring for handlers.
- `AuthService.Infrastructure` – EF Core DbContext, PostgreSQL provider setup, repositories, JWT token service, design-time DbContext factory, and entity configurations/migrations.
- `AuthService.Api` – ASP.NET Core minimal API hosting controllers for auth, users, roles, and permissions with request DTOs and OpenAPI support.

## What it demonstrates
- Pragmatic DDD-lite structure (Domain/Application/Infrastructure/Api separation).
- Tenant model with activation/deactivation and tenant-scoped users/roles.
- User registration, login, and password change flows with salted hashing.
- JWT access + refresh tokens with configurable lifetimes.
- Role and permission management with many-to-many mappings.
- EF Core migrations and PostgreSQL setup with design-time DbContext factory.
- Containerization with multi-arch Dockerfile (linux/amd64, linux/arm64).

## Highlights for reviewers
- Clear layering and DI wiring to keep controllers thin and handlers testable.
- Simple Result/Error pattern for predictable API responses.
- Security basics covered: hashed passwords, token expirations, refresh flow.
- Ready-to-run Docker image and sample commands to see it live quickly.


## API Overview (all POST)
- `POST /api/tenants` – `{ name, description? }`
- `POST /api/tenants/activate` / `deactivate` – `{ tenantId }`
- `POST /api/tenants/{tenantId}/products` – `{ productId }`
- `POST /api/products` – `{ name, description? }`
- `POST /api/auth/register` – `{ firstName, lastName, email, password, tenantId }`
- `POST /api/auth/login` – `{ email, password, tenantId }`
- `POST /api/auth/refresh` – `{ refreshToken }`
- `POST /api/auth/change-password` – `{ userId, currentPassword, newPassword }`
- `POST /api/auth/activate` / `POST /api/auth/deactivate` – `{ userId }`
- `POST /api/users/{userId}/roles` – `{ roleId }`
- `POST /api/roles` – `{ name, description?, tenantId }`
- `POST /api/roles/{roleId}/permissions` – `{ permissionId }`
- `POST /api/permissions` – `{ name, description? }`

Responses follow a simple result pattern: success returns data (or 204 for void), failures return `400` with an error code and message.
