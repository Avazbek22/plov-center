# Plov Center Backend

Production-ready ASP.NET Core Web API backend for the Plov Center cafe website and admin panel.  
Scope: public showcase pages, public menu, admin authentication, CMS-style content management, category/dish CRUD, and image uploads.

## Stack

- .NET 10 (`net10.0`)
- ASP.NET Core Web API with controllers
- PostgreSQL
- Entity Framework Core (Code First)
- JWT Bearer authentication
- Clean Architecture
- CQRS with feature-based request handlers
- Swagger / OpenAPI

## Solution Structure

- `src/Domain`  
  Domain entities and core model behavior.
- `src/Application`  
  Use cases, validation, CQRS dispatcher, abstractions, and business rules.
- `src/Application.Contract`  
  External request/response DTOs and API-facing contracts.
- `src/Infrastructure`  
  EF Core, repositories, JWT implementation, password hashing, file storage, seeding, and migrations.
- `src/WebApi`  
  Controllers, middleware, auth/cors/swagger configuration, static files, and startup pipeline.

## Implemented Features

- Admin login with JWT
- Current admin endpoint
- Protected admin endpoints
- Category management
  - list, get, create, update, delete
  - visibility toggle
  - reorder for drag-and-drop scenarios
- Dish management
  - list, get, create, update, delete
  - visibility toggle
  - optional filtering by category
- Site content management
  - public content endpoint
  - admin content endpoint
  - update About section
  - update Contacts section
- Public menu endpoint
  - visible categories only
  - visible dishes only
  - sorted output
- Image uploads
  - dish and about-page images
  - `jpg`, `jpeg`, `png` only
  - max file size `5 MB`
  - files stored on local disk under `src/WebApi/wwwroot/uploads`
- Global error handling with consistent JSON responses
- Swagger with JWT support
- EF Core migrations
- Startup migration + seed pipeline

## Business Rules

- Category name is required.
- Dish name is required.
- Dish must belong to an existing category.
- Dish price must be `>= 0`.
- Dish description is optional but length-limited.
- Hidden categories and dishes are excluded from the public menu.
- A category with existing dishes cannot be deleted.
- Uploads larger than `5 MB` are rejected.
- Only `jpg`, `jpeg`, and `png` uploads are accepted.
- All validation is enforced on the backend regardless of frontend checks.

## Local Development Setup

### 1. Prerequisites

- .NET SDK `10.0.104` or compatible `10.0.x`
- PostgreSQL running locally

This repository includes:

- `global.json` to pin the SDK family
- `dotnet-tools.json` with local `dotnet-ef`

Restore local tools:

```bash
dotnet tool restore
```

### 2. PostgreSQL

Default local configuration is already set in [`src/WebApi/appsettings.json`](/C:/Users/avazb/RiderProjects/plov-center/src/WebApi/appsettings.json):

```json
"ConnectionStrings": {
  "Postgres": "Host=localhost;Port=5432;Database=plov_center;Username=postgres;Password=postgres"
}
```

You can override any setting via environment variables, for example:

```powershell
$env:ConnectionStrings__Postgres="Host=localhost;Port=5432;Database=plov_center;Username=postgres;Password=postgres"
$env:Jwt__SigningKey="replace-with-a-long-random-secret"
$env:SeedAdmin__Password="replace-me"
```

### 3. Restore and Build

```bash
dotnet restore
dotnet build PlovCenter.sln
```

### 4. Apply Migrations

```bash
dotnet dotnet-ef database update --project src/Infrastructure/PlovCenter.Infrastructure.csproj --startup-project src/WebApi/PlovCenter.WebApi.csproj
```

If you need to create a new migration later:

```bash
dotnet dotnet-ef migrations add <MigrationName> --project src/Infrastructure/PlovCenter.Infrastructure.csproj --startup-project src/WebApi/PlovCenter.WebApi.csproj --output-dir Persistence/Migrations
```

### 5. Run the API

```bash
dotnet run --project src/WebApi/PlovCenter.WebApi.csproj --launch-profile http
```

Useful URLs:

- Swagger UI: `http://localhost:5288/swagger`
- Public menu: `http://localhost:5288/api/public/menu`
- Public content: `http://localhost:5288/api/public/content`

## Seeded Admin

Default development admin comes from configuration:

- Username: `admin`
- Password: `Admin123!`

These values are intended for local development only. Override them before using the project anywhere outside a local machine.

## Main API Routes

### Public

- `GET /api/public/menu`
- `GET /api/public/content`

### Admin Auth

- `POST /api/admin/auth/login`
- `GET /api/admin/auth/me`

### Admin Categories

- `GET /api/admin/categories`
- `GET /api/admin/categories/{categoryId}`
- `POST /api/admin/categories`
- `PUT /api/admin/categories/{categoryId}`
- `PATCH /api/admin/categories/{categoryId}/visibility`
- `PUT /api/admin/categories/reorder`
- `DELETE /api/admin/categories/{categoryId}`

### Admin Dishes

- `GET /api/admin/dishes`
- `GET /api/admin/dishes/{dishId}`
- `POST /api/admin/dishes`
- `PUT /api/admin/dishes/{dishId}`
- `PATCH /api/admin/dishes/{dishId}/visibility`
- `DELETE /api/admin/dishes/{dishId}`

### Admin Content

- `GET /api/admin/content`
- `PUT /api/admin/content/about`
- `PUT /api/admin/content/contacts`

### Admin Uploads

- `POST /api/admin/uploads/image`

Multipart form fields:

- `area` = `Dish` or `About`
- `file` = image file

## Configuration Sections

- `ConnectionStrings:Postgres`
- `Jwt`
- `Cors`
- `SeedAdmin`
- `FileStorage`

## Files and Uploads

- Physical storage path: [`src/WebApi/wwwroot/uploads`](/C:/Users/avazb/RiderProjects/plov-center/src/WebApi/wwwroot/uploads)
- Public file URL format: `/uploads/{relative-path}`
- Uploaded file names are randomized
- Only relative file paths are stored in the database

## Migrations and Seeding

On application startup:

- pending EF migrations are applied
- the configured admin account is created or updated
- required site content keys are ensured

Known content keys:

- `about.text`
- `about.photo`
- `contacts.address`
- `contacts.phone`
- `contacts.hours`
- `contacts.map_embed`

## Verification Performed

The repository was verified in this environment with:

- `dotnet restore`
- `dotnet build PlovCenter.sln`
- `dotnet ef migrations add InitialCreate`
- `dotnet dotnet-ef database update`
- `dotnet run --project src/WebApi/PlovCenter.WebApi.csproj --launch-profile http`
- Swagger availability check
- Admin login and `me` endpoint
- Category and dish creation
- Conflict on category deletion when dishes exist
- Image upload and static file serving
- Public menu and content responses

## Notes

- The project intentionally does not include orders, carts, delivery, payments, customer accounts, tests, CI/CD, or unrelated future modules.
- The backend is ready for future extension without changing the architectural base.
