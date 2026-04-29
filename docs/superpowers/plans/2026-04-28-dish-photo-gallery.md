# Dish Photo Gallery Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace the single-photo-per-dish model with an ordered multi-photo gallery (drag-n-drop admin editor + swipeable carousel in the public menu modal).

**Architecture:** New `DishPhoto` entity in a 1:N relationship with `Dish` (FK with `ON DELETE CASCADE`). Backend uses a "replace-all" save strategy on update — all existing photos for the dish are deleted and new ones inserted in a single transaction. Frontend admin uses `@dnd-kit` for drag-n-drop reordering; public menu uses `embla-carousel-react` for the slider inside the modal.

**Tech Stack:** .NET 10, EF Core (Npgsql), MediatR, FluentValidation; React 19, MUI 7, motion 12, react-hook-form 7, TanStack Query 5, `@dnd-kit/core` + `@dnd-kit/sortable` (new), `embla-carousel-react` (new).

**Testing note:** The project currently has no test projects (CLAUDE.md confirms this). Per spec section 8, automated tests are out of scope for this feature. Each task verifies via `dotnet build`, `npm run build`, and explicit manual smoke checks against the acceptance criteria in the spec.

**Spec:** `docs/superpowers/specs/2026-04-28-dish-photo-gallery-design.md`

---

## File Map

**New files:**
- `src/Domain/Entities/DishPhoto.cs`
- `src/Infrastructure/Persistence/Configurations/DishPhotoConfiguration.cs`
- `src/Infrastructure/Persistence/Migrations/<timestamp>_AddDishPhotos.cs` (generated)
- `src/Application.Contract/Dishes/DishPhotoInput.cs`
- `src/Application.Contract/Dishes/Responses/DishPhotoResponse.cs`
- `front/src/components/shared/DishGalleryEditor.tsx`
- `front/src/components/shared/DishCarousel.tsx`

**Modified files:**
- `src/Domain/Entities/Dish.cs` — remove `PhotoPath`, add `Photos`
- `src/Application/Common/Interfaces/Contexts/IApplicationDbContext.cs` — add `DishPhotos` DbSet
- `src/Infrastructure/Persistence/Contexts/ApplicationDbContext.cs` — add `DishPhotos` DbSet
- `src/Application.Contract/Dishes/Commands/CreateDishCommand.cs` — `PhotoPath` → `Photos`
- `src/Application.Contract/Dishes/Commands/UpdateDishCommand.cs` — `PhotoPath` → `Photos`
- `src/Application.Contract/Dishes/Responses/DishResponse.cs` — `PhotoPath` → `Photos`
- `src/Application.Contract/Menu/Responses/PublicMenuDishResponse.cs` — `PhotoPath` → `Photos`
- `src/Application/Features/Dishes/Commands/CreateDishCommandValidator.cs`
- `src/Application/Features/Dishes/Commands/UpdateDishCommandValidator.cs`
- `src/Application/Features/Dishes/Commands/CreateDishCommandHandler.cs`
- `src/Application/Features/Dishes/Commands/UpdateDishCommandHandler.cs`
- `src/Application/Features/Dishes/Queries/GetAdminDishesQueryHandler.cs`
- `src/Application/Features/Dishes/Queries/GetDishByIdQueryHandler.cs`
- `src/Application/Features/Menu/Queries/GetPublicMenuQueryHandler.cs`
- `src/Application/Features/Dishes/Mappings/DishResponseMappings.cs`
- `src/WebApi/Contracts/Admin/Dishes/UpdateDishRequest.cs`
- `src/WebApi/Controllers/Admin/AdminDishesController.cs` — update binding for `Photos`
- `front/src/types/dish.ts` — `photoPath` → `photos`, new `DishPhotoForm`
- `front/src/types/public.ts` — `photoPath` → `photos`
- `front/src/pages/admin/Dishes.tsx` — swap `ImageUpload` for `DishGalleryEditor`, update list table
- `front/src/pages/PublicMenu.tsx` — grid card cover + badge, modal carousel
- `front/src/public-menu.css` — new classes for carousel + badges
- `front/package.json` — add `@dnd-kit/*`, `embla-carousel-react`

---

## Task 1: Create DishPhoto entity

**Files:**
- Create: `src/Domain/Entities/DishPhoto.cs`

- [ ] **Step 1: Create the entity file**

```csharp
using PlovCenter.Domain.Common;

namespace PlovCenter.Domain.Entities;

public sealed class DishPhoto : AuditableEntity
{
    public Guid DishId { get; set; }

    public Dish? Dish { get; set; }

    public string RelativePath { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}
```

- [ ] **Step 2: Verify it compiles in isolation**

Run: `dotnet build src/Domain/Domain.csproj`
Expected: Build succeeded, 0 errors. (No project may exist named `Domain.csproj` — if so, run `dotnet build` from repo root and confirm Domain assembly compiles.)

- [ ] **Step 3: Commit**

```bash
git add src/Domain/Entities/DishPhoto.cs
git commit -m "feat(domain): add DishPhoto entity"
```

---

## Task 2: Update Dish entity (remove PhotoPath, add Photos)

**Files:**
- Modify: `src/Domain/Entities/Dish.cs`

- [ ] **Step 1: Replace the file contents**

```csharp
using PlovCenter.Domain.Common;

namespace PlovCenter.Domain.Entities;

public sealed class Dish : AuditableEntity
{
    public Guid CategoryId { get; set; }

    public Category? Category { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int SortOrder { get; set; }

    public bool IsVisible { get; set; }

    public List<DishPhoto> Photos { get; set; } = [];
}
```

- [ ] **Step 2: Build will fail — that's expected**

Run: `dotnet build`
Expected: errors about `PhotoPath` not existing in:
- `DishConfiguration.cs`
- `CreateDishCommandHandler.cs`
- `UpdateDishCommandHandler.cs`
- `GetAdminDishesQueryHandler.cs`
- `GetDishByIdQueryHandler.cs`
- `GetPublicMenuQueryHandler.cs`
- `DishResponseMappings.cs`
- `CreateDishCommandValidator.cs`
- `UpdateDishCommandValidator.cs`
These will be fixed in subsequent tasks. Do **not** commit yet — Tasks 1, 2, 3, and 4 are committed together at the end of Task 4 (when DbSet is wired up and EF configuration is in place).

---

## Task 3: Add DishPhotoConfiguration and remove PhotoPath from DishConfiguration

**Files:**
- Create: `src/Infrastructure/Persistence/Configurations/DishPhotoConfiguration.cs`
- Modify: `src/Infrastructure/Persistence/Configurations/DishConfiguration.cs`

- [ ] **Step 1: Create the photo configuration**

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Infrastructure.Persistence.Configurations;

internal sealed class DishPhotoConfiguration : IEntityTypeConfiguration<DishPhoto>
{
    public void Configure(EntityTypeBuilder<DishPhoto> builder)
    {
        builder.ToTable("dish_photos");

        builder.HasKey(static photo => photo.Id);

        builder.Property(static photo => photo.RelativePath)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(static photo => photo.SortOrder).IsRequired();
        builder.Property(static photo => photo.CreatedUtc).IsRequired();
        builder.Property(static photo => photo.UpdatedUtc).IsRequired();

        builder.HasOne(static photo => photo.Dish)
            .WithMany(static dish => dish.Photos)
            .HasForeignKey(static photo => photo.DishId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(static photo => new { photo.DishId, photo.SortOrder });
    }
}
```

- [ ] **Step 2: Remove PhotoPath line from DishConfiguration**

In `src/Infrastructure/Persistence/Configurations/DishConfiguration.cs`, delete this block:

```csharp
        builder.Property(static dish => dish.PhotoPath)
            .HasMaxLength(512);
```

The full updated file:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Infrastructure.Persistence.Configurations;

internal sealed class DishConfiguration : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
        builder.ToTable("dishes");

        builder.HasKey(static dish => dish.Id);

        builder.Property(static dish => dish.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(static dish => dish.Description)
            .HasMaxLength(2000);

        builder.Property(static dish => dish.Price)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(static dish => dish.SortOrder).IsRequired();
        builder.Property(static dish => dish.IsVisible).IsRequired();
        builder.Property(static dish => dish.CreatedUtc).IsRequired();
        builder.Property(static dish => dish.UpdatedUtc).IsRequired();

        builder.HasIndex(static dish => new { dish.CategoryId, dish.SortOrder });
    }
}
```

---

## Task 4: Add DishPhotos DbSet to context interface and concrete class

**Files:**
- Modify: `src/Application/Common/Interfaces/Contexts/IApplicationDbContext.cs`
- Modify: `src/Infrastructure/Persistence/Contexts/ApplicationDbContext.cs`

- [ ] **Step 1: Add `DishPhotos` to the interface**

Replace the file contents with:

```csharp
using Microsoft.EntityFrameworkCore;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Common.Interfaces.Contexts;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }

    DbSet<Dish> Dishes { get; }

    DbSet<DishPhoto> DishPhotos { get; }

    DbSet<AdminUser> AdminUsers { get; }

    DbSet<SiteContentEntry> SiteContentEntries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
```

- [ ] **Step 2: Add `DishPhotos` to ApplicationDbContext**

Replace the file contents with:

```csharp
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Infrastructure.Persistence.Contexts;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Dish> Dishes => Set<Dish>();

    public DbSet<DishPhoto> DishPhotos => Set<DishPhoto>();

    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    public DbSet<SiteContentEntry> SiteContentEntries => Set<SiteContentEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
```

- [ ] **Step 3: Build is still expected to fail because of contract/handler call sites**

Run: `dotnet build`
Expected: errors only in `Application` and `WebApi` projects (handlers, validators, mappings still reference `PhotoPath`). Domain + Infrastructure compile.

- [ ] **Step 4: Commit Tasks 1-4 together**

```bash
git add src/Domain/Entities/DishPhoto.cs src/Domain/Entities/Dish.cs \
        src/Infrastructure/Persistence/Configurations/DishPhotoConfiguration.cs \
        src/Infrastructure/Persistence/Configurations/DishConfiguration.cs \
        src/Application/Common/Interfaces/Contexts/IApplicationDbContext.cs \
        src/Infrastructure/Persistence/Contexts/ApplicationDbContext.cs
git commit -m "feat(domain): introduce DishPhoto 1:N relationship with Dish"
```

---

## Task 5: Update contracts — DishPhotoInput, DishPhotoResponse, command/response DTOs

**Files:**
- Create: `src/Application.Contract/Dishes/DishPhotoInput.cs`
- Create: `src/Application.Contract/Dishes/Responses/DishPhotoResponse.cs`
- Modify: `src/Application.Contract/Dishes/Commands/CreateDishCommand.cs`
- Modify: `src/Application.Contract/Dishes/Commands/UpdateDishCommand.cs`
- Modify: `src/Application.Contract/Dishes/Responses/DishResponse.cs`
- Modify: `src/Application.Contract/Menu/Responses/PublicMenuDishResponse.cs`

- [ ] **Step 1: Create `DishPhotoInput`**

```csharp
namespace PlovCenter.Application.Contract.Dishes;

public sealed record DishPhotoInput(string RelativePath, int SortOrder);
```

- [ ] **Step 2: Create `DishPhotoResponse`**

```csharp
namespace PlovCenter.Application.Contract.Dishes.Responses;

public sealed record DishPhotoResponse(Guid Id, string RelativePath, int SortOrder);
```

- [ ] **Step 3: Update `CreateDishCommand`**

Replace the file contents with:

```csharp
using MediatR;
using PlovCenter.Application.Contract.Dishes.Responses;

namespace PlovCenter.Application.Contract.Dishes.Commands;

public sealed class CreateDishCommand : IRequest<DishResponse>
{
    public Guid CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public IReadOnlyList<DishPhotoInput> Photos { get; set; } = [];

    public int SortOrder { get; set; }

    public bool IsVisible { get; set; }
}
```

- [ ] **Step 4: Update `UpdateDishCommand`**

Replace the file contents with:

```csharp
using MediatR;
using PlovCenter.Application.Contract.Dishes.Responses;

namespace PlovCenter.Application.Contract.Dishes.Commands;

public sealed class UpdateDishCommand : IRequest<DishResponse>
{
    public Guid DishId { get; set; }

    public Guid CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public IReadOnlyList<DishPhotoInput> Photos { get; set; } = [];

    public int SortOrder { get; set; }

    public bool IsVisible { get; set; }
}
```

- [ ] **Step 5: Update `DishResponse`**

Replace the file contents with:

```csharp
namespace PlovCenter.Application.Contract.Dishes.Responses;

public sealed record DishResponse(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    string Name,
    string? Description,
    decimal Price,
    IReadOnlyList<DishPhotoResponse> Photos,
    int SortOrder,
    bool IsVisible,
    DateTime CreatedUtc,
    DateTime UpdatedUtc);
```

- [ ] **Step 6: Update `PublicMenuDishResponse`**

Replace the file contents with:

```csharp
namespace PlovCenter.Application.Contract.Menu.Responses;

public sealed record PublicMenuDishResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    IReadOnlyList<string> Photos,
    int SortOrder);
```

- [ ] **Step 7: Build still expected to fail**

Run: `dotnet build`
Expected: errors in handlers, validators, mappings, controllers (next tasks). No errors in `Application.Contract` itself.

---

## Task 6: Update validators

**Files:**
- Modify: `src/Application/Features/Dishes/Commands/CreateDishCommandValidator.cs`
- Modify: `src/Application/Features/Dishes/Commands/UpdateDishCommandValidator.cs`

- [ ] **Step 1: Update `CreateDishCommandValidator`**

Replace the file contents with:

```csharp
using FluentValidation;
using PlovCenter.Application.Common.Validation;
using PlovCenter.Application.Contract.Dishes;
using PlovCenter.Application.Contract.Dishes.Commands;

namespace PlovCenter.Application.Features.Dishes.Commands;

public sealed class CreateDishCommandValidator : AbstractValidator<CreateDishCommand>
{
    public CreateDishCommandValidator()
    {
        RuleFor(static command => command.CategoryId).NotEmpty();

        RuleFor(static command => command.Name)
            .NotEmpty()
            .MaximumLength(ValidationRules.DishNameMaxLength);

        RuleFor(static command => command.Description)
            .MaximumLength(ValidationRules.DishDescriptionMaxLength);

        RuleFor(static command => command.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(static command => command.SortOrder)
            .GreaterThanOrEqualTo(0);

        RuleFor(static command => command.Photos)
            .NotNull()
            .Must(static photos => photos.Select(p => p.SortOrder).Distinct().Count() == photos.Count)
            .WithMessage("Photos must not contain duplicate sort orders.")
            .Must(static photos => photos.Count <= 50)
            .WithMessage("A dish may have at most 50 photos.");

        RuleForEach(static command => command.Photos).ChildRules(photo =>
        {
            photo.RuleFor(static p => p.RelativePath)
                .NotEmpty()
                .MaximumLength(512);

            photo.RuleFor(static p => p.SortOrder)
                .GreaterThanOrEqualTo(0);
        });
    }
}
```

- [ ] **Step 2: Update `UpdateDishCommandValidator`**

Replace the file contents with:

```csharp
using FluentValidation;
using PlovCenter.Application.Common.Validation;
using PlovCenter.Application.Contract.Dishes;
using PlovCenter.Application.Contract.Dishes.Commands;

namespace PlovCenter.Application.Features.Dishes.Commands;

public sealed class UpdateDishCommandValidator : AbstractValidator<UpdateDishCommand>
{
    public UpdateDishCommandValidator()
    {
        RuleFor(static command => command.DishId).NotEmpty();
        RuleFor(static command => command.CategoryId).NotEmpty();

        RuleFor(static command => command.Name)
            .NotEmpty()
            .MaximumLength(ValidationRules.DishNameMaxLength);

        RuleFor(static command => command.Description)
            .MaximumLength(ValidationRules.DishDescriptionMaxLength);

        RuleFor(static command => command.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(static command => command.SortOrder)
            .GreaterThanOrEqualTo(0);

        RuleFor(static command => command.Photos)
            .NotNull()
            .Must(static photos => photos.Select(p => p.SortOrder).Distinct().Count() == photos.Count)
            .WithMessage("Photos must not contain duplicate sort orders.")
            .Must(static photos => photos.Count <= 50)
            .WithMessage("A dish may have at most 50 photos.");

        RuleForEach(static command => command.Photos).ChildRules(photo =>
        {
            photo.RuleFor(static p => p.RelativePath)
                .NotEmpty()
                .MaximumLength(512);

            photo.RuleFor(static p => p.SortOrder)
                .GreaterThanOrEqualTo(0);
        });
    }
}
```

---

## Task 7: Update DishResponseMappings

**Files:**
- Modify: `src/Application/Features/Dishes/Mappings/DishResponseMappings.cs`

- [ ] **Step 1: Replace the file contents**

```csharp
using PlovCenter.Application.Contract.Dishes.Responses;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Features.Dishes.Mappings;

internal static class DishResponseMappings
{
    public static DishResponse ToDishResponse(this Dish dish, string? categoryName = null)
    {
        var photos = dish.Photos
            .OrderBy(static photo => photo.SortOrder)
            .Select(static photo => new DishPhotoResponse(photo.Id, photo.RelativePath, photo.SortOrder))
            .ToArray();

        return new DishResponse(
            dish.Id,
            dish.CategoryId,
            categoryName ?? dish.Category?.Name ?? string.Empty,
            dish.Name,
            dish.Description,
            dish.Price,
            photos,
            dish.SortOrder,
            dish.IsVisible,
            dish.CreatedUtc,
            dish.UpdatedUtc);
    }
}
```

---

## Task 8: Update CreateDishCommandHandler

**Files:**
- Modify: `src/Application/Features/Dishes/Commands/CreateDishCommandHandler.cs`

- [ ] **Step 1: Replace the file contents**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Extensions;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Application.Contract.Dishes.Commands;
using PlovCenter.Application.Contract.Dishes.Responses;
using PlovCenter.Application.Features.Dishes.Mappings;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Features.Dishes.Commands;

public sealed class CreateDishCommandHandler(
    IApplicationDbContext applicationDbContext,
    IDateTimeService dateTimeService) : IRequestHandler<CreateDishCommand, DishResponse>
{
    public async Task<DishResponse> Handle(CreateDishCommand request, CancellationToken cancellationToken)
    {
        var category = await applicationDbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        var utcNow = dateTimeService.UtcNow;

        var dish = new Dish
        {
            Id = Guid.NewGuid(),
            CategoryId = request.CategoryId,
            Name = request.Name.NormalizeTrimmed(),
            Description = request.Description.NormalizeOptional(),
            Price = request.Price,
            SortOrder = request.SortOrder,
            IsVisible = request.IsVisible,
            CreatedUtc = utcNow,
            UpdatedUtc = utcNow
        };

        foreach (var input in request.Photos.OrderBy(static p => p.SortOrder))
        {
            dish.Photos.Add(new DishPhoto
            {
                Id = Guid.NewGuid(),
                DishId = dish.Id,
                RelativePath = input.RelativePath.NormalizeTrimmed(),
                SortOrder = input.SortOrder,
                CreatedUtc = utcNow,
                UpdatedUtc = utcNow
            });
        }

        applicationDbContext.Dishes.Add(dish);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return dish.ToDishResponse(category.Name);
    }
}
```

---

## Task 9: Update UpdateDishCommandHandler with replace-all strategy

**Files:**
- Modify: `src/Application/Features/Dishes/Commands/UpdateDishCommandHandler.cs`

- [ ] **Step 1: Replace the file contents**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Extensions;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Common.Interfaces.Services;
using PlovCenter.Application.Contract.Dishes.Commands;
using PlovCenter.Application.Contract.Dishes.Responses;
using PlovCenter.Application.Features.Dishes.Mappings;
using PlovCenter.Domain.Entities;

namespace PlovCenter.Application.Features.Dishes.Commands;

public sealed class UpdateDishCommandHandler(
    IApplicationDbContext applicationDbContext,
    IDateTimeService dateTimeService) : IRequestHandler<UpdateDishCommand, DishResponse>
{
    public async Task<DishResponse> Handle(UpdateDishCommand request, CancellationToken cancellationToken)
    {
        var dish = await applicationDbContext.Dishes
            .Include(static item => item.Category)
            .Include(static item => item.Photos)
            .FirstOrDefaultAsync(item => item.Id == request.DishId, cancellationToken)
            ?? throw new NotFoundException("Dish was not found.");

        var category = await applicationDbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("Category was not found.");

        var utcNow = dateTimeService.UtcNow;

        dish.CategoryId = request.CategoryId;
        dish.Name = request.Name.NormalizeTrimmed();
        dish.Description = request.Description.NormalizeOptional();
        dish.Price = request.Price;
        dish.SortOrder = request.SortOrder;
        dish.IsVisible = request.IsVisible;
        dish.UpdatedUtc = utcNow;

        applicationDbContext.DishPhotos.RemoveRange(dish.Photos);
        dish.Photos.Clear();

        foreach (var input in request.Photos.OrderBy(static p => p.SortOrder))
        {
            dish.Photos.Add(new DishPhoto
            {
                Id = Guid.NewGuid(),
                DishId = dish.Id,
                RelativePath = input.RelativePath.NormalizeTrimmed(),
                SortOrder = input.SortOrder,
                CreatedUtc = utcNow,
                UpdatedUtc = utcNow
            });
        }

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return dish.ToDishResponse(category.Name);
    }
}
```

---

## Task 10: Update GetAdminDishesQueryHandler

**Files:**
- Modify: `src/Application/Features/Dishes/Queries/GetAdminDishesQueryHandler.cs`

- [ ] **Step 1: Replace the file contents**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Contract.Dishes.Queries;
using PlovCenter.Application.Contract.Dishes.Responses;

namespace PlovCenter.Application.Features.Dishes.Queries;

public sealed class GetAdminDishesQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetAdminDishesQuery, IReadOnlyCollection<DishResponse>>
{
    public async Task<IReadOnlyCollection<DishResponse>> Handle(GetAdminDishesQuery request, CancellationToken cancellationToken)
    {
        var query = applicationDbContext.Dishes
            .AsNoTracking()
            .AsQueryable();

        if (request.CategoryId.HasValue)
        {
            query = query.Where(item => item.CategoryId == request.CategoryId.Value);
        }

        return await query
            .OrderBy(item => item.Category!.SortOrder)
            .ThenBy(item => item.SortOrder)
            .ThenBy(item => item.Name)
            .Select(dish => new DishResponse(
                dish.Id,
                dish.CategoryId,
                dish.Category!.Name,
                dish.Name,
                dish.Description,
                dish.Price,
                dish.Photos
                    .OrderBy(photo => photo.SortOrder)
                    .Select(photo => new DishPhotoResponse(photo.Id, photo.RelativePath, photo.SortOrder))
                    .ToArray(),
                dish.SortOrder,
                dish.IsVisible,
                dish.CreatedUtc,
                dish.UpdatedUtc))
            .ToArrayAsync(cancellationToken);
    }
}
```

---

## Task 11: Update GetDishByIdQueryHandler

**Files:**
- Modify: `src/Application/Features/Dishes/Queries/GetDishByIdQueryHandler.cs`

- [ ] **Step 1: Replace the file contents**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Exceptions;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Contract.Dishes.Queries;
using PlovCenter.Application.Contract.Dishes.Responses;

namespace PlovCenter.Application.Features.Dishes.Queries;

public sealed class GetDishByIdQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetDishByIdQuery, DishResponse>
{
    public async Task<DishResponse> Handle(GetDishByIdQuery request, CancellationToken cancellationToken)
    {
        var dish = await applicationDbContext.Dishes
            .AsNoTracking()
            .Where(item => item.Id == request.DishId)
            .Select(dish => new DishResponse(
                dish.Id,
                dish.CategoryId,
                dish.Category!.Name,
                dish.Name,
                dish.Description,
                dish.Price,
                dish.Photos
                    .OrderBy(photo => photo.SortOrder)
                    .Select(photo => new DishPhotoResponse(photo.Id, photo.RelativePath, photo.SortOrder))
                    .ToArray(),
                dish.SortOrder,
                dish.IsVisible,
                dish.CreatedUtc,
                dish.UpdatedUtc))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Dish was not found.");

        return dish;
    }
}
```

---

## Task 12: Update GetPublicMenuQueryHandler

**Files:**
- Modify: `src/Application/Features/Menu/Queries/GetPublicMenuQueryHandler.cs`

- [ ] **Step 1: Replace the file contents**

```csharp
using MediatR;
using Microsoft.EntityFrameworkCore;
using PlovCenter.Application.Common.Interfaces.Contexts;
using PlovCenter.Application.Contract.Menu.Queries;
using PlovCenter.Application.Contract.Menu.Responses;

namespace PlovCenter.Application.Features.Menu.Queries;

public sealed class GetPublicMenuQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetPublicMenuQuery, PublicMenuResponse>
{
    public async Task<PublicMenuResponse> Handle(GetPublicMenuQuery request, CancellationToken cancellationToken)
    {
        var categories = await applicationDbContext.Categories
            .AsNoTracking()
            .Where(category => category.IsVisible)
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Name)
            .Select(category => new PublicMenuCategoryResponse(
                category.Id,
                category.Name,
                category.SortOrder,
                category.Dishes
                    .Where(static dish => dish.IsVisible)
                    .OrderBy(static dish => dish.SortOrder)
                    .ThenBy(static dish => dish.Name)
                    .Select(dish => new PublicMenuDishResponse(
                        dish.Id,
                        dish.Name,
                        dish.Description,
                        dish.Price,
                        dish.Photos
                            .OrderBy(photo => photo.SortOrder)
                            .Select(photo => photo.RelativePath)
                            .ToArray(),
                        dish.SortOrder))
                    .ToArray()))
            .ToArrayAsync(cancellationToken);

        return new PublicMenuResponse(categories);
    }
}
```

---

## Task 13: Update WebApi UpdateDishRequest and AdminDishesController

**Files:**
- Modify: `src/WebApi/Contracts/Admin/Dishes/UpdateDishRequest.cs`
- Modify: `src/WebApi/Controllers/Admin/AdminDishesController.cs`

- [ ] **Step 1: Update `UpdateDishRequest`**

Replace the file contents with:

```csharp
using PlovCenter.Application.Contract.Dishes;

namespace PlovCenter.WebApi.Contracts.Admin.Dishes;

public sealed class UpdateDishRequest
{
    public Guid CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public IReadOnlyList<DishPhotoInput> Photos { get; set; } = [];

    public int SortOrder { get; set; }

    public bool IsVisible { get; set; }
}
```

- [ ] **Step 2: Update `AdminDishesController.UpdateDishAsync` mapping**

In `src/WebApi/Controllers/Admin/AdminDishesController.cs`, locate the `UpdateDishAsync` method (lines 39-58) and replace its body. The full updated method:

```csharp
    [HttpPut("{dishId:guid}")]
    public Task<DishResponse> UpdateDishAsync(
        Guid dishId,
        [FromBody] UpdateDishRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDishCommand
        {
            DishId = dishId,
            CategoryId = request.CategoryId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Photos = request.Photos,
            SortOrder = request.SortOrder,
            IsVisible = request.IsVisible
        };

        return mediator.Send(command, cancellationToken);
    }
```

- [ ] **Step 3: Build the whole solution**

Run: `dotnet build`
Expected: Build succeeded, 0 errors, 0 warnings.

- [ ] **Step 4: Commit Tasks 5-13 together**

```bash
git add src/Application.Contract/Dishes/DishPhotoInput.cs \
        src/Application.Contract/Dishes/Responses/DishPhotoResponse.cs \
        src/Application.Contract/Dishes/Commands/CreateDishCommand.cs \
        src/Application.Contract/Dishes/Commands/UpdateDishCommand.cs \
        src/Application.Contract/Dishes/Responses/DishResponse.cs \
        src/Application.Contract/Menu/Responses/PublicMenuDishResponse.cs \
        src/Application/Features/Dishes/Commands/CreateDishCommandValidator.cs \
        src/Application/Features/Dishes/Commands/UpdateDishCommandValidator.cs \
        src/Application/Features/Dishes/Mappings/DishResponseMappings.cs \
        src/Application/Features/Dishes/Commands/CreateDishCommandHandler.cs \
        src/Application/Features/Dishes/Commands/UpdateDishCommandHandler.cs \
        src/Application/Features/Dishes/Queries/GetAdminDishesQueryHandler.cs \
        src/Application/Features/Dishes/Queries/GetDishByIdQueryHandler.cs \
        src/Application/Features/Menu/Queries/GetPublicMenuQueryHandler.cs \
        src/WebApi/Contracts/Admin/Dishes/UpdateDishRequest.cs \
        src/WebApi/Controllers/Admin/AdminDishesController.cs
git commit -m "feat(api): switch dish photos contract to ordered list of DishPhoto"
```

---

## Task 14: Generate AddDishPhotos EF migration

**Files:**
- Create: `src/Infrastructure/Persistence/Migrations/<timestamp>_AddDishPhotos.cs` (generated)
- Modify: `src/Infrastructure/Persistence/Migrations/ApplicationDbContextModelSnapshot.cs` (regenerated)

- [ ] **Step 1: Generate the migration**

Run from repo root:

```bash
dotnet ef migrations add AddDishPhotos --project src/Infrastructure --startup-project src/WebApi
```

Expected: Two new files added under `src/Infrastructure/Persistence/Migrations/`. The model snapshot is regenerated.

- [ ] **Step 2: Inspect the generated migration**

Open the new `<timestamp>_AddDishPhotos.cs`. Confirm `Up()` performs:

1. `migrationBuilder.DropColumn(name: "photo_path", table: "dishes");`
2. `migrationBuilder.CreateTable(name: "dish_photos", ...)` with columns `id`, `dish_id`, `relative_path` (max 512, not null), `sort_order`, `created_utc`, `updated_utc`, primary key `id`, foreign key `dish_id` → `dishes(id)` `ON DELETE CASCADE`
3. `migrationBuilder.CreateIndex(name: "IX_dish_photos_dish_id_sort_order", ...)`

Confirm `Down()` does the reverse (drop table, add `photo_path` column back).

If anything is off (e.g., wrong cascade delete), it indicates an issue in the configuration — go back to Task 3 and fix.

- [ ] **Step 3: Apply the migration to dev DB**

Run:

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/WebApi
```

Expected: "Done." with no errors. Existing photo data on `dishes.photo_path` is dropped (acceptable per spec).

- [ ] **Step 4: Verify schema in psql or via WebApi swagger**

Run a quick check using whatever local Postgres tooling is available:

```bash
psql "$YOUR_LOCAL_CONN_STRING" -c "\d dish_photos"
```

Expected output shows the table with the FK and index. If `psql` is not available, simply continue — the next task's runtime check will catch schema drift.

- [ ] **Step 5: Commit**

```bash
git add src/Infrastructure/Persistence/Migrations/
git commit -m "feat(db): migration AddDishPhotos — drop dishes.photo_path, add dish_photos table"
```

---

## Task 15: Backend smoke — run WebApi and exercise endpoints

**Files:** none (smoke only)

- [ ] **Step 1: Run the WebApi**

Run in a background-friendly terminal:

```bash
dotnet run --project src/WebApi
```

Expected: Server starts on `http://localhost:5288`. Migration auto-applies (already applied). Swagger available at `http://localhost:5288/swagger`.

- [ ] **Step 2: Hit the public menu endpoint**

```bash
curl -s http://localhost:5288/api/public/menu | jq '.categories[0].dishes[0]'
```

Expected: JSON includes a `photos` array (probably empty since DB is fresh post-migration). No `photoPath` field.

- [ ] **Step 3: Stop the server**

Send `Ctrl+C` to the dotnet process (or `kill` it).

- [ ] **Step 4: No commit — this task is verification only**

---

## Task 16: Update frontend types — dish.ts and public.ts

**Files:**
- Modify: `front/src/types/dish.ts`
- Modify: `front/src/types/public.ts`

- [ ] **Step 1: Replace `front/src/types/dish.ts`**

```ts
import { z } from 'zod';

export interface DishPhotoResponse {
  id: string;
  relativePath: string;
  sortOrder: number;
}

export interface DishResponse {
  id: string;
  categoryId: string;
  categoryName: string;
  name: string;
  description: string | null;
  price: number;
  photos: DishPhotoResponse[];
  sortOrder: number;
  isVisible: boolean;
  createdUtc: string;
  updatedUtc: string;
}

export interface DishPhotoForm {
  tempId: string;
  relativePath: string | null;
  sortOrder: number;
  uploading: boolean;
}

export interface DishFormData {
  categoryId: string;
  name: string;
  description: string | null;
  price: number;
  photos: DishPhotoForm[];
  sortOrder: number;
  isVisible: boolean;
}

const dishPhotoFormSchema = z.object({
  tempId: z.string().min(1),
  relativePath: z.string().nullable(),
  sortOrder: z.number().int().min(0),
  uploading: z.boolean(),
});

export const dishFormSchema = z.object({
  categoryId: z.string().min(1, 'Категория обязательна'),
  name: z.string().min(1, 'Название обязательно').max(200),
  description: z.string().max(2000).nullable(),
  price: z.number().positive('Цена должна быть больше 0'),
  photos: z.array(dishPhotoFormSchema).max(50, 'Не больше 50 фото'),
  sortOrder: z.number().int().min(0),
  isVisible: z.boolean(),
});
```

- [ ] **Step 2: Replace `front/src/types/public.ts`**

```ts
export interface PublicMenuDish {
  id: string;
  name: string;
  description: string | null;
  price: number;
  photos: string[];
  sortOrder: number;
}

export interface PublicMenuCategory {
  id: string;
  name: string;
  sortOrder: number;
  dishes: PublicMenuDish[];
}

export interface PublicMenu {
  categories: PublicMenuCategory[];
}

export interface PublicAbout {
  text: string | null;
  photoPath: string | null;
}

export interface PublicContacts {
  address: string | null;
  phone: string | null;
  hours: string | null;
  mapEmbed: string | null;
}

export interface PublicSiteContent {
  about: PublicAbout;
  contacts: PublicContacts;
}
```

- [ ] **Step 3: Type-check**

Run: `cd front && npx tsc --noEmit`
Expected: Errors only in `Dishes.tsx` and `PublicMenu.tsx` (they reference `dish.photoPath` which no longer exists). These will be fixed in Tasks 18 and 22.

---

## Task 17: Install drag-n-drop and carousel libraries

**Files:**
- Modify: `front/package.json` (via npm)

- [ ] **Step 1: Install packages**

Run:

```bash
cd front && npm install @dnd-kit/core @dnd-kit/sortable @dnd-kit/utilities embla-carousel-react
```

Expected: Packages added to `dependencies`. No vulnerability warnings worth blocking on. `package-lock.json` updates.

- [ ] **Step 2: Verify the dev server still starts**

Run: `cd front && npm run dev`
Expected: Vite starts on `:3000`. Stop with Ctrl+C — won't actually run pages until subsequent tasks finish.

- [ ] **Step 3: Commit**

```bash
git add front/package.json front/package-lock.json
git commit -m "chore(front): add @dnd-kit and embla-carousel-react"
```

---

## Task 18: Create DishGalleryEditor component

**Files:**
- Create: `front/src/components/shared/DishGalleryEditor.tsx`

- [ ] **Step 1: Create the file**

```tsx
import { useRef } from 'react'
import {
  DndContext,
  PointerSensor,
  KeyboardSensor,
  closestCenter,
  useSensor,
  useSensors,
  type DragEndEvent,
} from '@dnd-kit/core'
import {
  SortableContext,
  arrayMove,
  rectSortingStrategy,
  sortableKeyboardCoordinates,
  useSortable,
} from '@dnd-kit/sortable'
import { CSS } from '@dnd-kit/utilities'

import Box from '@mui/material/Box'
import CircularProgress from '@mui/material/CircularProgress'
import IconButton from '@mui/material/IconButton'
import CloseIcon from '@mui/icons-material/Close'
import StarIcon from '@mui/icons-material/Star'
import DragIndicatorIcon from '@mui/icons-material/DragIndicator'
import AddPhotoAlternateIcon from '@mui/icons-material/AddPhotoAlternate'

import { uploadImage } from '@/api/uploads'
import { imageUrl } from '@/utils/image-url'
import type { DishPhotoForm } from '@/types/dish'

interface DishGalleryEditorProps {
  value: DishPhotoForm[]
  onChange: (next: DishPhotoForm[]) => void
}

function generateTempId(): string {
  return `tmp-${Date.now()}-${Math.random().toString(36).slice(2, 10)}`
}

function reindex(photos: DishPhotoForm[]): DishPhotoForm[] {
  return photos.map((p, index) => ({ ...p, sortOrder: index }))
}

export default function DishGalleryEditor({ value, onChange }: DishGalleryEditorProps) {
  const inputRef = useRef<HTMLInputElement>(null)

  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 6 } }),
    useSensor(KeyboardSensor, { coordinateGetter: sortableKeyboardCoordinates }),
  )

  function handleFiles(files: FileList | null) {
    if (!files || files.length === 0) return

    const fileArray = Array.from(files)
    const startIndex = value.length

    const newPhotos: DishPhotoForm[] = fileArray.map((_, i) => ({
      tempId: generateTempId(),
      relativePath: null,
      sortOrder: startIndex + i,
      uploading: true,
    }))

    onChange([...value, ...newPhotos])

    fileArray.forEach((file, i) => {
      const tempId = newPhotos[i].tempId
      uploadImage(file, 'dish')
        .then((response) => {
          onChange(
            (currentValueRef.current ?? []).map((p) =>
              p.tempId === tempId
                ? { ...p, relativePath: response.relativePath, uploading: false }
                : p,
            ),
          )
        })
        .catch(() => {
          onChange((currentValueRef.current ?? []).filter((p) => p.tempId !== tempId))
        })
    })

    if (inputRef.current) inputRef.current.value = ''
  }

  // Keep a ref-mirror of the latest value so async upload callbacks
  // close over the freshest array (avoids stale-closure overwrites
  // when multiple parallel uploads finish).
  const currentValueRef = useRef(value)
  currentValueRef.current = value

  function handleRemove(tempId: string) {
    onChange(reindex(value.filter((p) => p.tempId !== tempId)))
  }

  function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event
    if (!over || active.id === over.id) return

    const oldIndex = value.findIndex((p) => p.tempId === active.id)
    const newIndex = value.findIndex((p) => p.tempId === over.id)
    if (oldIndex < 0 || newIndex < 0) return

    onChange(reindex(arrayMove(value, oldIndex, newIndex)))
  }

  return (
    <Box>
      <Box sx={{ fontSize: 12, fontWeight: 600, color: 'text.secondary', mb: 1 }}>
        Фото блюда
      </Box>

      <DndContext sensors={sensors} collisionDetection={closestCenter} onDragEnd={handleDragEnd}>
        <SortableContext items={value.map((p) => p.tempId)} strategy={rectSortingStrategy}>
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
            {value.map((photo, index) => (
              <SortableTile
                key={photo.tempId}
                photo={photo}
                isCover={index === 0}
                onRemove={() => handleRemove(photo.tempId)}
              />
            ))}
            <AddTile onClick={() => inputRef.current?.click()} />
          </Box>
        </SortableContext>
      </DndContext>

      <input
        ref={inputRef}
        type="file"
        accept="image/jpeg,image/png"
        hidden
        multiple
        onChange={(e) => handleFiles(e.target.files)}
      />

      <Box sx={{ fontSize: 11, color: 'text.disabled', mt: 1 }}>
        Перетащи, чтобы изменить порядок · первое фото = обложка · JPG/PNG, до 5 МБ
      </Box>
    </Box>
  )
}

interface SortableTileProps {
  photo: DishPhotoForm
  isCover: boolean
  onRemove: () => void
}

function SortableTile({ photo, isCover, onRemove }: SortableTileProps) {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
    id: photo.tempId,
  })

  const style: React.CSSProperties = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.4 : 1,
  }

  const src = photo.relativePath ? imageUrl(photo.relativePath) : null

  return (
    <Box
      ref={setNodeRef}
      style={style}
      sx={{
        position: 'relative',
        width: 96,
        height: 96,
        border: isCover ? '2px solid' : '1px solid',
        borderColor: isCover ? 'primary.main' : 'divider',
        borderRadius: 1,
        overflow: 'hidden',
        bgcolor: 'grey.100',
      }}
    >
      {src && (
        <Box
          component="img"
          src={src}
          alt=""
          sx={{ width: '100%', height: '100%', objectFit: 'cover' }}
        />
      )}

      {photo.uploading && (
        <Box sx={{
          position: 'absolute', inset: 0,
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          bgcolor: 'rgba(255,255,255,0.7)',
        }}>
          <CircularProgress size={28} />
        </Box>
      )}

      {isCover && !photo.uploading && (
        <Box sx={{
          position: 'absolute', top: 4, left: 4,
          bgcolor: 'primary.main', color: 'primary.contrastText',
          fontSize: 10, fontWeight: 600,
          px: 0.75, py: 0.25, borderRadius: 8,
          display: 'flex', alignItems: 'center', gap: 0.25,
        }}>
          <StarIcon sx={{ fontSize: 12 }} /> Обложка
        </Box>
      )}

      <IconButton
        size="small"
        onClick={onRemove}
        disabled={photo.uploading}
        sx={{
          position: 'absolute', top: 2, right: 2,
          bgcolor: 'rgba(0,0,0,0.55)', color: 'white',
          width: 22, height: 22,
          '&:hover': { bgcolor: 'rgba(0,0,0,0.75)' },
        }}
      >
        <CloseIcon sx={{ fontSize: 14 }} />
      </IconButton>

      <Box
        {...attributes}
        {...listeners}
        sx={{
          position: 'absolute', bottom: 0, left: 0, right: 0,
          height: 18,
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          color: 'rgba(255,255,255,0.85)',
          bgcolor: 'rgba(0,0,0,0.35)',
          cursor: 'grab',
          '&:active': { cursor: 'grabbing' },
        }}
      >
        <DragIndicatorIcon sx={{ fontSize: 14 }} />
      </Box>
    </Box>
  )
}

function AddTile({ onClick }: { onClick: () => void }) {
  return (
    <Box
      onClick={onClick}
      sx={{
        width: 96, height: 96,
        border: '2px dashed',
        borderColor: 'primary.light',
        borderRadius: 1,
        display: 'flex', flexDirection: 'column',
        alignItems: 'center', justifyContent: 'center',
        gap: 0.5, color: 'primary.main',
        cursor: 'pointer',
        bgcolor: 'background.paper',
        '&:hover': { bgcolor: 'action.hover' },
      }}
    >
      <AddPhotoAlternateIcon />
      <Box sx={{ fontSize: 11 }}>Загрузить</Box>
    </Box>
  )
}
```

- [ ] **Step 2: Verify it type-checks in isolation**

Run: `cd front && npx tsc --noEmit`
Expected: Errors should now exist only in `pages/admin/Dishes.tsx` and `pages/PublicMenu.tsx`. The new component itself compiles clean.

---

## Task 19: Wire DishGalleryEditor into admin Dishes page

**Files:**
- Modify: `front/src/pages/admin/Dishes.tsx`

- [ ] **Step 1: Update imports and DEFAULT_VALUES**

In `front/src/pages/admin/Dishes.tsx`:

Replace the import:

```tsx
import ImageUpload from '@/components/shared/ImageUpload';
```

with:

```tsx
import DishGalleryEditor from '@/components/shared/DishGalleryEditor';
```

Replace `DEFAULT_VALUES`:

```tsx
const DEFAULT_VALUES: DishFormData = {
  categoryId: '',
  name: '',
  description: null,
  price: 0,
  photos: [],
  sortOrder: 0,
  isVisible: true,
};
```

- [ ] **Step 2: Update `openEditDialog` to map server photos to form photos**

Replace the function:

```tsx
  function openEditDialog(dish: DishResponse) {
    setEditingDish(dish);
    form.reset({
      categoryId: dish.categoryId,
      name: dish.name,
      description: dish.description,
      price: dish.price,
      photos: dish.photos.map((p) => ({
        tempId: `srv-${p.id}`,
        relativePath: p.relativePath,
        sortOrder: p.sortOrder,
        uploading: false,
      })),
      sortOrder: dish.sortOrder,
      isVisible: dish.isVisible,
    });
    setDialogOpen(true);
  }
```

- [ ] **Step 3: Update `onSubmit` to strip client-only fields and reject pending uploads**

Replace the function:

```tsx
  async function onSubmit(data: DishFormData) {
    if (data.photos.some((p) => p.uploading || !p.relativePath)) {
      return; // submit gated below; this is defensive
    }
    const payload = {
      ...data,
      photos: data.photos.map((p, index) => ({
        relativePath: p.relativePath as string,
        sortOrder: index,
      })),
    };
    if (editingDish) {
      await updateDish.mutateAsync({ id: editingDish.id, data: payload as unknown as DishFormData });
    } else {
      await createDish.mutateAsync(payload as unknown as DishFormData);
    }
    closeDialog();
  }
```

(Note: the `as unknown as DishFormData` cast is intentional — the wire payload uses `DishPhotoInput`-shaped photos, not `DishPhotoForm`. We bridge here rather than introduce a second form-type for the API call. Task 21 cleans this up by giving `useCreateDish` / `useUpdateDish` an explicit payload type.)

- [ ] **Step 4: Replace the `<ImageUpload>` Controller with `<DishGalleryEditor>`**

Locate the `Controller` block at the bottom of `<DialogContent>` (around lines 320-330) — the one rendering `<ImageUpload area="dish" ... />`. Replace the entire `Controller` with:

```tsx
            <Controller
              name="photos"
              control={form.control}
              render={({ field }) => (
                <DishGalleryEditor
                  value={field.value}
                  onChange={(next) => field.onChange(next)}
                />
              )}
            />
```

- [ ] **Step 5: Disable Save while uploads are pending**

Replace the existing `submitting` line and the `<Button type="submit">` inside `<DialogActions>`:

```tsx
  const photosUploading = form.watch('photos')?.some((p) => p.uploading) ?? false;
  const submitting = createDish.isPending || updateDish.isPending;
```

```tsx
            <Button
              type="submit"
              variant="contained"
              disabled={submitting || photosUploading}
              startIcon={submitting ? <CircularProgress size={18} color="inherit" /> : undefined}
            >
              {editingDish ? 'Сохранить' : 'Создать'}
            </Button>
```

- [ ] **Step 6: Update the table cover thumbnail and add "+N" badge**

Locate the `<TableCell>` for `<Фото>` (around lines 181-209). Replace the entire cell with:

```tsx
                    <TableCell>
                      {dish.photos.length > 0 ? (
                        <Box sx={{ position: 'relative', width: 40, height: 40 }}>
                          <Box
                            component="img"
                            src={imageUrl(dish.photos[0].relativePath)!}
                            alt={dish.name}
                            sx={{
                              width: 40,
                              height: 40,
                              objectFit: 'cover',
                              borderRadius: 0.5,
                            }}
                          />
                          {dish.photos.length > 1 && (
                            <Box sx={{
                              position: 'absolute',
                              top: -4, right: -4,
                              bgcolor: 'rgba(0,0,0,0.7)',
                              color: 'white',
                              fontSize: 10,
                              fontWeight: 600,
                              borderRadius: 8,
                              minWidth: 18, height: 18,
                              px: 0.5,
                              display: 'flex', alignItems: 'center', justifyContent: 'center',
                            }}>
                              +{dish.photos.length - 1}
                            </Box>
                          )}
                        </Box>
                      ) : (
                        <Box
                          sx={{
                            width: 40,
                            height: 40,
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            bgcolor: '#F5ECD7',
                            borderRadius: 0.5,
                          }}
                        >
                          <ImageIcon color="disabled" fontSize="small" />
                        </Box>
                      )}
                    </TableCell>
```

- [ ] **Step 7: Type-check**

Run: `cd front && npx tsc --noEmit`
Expected: Only errors remain in `pages/PublicMenu.tsx`.

---

## Task 20: Loosen `useUpdateDish` / `useCreateDish` payload type to avoid `as unknown` cast

**Files:**
- Modify: `front/src/hooks/use-dishes.ts`
- Modify: `front/src/api/dishes.ts`
- Modify: `front/src/types/dish.ts`
- Modify: `front/src/pages/admin/Dishes.tsx`

- [ ] **Step 1: Add a wire-payload type to `types/dish.ts`**

Append to `front/src/types/dish.ts`:

```ts
export interface DishWritePhoto {
  relativePath: string;
  sortOrder: number;
}

export interface DishWritePayload {
  categoryId: string;
  name: string;
  description: string | null;
  price: number;
  photos: DishWritePhoto[];
  sortOrder: number;
  isVisible: boolean;
}
```

- [ ] **Step 2: Update `front/src/api/dishes.ts`**

Replace the `createDish` / `updateDish` signatures:

```ts
import type { DishResponse, DishWritePayload } from '@/types/dish';
import { apiFetch } from './client';

export function getDishes(categoryId?: string): Promise<DishResponse[]> {
  const url = categoryId
    ? `/api/admin/dishes?categoryId=${encodeURIComponent(categoryId)}`
    : '/api/admin/dishes';
  return apiFetch<DishResponse[]>(url);
}

export function getDish(id: string): Promise<DishResponse> {
  return apiFetch<DishResponse>(`/api/admin/dishes/${id}`);
}

export function createDish(data: DishWritePayload): Promise<DishResponse> {
  return apiFetch<DishResponse>('/api/admin/dishes', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

export function updateDish(id: string, data: DishWritePayload): Promise<DishResponse> {
  return apiFetch<DishResponse>(`/api/admin/dishes/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  });
}

export function setDishVisibility(id: string, isVisible: boolean): Promise<DishResponse> {
  return apiFetch<DishResponse>(`/api/admin/dishes/${id}/visibility`, {
    method: 'PATCH',
    body: JSON.stringify({ isVisible }),
  });
}

export function deleteDish(id: string): Promise<void> {
  return apiFetch<void>(`/api/admin/dishes/${id}`, {
    method: 'DELETE',
  });
}
```

- [ ] **Step 3: Update `front/src/hooks/use-dishes.ts`**

Replace the type import and the two mutation hook bodies:

```ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { enqueueSnackbar } from 'notistack';

import type { DishWritePayload } from '@/types/dish';
import { ApiError } from '@/api/client';
import {
  getDishes,
  createDish,
  updateDish,
  setDishVisibility,
  deleteDish,
} from '@/api/dishes';

function handleError(error: Error) {
  const message =
    error instanceof ApiError ? error.response.message : 'Произошла ошибка';
  enqueueSnackbar(message, { variant: 'error' });
}

export function useDishesQuery(categoryId?: string) {
  return useQuery({
    queryKey: ['dishes', { categoryId }],
    queryFn: () => getDishes(categoryId),
  });
}

export function useCreateDish() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: createDish,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['dishes'] });
      enqueueSnackbar('Блюдо создано', { variant: 'success' });
    },
    onError: handleError,
  });
}

export function useUpdateDish() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: { id: string; data: DishWritePayload }) =>
      updateDish(params.id, params.data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['dishes'] });
      enqueueSnackbar('Блюдо обновлено', { variant: 'success' });
    },
    onError: handleError,
  });
}

export function useSetDishVisibility() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (params: { id: string; isVisible: boolean }) =>
      setDishVisibility(params.id, params.isVisible),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['dishes'] });
    },
    onError: handleError,
  });
}

export function useDeleteDish() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: deleteDish,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['dishes'] });
      enqueueSnackbar('Блюдо удалено', { variant: 'success' });
    },
    onError: handleError,
  });
}
```

- [ ] **Step 4: Drop the `as unknown` cast in Dishes.tsx**

In `front/src/pages/admin/Dishes.tsx`, replace `onSubmit`:

```tsx
  async function onSubmit(data: DishFormData) {
    if (data.photos.some((p) => p.uploading || !p.relativePath)) {
      return;
    }
    const payload: DishWritePayload = {
      categoryId: data.categoryId,
      name: data.name,
      description: data.description,
      price: data.price,
      photos: data.photos.map((p, index) => ({
        relativePath: p.relativePath as string,
        sortOrder: index,
      })),
      sortOrder: data.sortOrder,
      isVisible: data.isVisible,
    };
    if (editingDish) {
      await updateDish.mutateAsync({ id: editingDish.id, data: payload });
    } else {
      await createDish.mutateAsync(payload);
    }
    closeDialog();
  }
```

Add `DishWritePayload` to the existing type import block at the top of the file:

```tsx
import type { DishResponse, DishFormData, DishWritePayload } from '@/types/dish';
```

- [ ] **Step 5: Type-check + lint**

Run:

```bash
cd front && npx tsc --noEmit && npm run lint
```

Expected: Only `PublicMenu.tsx` errors remaining.

- [ ] **Step 6: Commit Tasks 16, 18, 19, 20 together**

```bash
git add front/src/types/dish.ts front/src/types/public.ts \
        front/src/components/shared/DishGalleryEditor.tsx \
        front/src/api/dishes.ts \
        front/src/hooks/use-dishes.ts \
        front/src/pages/admin/Dishes.tsx
git commit -m "feat(admin): drag-n-drop multi-photo gallery editor for dishes"
```

---

## Task 21: Create DishCarousel component

**Files:**
- Create: `front/src/components/shared/DishCarousel.tsx`

- [ ] **Step 1: Create the file**

```tsx
import { useCallback, useEffect, useState } from 'react'
import useEmblaCarousel from 'embla-carousel-react'
import { imageUrl } from '@/utils/image-url'

interface DishCarouselProps {
  photos: string[]
  alt: string
}

export default function DishCarousel({ photos, alt }: DishCarouselProps) {
  if (photos.length === 0) {
    return null
  }

  if (photos.length === 1) {
    return (
      <img
        className="pm-modal-photo"
        src={imageUrl(photos[0])!}
        alt={alt}
      />
    )
  }

  return <Carousel photos={photos} alt={alt} />
}

function Carousel({ photos, alt }: DishCarouselProps) {
  const [emblaRef, emblaApi] = useEmblaCarousel({ loop: false, align: 'start' })
  const [selectedIndex, setSelectedIndex] = useState(0)

  const onSelect = useCallback(() => {
    if (!emblaApi) return
    setSelectedIndex(emblaApi.selectedScrollSnap())
  }, [emblaApi])

  useEffect(() => {
    if (!emblaApi) return
    onSelect()
    emblaApi.on('select', onSelect)
    return () => {
      emblaApi.off('select', onSelect)
    }
  }, [emblaApi, onSelect])

  return (
    <div className="pm-carousel">
      <div className="pm-carousel-viewport" ref={emblaRef}>
        <div className="pm-carousel-track">
          {photos.map((path, i) => (
            <div className="pm-carousel-slide" key={`${path}-${i}`}>
              <img
                className="pm-modal-photo"
                src={imageUrl(path)!}
                alt={alt}
                draggable={false}
              />
            </div>
          ))}
        </div>
      </div>

      <div className="pm-carousel-counter">
        {selectedIndex + 1} / {photos.length}
      </div>

      <div className="pm-carousel-dots">
        {photos.map((_, i) => (
          <button
            key={i}
            className={`pm-carousel-dot${i === selectedIndex ? ' is-active' : ''}`}
            onClick={() => emblaApi?.scrollTo(i)}
            aria-label={`Перейти к фото ${i + 1}`}
            type="button"
          />
        ))}
      </div>
    </div>
  )
}
```

---

## Task 22: Add carousel CSS to public-menu.css

**Files:**
- Modify: `front/src/public-menu.css`

- [ ] **Step 1: Append the new classes**

Add at the end of `front/src/public-menu.css`:

```css
/* ── Dish carousel (modal) ── */

.pm-carousel {
  position: relative;
  width: 100%;
}

.pm-carousel-viewport {
  overflow: hidden;
  width: 100%;
  touch-action: pan-y;
}

.pm-carousel-track {
  display: flex;
}

.pm-carousel-slide {
  flex: 0 0 100%;
  min-width: 0;
}

.pm-carousel-counter {
  position: absolute;
  top: 12px;
  right: 12px;
  background: rgba(0, 0, 0, 0.6);
  color: #fff;
  font-size: 12px;
  padding: 3px 9px;
  border-radius: 12px;
  pointer-events: none;
}

.pm-carousel-dots {
  position: absolute;
  bottom: 10px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  gap: 6px;
  padding: 4px 8px;
  background: rgba(0, 0, 0, 0.35);
  border-radius: 12px;
}

.pm-carousel-dot {
  width: 7px;
  height: 7px;
  border-radius: 50%;
  border: 0;
  padding: 0;
  background: rgba(255, 255, 255, 0.55);
  cursor: pointer;
  transition: background 120ms ease;
}

.pm-carousel-dot.is-active {
  background: #fff;
}

/* ── Card multi-photo badge ── */

.pm-dish-card-photo-badge {
  position: absolute;
  top: 8px;
  right: 8px;
  background: rgba(0, 0, 0, 0.65);
  color: #fff;
  font-size: 11px;
  font-weight: 600;
  padding: 2px 8px;
  border-radius: 10px;
  pointer-events: none;
}
```

---

## Task 23: Wire carousel + cover badge into PublicMenu.tsx

**Files:**
- Modify: `front/src/pages/PublicMenu.tsx`

- [ ] **Step 1: Add the import**

Near the other imports, add:

```tsx
import DishCarousel from '@/components/shared/DishCarousel';
```

- [ ] **Step 2: Replace `DishCard` (around lines 362-382) to use `photos[0]` and "+N" badge**

```tsx
function DishCard({ dish }: { dish: PublicMenuDish }) {
  const cover = dish.photos[0];
  const extras = dish.photos.length - 1;

  return (
    <div className="pm-dish-card">
      <div className="pm-dish-card-photo-wrap">
        <img
          className="pm-dish-card-photo"
          src={imageUrl(cover)!}
          alt={dish.name}
          loading="lazy"
        />
        {extras > 0 && (
          <div className="pm-dish-card-photo-badge">+{extras}</div>
        )}
      </div>
      <div className="pm-dish-card-body">
        <div className="pm-dish-card-name">{dish.name}</div>
        {dish.description && (
          <div className="pm-dish-card-desc">{dish.description}</div>
        )}
        <div className="pm-dish-card-price">{formatPrice(dish.price)} сум</div>
      </div>
    </div>
  );
}
```

- [ ] **Step 3: Replace the grid render condition for cards-vs-list**

Locate the block (around lines 346-354):

```tsx
        {category.dishes.map((dish) => (
          <motion.div key={dish.id} variants={dishCard} onClick={() => onDishClick(dish)} style={{ cursor: 'pointer' }}>
            {dish.photoPath ? (
              <DishCard dish={dish} />
            ) : (
              <DishListItem dish={dish} />
            )}
          </motion.div>
        ))}
```

Replace with:

```tsx
        {category.dishes.map((dish) => (
          <motion.div key={dish.id} variants={dishCard} onClick={() => onDishClick(dish)} style={{ cursor: 'pointer' }}>
            {dish.photos.length > 0 ? (
              <DishCard dish={dish} />
            ) : (
              <DishListItem dish={dish} />
            )}
          </motion.div>
        ))}
```

- [ ] **Step 4: Replace the modal photo block to use `DishCarousel`**

Locate the block in `DishModal` (around lines 479-492):

```tsx
        {dish.photoPath ? (
          <div className="pm-modal-photo-wrap">
            <motion.img
              className="pm-modal-photo"
              src={imageUrl(dish.photoPath)!}
              alt={dish.name}
              initial={{ scale: 1 }}
              animate={{ scale: 1.06 }}
              transition={{ duration: 8, ease: 'linear' }}
            />
          </div>
        ) : (
          <div className="pm-modal-placeholder" />
        )}
```

Replace with:

```tsx
        {dish.photos.length > 0 ? (
          <div className="pm-modal-photo-wrap">
            <DishCarousel photos={dish.photos} alt={dish.name} />
          </div>
        ) : (
          <div className="pm-modal-placeholder" />
        )}
```

(We drop the slow zoom animation here — it doesn't compose cleanly with the carousel transitions. The Ken-Burns zoom only made sense for a static image. If we miss it later we can re-add per-slide via embla's `slide` events.)

- [ ] **Step 5: Type-check + lint + build**

Run:

```bash
cd front && npx tsc --noEmit && npm run lint && npm run build
```

Expected: All clean. `npm run build` produces `dist/` with no errors.

- [ ] **Step 6: Commit**

```bash
git add front/src/components/shared/DishCarousel.tsx \
        front/src/public-menu.css \
        front/src/pages/PublicMenu.tsx
git commit -m "feat(public): swipeable photo carousel + multi-photo card badge"
```

---

## Task 24: End-to-end smoke test

**Files:** none (verification only)

- [ ] **Step 1: Start backend and frontend in parallel**

Terminal 1:

```bash
dotnet run --project src/WebApi
```

Terminal 2:

```bash
cd front && npm run dev
```

Wait until both are healthy: backend on `:5288`, Vite on `:3000`.

- [ ] **Step 2: Acceptance criteria #1 — fresh DB starts clean**

Open `http://localhost:3000` in a browser. Expected: Public menu loads with the seeded categories. No console errors. (The dev DB now has zero photos, so cards fall back to list-item rendering.)

- [ ] **Step 3: Acceptance criteria #2 — multi-photo create + reorder + save**

1. Log in at `http://localhost:3000/admin/login` with seeded admin credentials
2. Navigate to "Блюда"
3. Click "Добавить блюдо", fill required fields, then click the "+ Загрузить" tile, select 5 JPG/PNG files in the OS dialog at once
4. Wait for spinners to finish on each tile (each file uploads via `/api/admin/uploads/image` in parallel)
5. Drag the third tile to the first position. Expect "★ Обложка" badge to migrate to the dragged tile
6. Click "Создать"
7. Reload the page; open the same dish in Edit mode. Expect: 5 tiles in the new order, first one carries the "★ Обложка" badge

- [ ] **Step 4: Acceptance criteria #3 — edit deletes a photo and reorders**

1. With the same dish open for edit, click the × on the second tile. Expect: tile removed; remaining 4 retain order
2. Drag the last tile to the first position
3. Click "Сохранить"
4. Reload public menu. Expect: dish card cover is the newly-promoted photo; modal carousel has 4 photos, dragged-to-first is at index 0

- [ ] **Step 5: Acceptance criteria #4 — public-menu modal slider and badge**

1. On the public menu, find the dish from above. Expect: cover thumbnail with "+3" badge in top-right
2. Tap the card → modal opens with the carousel
3. Swipe horizontally (or use mouse drag). Expect: slides advance, dots-indicator updates, counter "X / 4" updates
4. Click a dot → expect that slide centers
5. Press Escape → expect modal closes

- [ ] **Step 6: Acceptance criteria #5 — single photo bypasses carousel**

Create another dish with exactly one photo. Open it on the public menu modal. Expect: plain `<img>` (no dots, no counter, no horizontal swipe — verify by inspecting the DOM for absence of `.pm-carousel`).

- [ ] **Step 7: Acceptance criteria #6 — zero-photo dish shows placeholder**

Create a dish with no photos at all. Open the public menu. Expect: the dish renders as the dotted-line list item (no card image), and opening its modal shows the existing `.pm-modal-placeholder`.

- [ ] **Step 8: Acceptance criteria #7 — clean builds**

In two separate terminals (backend + frontend stopped):

```bash
dotnet build
cd front && npm run build
```

Expected: both succeed with 0 errors. No leftover references to `PhotoPath` or `photoPath` (search the repo to be sure):

```bash
grep -r "PhotoPath" src/ --include="*.cs" || echo "OK — no PhotoPath in C#"
grep -rn "photoPath" front/src --include="*.ts" --include="*.tsx" | grep -v "about" | grep -v "PublicAbout" | grep -v "ContactsContent" || echo "OK — no dish photoPath in TS/TSX"
```

(The `about` content area still uses `photoPath`; that's expected and out of scope.)

- [ ] **Step 9: No commit — verification only**

If any acceptance criterion fails, file an issue or fix in a follow-up task and re-run the relevant smoke step.
