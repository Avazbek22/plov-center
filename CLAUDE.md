# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

PlovCenter — backend API + admin panel for a cafe website. Two codebases in one repo: .NET 10 backend (Clean Architecture) and React 19 admin SPA.

## Commands

```bash
# Backend — build
dotnet build

# Backend — run (auto-migrates + seeds on startup)
dotnet run --project src/WebApi

# Backend — EF Core migrations (from repo root)
dotnet ef migrations add <Name> --project src/Infrastructure --startup-project src/WebApi
dotnet ef database update --project src/Infrastructure --startup-project src/WebApi

# Frontend — dev server (proxies /api to localhost:5288)
cd front && npm run dev

# Frontend — build
cd front && npm run build

# Frontend — lint
cd front && npm run lint
```

No test projects exist yet.

## Architecture

### Backend — Clean Architecture

5 .NET projects, dependency flows top-to-bottom:

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

**WebApi** — controllers split into `Controllers/Admin/` (require `AdminAccess` policy) and `Controllers/Public/`. Global exception middleware maps `AppException` subtypes to structured `ApiErrorResponse`. Swagger at `/swagger`. Backend runs on port 5288.

### Frontend — React Admin SPA

Located in `front/`. Vite dev server on port 3000, proxies `/api` to backend at `localhost:5288`.

```
front/src/
  api/          — apiFetch wrapper + per-resource API functions (categories, dishes, content, uploads, auth)
  auth/         — AuthProvider context + ProtectedRoute
  components/   — layout/ (AdminLayout, Sidebar), shared/ (ConfirmDialog, ImageUpload)
  hooks/        — TanStack Query hooks per resource (use-categories, use-dishes, use-content)
  pages/        — Landing, NotFound, admin/ (Login, Dashboard, Categories, Dishes, Content)
  theme/        — MUI theme config
  types/        — TypeScript types mirroring backend DTOs
```

**Data flow**: `api/*.ts` (apiFetch calls) -> `hooks/use-*.ts` (TanStack Query wrappers with cache invalidation + notistack toasts) -> `pages/admin/*.tsx` (consume hooks).

`apiFetch` in `api/client.ts` auto-attaches JWT from localStorage, handles 401 by clearing auth and redirecting to `/admin/login`.

## Key Patterns

Controllers receive commands/queries from `Application.Contract`, send via `IMediator`. Handlers in `Application` do the work using `IApplicationDbContext` (no repository abstraction — direct `DbSet` access).

Custom exceptions hierarchy: `AppException` (abstract) -> `NotFoundException`, `ConflictException`, `ForbiddenException`, `UnauthorizedException`, `RequestValidationException`. `GlobalExceptionHandlingMiddleware` catches all and writes `ApiErrorResponse`.

String normalization: handlers call `.NormalizeTrimmed()` / `.NormalizeOptional()` on inputs.

File uploads: `IFileStorageService` stores to local filesystem under `wwwroot/uploads/`, served as static files.

Frontend hooks pattern: each `use-*.ts` exports a query hook + mutation hooks, handles errors via `handleMutationError` helper that shows notistack toast with `ApiError.response.message`. Mutations invalidate query cache on success.

## Configuration

Connection string: `ConnectionStrings:Postgres` in `appsettings.json`. JWT settings under `Jwt` section. Seed admin under `SeedAdmin`. CORS origins under `Cors:AllowedOrigins`. File storage root under `FileStorage:RootPath`. Secrets via User Secrets (id: `4d03aec6-0439-4be8-99f6-7b2fe22d0378`).

Frontend path alias: `@` -> `front/src/` (configured in vite.config.ts + tsconfig.app.json).

## Conventions

- File-scoped namespaces, primary constructors, `sealed` on all non-abstract classes
- No repositories — handlers query `IApplicationDbContext` directly
- Each command/query has a matching validator (same folder, `*Validator.cs`)
- Response mappings in `Mappings/*Mappings.cs` as extension methods
- Admin routes: `api/admin/{resource}`, public routes: `api/public/{resource}`
- Frontend API functions return typed promises, hooks wrap them in TanStack Query
- JWT stored in localStorage under key `plov-center-auth`

## Tech Stack

**Backend**: .NET 10, ASP.NET Core, PostgreSQL + EF Core (Npgsql), MediatR, FluentValidation, JWT Bearer auth

**Frontend**: TypeScript 5.9, React 19, Vite 8, MUI 7 (@mui/material), react-router-dom 7, @tanstack/react-query 5, react-hook-form 7 + @hookform/resolvers, zod 4, notistack 3
