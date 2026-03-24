# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

PlovCenter — backend API for a cafe website with admin panel. .NET 10, ASP.NET Core, PostgreSQL via EF Core (Npgsql), MediatR + FluentValidation + JWT auth.

## Commands

```bash
# Build
dotnet build

# Run (applies migrations + seeds on startup)
dotnet run --project src/WebApi

# EF Core migrations (run from repo root)
dotnet ef migrations add <Name> --project src/Infrastructure --startup-project src/WebApi
dotnet ef database update --project src/Infrastructure --startup-project src/WebApi
```

No test projects exist yet.

## Architecture

Clean Architecture with 5 projects, dependency flows top-to-bottom:

```
WebApi -> Application, Application.Contract, Infrastructure
Application -> Domain, Application.Contract
Infrastructure -> Domain, Application, Application.Contract
Application.Contract -> (MediatR.Contracts only)
Domain -> (no dependencies)
```

**Domain** — entities with `AuditableEntity` base (Id, CreatedUtc, UpdatedUtc). Entities: `Category`, `Dish`, `AdminUser`, `SiteContentEntry`.

**Application.Contract** — MediatR command/query DTOs and response records. Separated from Application so WebApi references contracts without pulling in handlers.

**Application** — MediatR handlers, FluentValidation validators, mappings. Organized by feature: `Features/{Auth,Categories,Dishes,Content,Menu,Uploads}` each containing `Commands/`, `Queries/`, `Mappings/`. Validation runs via `ValidationBehavior<,>` pipeline behavior — validators throw `RequestValidationException` automatically.

**Infrastructure** — EF Core `ApplicationDbContext`, entity configurations, services (JWT, password hashing, file storage, current user). `DatabaseInitialization.ApplyMigrationsAndSeedAsync()` runs on startup — auto-migrates and seeds admin user + site content entries.

**WebApi** — controllers split into `Controllers/Admin/` (require `AdminAccess` policy) and `Controllers/Public/`. Global exception middleware maps `AppException` subtypes to structured `ApiErrorResponse`. Swagger at `/swagger`.

## Key Patterns

Controllers receive commands/queries from `Application.Contract`, send via `IMediator`. Handlers in `Application` do the work using `IApplicationDbContext` (no repository abstraction — direct `DbSet` access).

Custom exceptions hierarchy: `AppException` (abstract) -> `NotFoundException`, `ConflictException`, `ForbiddenException`, `UnauthorizedException`, `RequestValidationException`. `GlobalExceptionHandlingMiddleware` catches all and writes `ApiErrorResponse`.

String normalization: handlers call `.NormalizeTrimmed()` / `.NormalizeOptional()` on inputs.

File uploads: `IFileStorageService` stores to local filesystem under `wwwroot/uploads/`, served as static files.

## Configuration

Connection string: `ConnectionStrings:Postgres` in `appsettings.json`. JWT settings under `Jwt` section. Seed admin under `SeedAdmin`. CORS origins under `Cors:AllowedOrigins`. File storage root under `FileStorage:RootPath`. Secrets via User Secrets (id: `4d03aec6-0439-4be8-99f6-7b2fe22d0378`).

## Conventions

- File-scoped namespaces, primary constructors, `sealed` on all non-abstract classes
- No repositories — handlers query `IApplicationDbContext` directly
- Each command/query has a matching validator (same folder, `*Validator.cs`)
- Response mappings in `Mappings/*Mappings.cs` as extension methods
- Admin routes: `api/admin/{resource}`, public routes: `api/public/{resource}`

## Active Technologies
- TypeScript 5.9, React 19, Node.js 20+ + Vite 8, @mui/material 7, react-router-dom 7, @tanstack/react-query 5, react-hook-form 7, zod 4, notistack
- localStorage для JWT-токена (001-admin-frontend-setup)

## Recent Changes
- 002-admin-crud-panel: Added notistack for toast notifications, CRUD pages for categories/dishes/content
- 001-admin-frontend-setup: Added TypeScript, React 19, Vite 8, MUI 7, React Router 7, TanStack Query 5, RHF 7, Zod 4
