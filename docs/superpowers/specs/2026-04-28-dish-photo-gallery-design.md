# Dish Photo Gallery — Design Spec

**Date:** 2026-04-28
**Status:** Approved (brainstorming complete, awaiting implementation plan)

## Goal

Replace the single-photo-per-dish model with an ordered multi-photo gallery. Public menu shows a swipeable carousel inside the dish modal; the admin form has a drag-n-drop multi-upload editor.

## Non-goals

- Image resizing / WebP / AVIF on the backend (keep current JPG/PNG, 5 MB limit)
- Per-photo metadata (alt text, captions) — schema is structured to allow it later, no UI now
- Cross-category photo browsing in the public carousel
- Migrating existing `PhotoPath` data — no production yet, dev data is disposable
- Test coverage — project has no test projects today; introducing them is a separate effort

## Constraints inherited from the project

- Clean Architecture layering (`Domain` → `Application.Contract` → `Application` → `Infrastructure`/`WebApi`)
- No repository abstraction — handlers query `IApplicationDbContext` directly
- File-scoped namespaces, primary constructors, `sealed` on all non-abstract classes
- Frontend uses MUI for admin, plain CSS + motion for public menu
- Uploads go through the existing `POST /api/admin/uploads/image` endpoint; no changes to it
- JWT, validation pipeline, exception middleware all stay as they are

## 1. Domain & schema

New entity: `Domain/Entities/DishPhoto.cs`

```csharp
public sealed class DishPhoto : AuditableEntity
{
    public Guid DishId { get; set; }
    public Dish? Dish { get; set; }
    public string RelativePath { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
```

Updated entity: `Domain/Entities/Dish.cs`

- Remove `string? PhotoPath`
- Add `public List<DishPhoto> Photos { get; set; } = [];`

Postgres table `dish_photos`:

| column           | type          | notes                                             |
|------------------|---------------|---------------------------------------------------|
| id               | uuid PK       | from `AuditableEntity.Id`                         |
| dish_id          | uuid FK       | `REFERENCES dishes(id) ON DELETE CASCADE`         |
| relative_path    | varchar(512)  | NOT NULL                                          |
| sort_order       | int           | NOT NULL                                          |
| created_utc      | timestamp     | from `AuditableEntity`                            |
| updated_utc      | timestamp     | from `AuditableEntity`                            |

Index: non-unique `(dish_id, sort_order)` — supports ordered fetch and "replace all" save strategy without uniqueness constraint conflicts during reordering.

EF Core configuration: `Infrastructure/Persistence/Configurations/DishPhotoConfiguration.cs` (mirrors style of `DishConfiguration`).

`ApplicationDbContext` adds `public DbSet<DishPhoto> DishPhotos => Set<DishPhoto>();` and the new configuration is registered.

Migration: one new EF Core migration `AddDishPhotos`. Steps:

1. `DROP COLUMN dishes.photo_path`
2. `CREATE TABLE dish_photos` with FK + index

Existing dev data with photos loses its image (acceptable — no prod, user explicitly approved data loss).

## 2. Application.Contract changes

`Application.Contract/Dishes/DishPhotoInput.cs` (new):

```csharp
public sealed record DishPhotoInput(string RelativePath, int SortOrder);
```

`Application.Contract/Dishes/Responses/DishPhotoResponse.cs` (new):

```csharp
public sealed record DishPhotoResponse(Guid Id, string RelativePath, int SortOrder);
```

`CreateDishCommand` / `UpdateDishCommand`:
- Remove `string? PhotoPath`
- Add `IReadOnlyList<DishPhotoInput> Photos { get; set; } = [];`

`DishResponse`:
- Replace `string? PhotoPath` with `IReadOnlyList<DishPhotoResponse> Photos`

`Menu/Responses/PublicMenuDishResponse`:
- Replace `string? PhotoPath` with `IReadOnlyList<string> Photos` (relative paths only, no IDs needed publicly)

## 3. Application layer

### Validators

`CreateDishCommandValidator` / `UpdateDishCommandValidator` add rules for `Photos`:
- Each `RelativePath` non-empty, ≤ 512 chars
- Each `SortOrder` ≥ 0
- No duplicate `SortOrder` values within a single payload
- Optional sanity cap (e.g. `Photos.Count ≤ 50`) to bound payload size; user said "no limit" on UX but a defensive backend cap is fine and not user-visible at normal scale

### Save strategy (replace-all)

In `CreateDishCommandHandler`:
- Insert new `Dish` and its `DishPhoto`s in one `SaveChangesAsync`

In `UpdateDishCommandHandler`:
1. Load `Dish` with `Include(d => d.Photos)`; throw `NotFoundException` if missing
2. Apply scalar updates (`Name`, `Description`, etc.)
3. **Remove all existing `DishPhoto`s for this dish from the context, then add new ones from the payload** — single transaction via `SaveChangesAsync`
4. Return mapped `DishResponse`

This is intentional: drag-n-drop reorders, additions, deletions, and edits all go through one path. Photo `Id`s are not stable across edits, which is fine since the public surface only sees paths.

### Mappings

`DishResponseMappings.cs` extension methods updated to project `Photos.OrderBy(p => p.SortOrder)` into the response. Public menu mapping projects to `IReadOnlyList<string>`.

### Queries

`GetDishByIdQueryHandler` and `GetAdminDishesQueryHandler` add `.Include(d => d.Photos)` (sorted in mapping).
`Menu` query handler same.

## 4. WebApi layer

No new endpoints. Existing controllers compile against the updated commands/responses. Request DTOs in `WebApi/Contracts/Admin/Dishes/` updated to mirror command shape.

## 5. Frontend — admin (`front/src/`)

### New types (`types/dish.ts`)

```ts
export interface DishPhotoForm {
  tempId: string;          // client-side uuid, used for dnd-kit keys + tracking pending uploads
  relativePath: string | null;  // null while uploading
  sortOrder: number;
  uploading: boolean;
}

export interface DishFormData {
  // existing fields …
  photos: DishPhotoForm[];   // replaces photoPath
}
```

Zod schema: `photos: z.array(...)` — no min, since the user wants no limit and a dish without photo is allowed.

### New component: `components/shared/DishGalleryEditor.tsx`

Replaces `ImageUpload` for the dish form (the existing `ImageUpload` stays for the `about` content area).

Layout: horizontal flex-wrap of 96×96 tiles + a final "+ Загрузить" tile.

Each photo tile shows:
- Image preview (object-fit: cover)
- "★ Обложка" badge — only on the first tile (sortOrder min)
- Delete × button top-right
- Drag handle ⋮⋮ at bottom center
- `CircularProgress` overlay while `uploading === true`

Behavior:
- Click "+ Загрузить" → opens file picker with `multiple accept="image/jpeg,image/png"`
- For each selected file: append a new `DishPhotoForm` with `tempId`, `uploading: true`, kick off `uploadImage(file, 'dish')` in parallel; on success set `relativePath` + `uploading: false`; on failure remove the tile and toast via notistack
- Drag-n-drop reordering via `@dnd-kit/core` + `@dnd-kit/sortable`. Sort order is recomputed on drop based on array index (`photos.map((p, i) => ({ ...p, sortOrder: i }))`)
- Delete × removes the tile from local state. Orphaned files on disk are an accepted consequence (already true for the single-photo flow today)

The component is a controlled form field bound via `react-hook-form` `Controller`, just like `ImageUpload` is today.

Submit gating: `Dishes.tsx` disables the form's "Save" button while any tile has `uploading === true`.

### Admin list view (`pages/admin/Dishes.tsx`)

The "Фото" column shows `photos[0].relativePath` as the cover thumbnail (or the placeholder square when the array is empty), with a small "+N" pill overlay when `photos.length > 1`.

### New dependencies

- `@dnd-kit/core`
- `@dnd-kit/sortable`
- `@dnd-kit/utilities`

## 6. Frontend — public (`front/src/pages/PublicMenu.tsx` + helpers)

### Grid card

Render `dish.photos[0]` as the cover. When `dish.photos.length > 1`, overlay a small badge `+N` (where N = `photos.length - 1`) in the top-right corner of the card image.

When `dish.photos.length === 0`, keep the existing placeholder.

### Modal — new component `DishCarousel`

Lives next to the existing modal renderer. Uses **`embla-carousel-react`**.

- Single photo: render plain `<img>` (no carousel)
- Multiple: render Embla viewport with horizontal slides, dots indicator below, counter `(current / total)` overlaid in top-right
- Touch-action: Embla's default `pan-y` lets vertical scrolling pass through, so the modal's `drag="y"` close-gesture continues working
- If conflict surfaces in implementation, fallback is to gate the modal's vertical drag to a top "handle" zone only

New dependency: `embla-carousel-react`.

### CSS

Carousel styles live in `public-menu.css`. Existing modal photo styles (`.pm-modal-photo-wrap`, `.pm-modal-photo`) stay; new classes: `.pm-carousel`, `.pm-carousel-slide`, `.pm-carousel-dots`, `.pm-carousel-counter`, `.pm-card-photo-badge`.

## 7. Cleanup checklist

- `Dish.PhotoPath` removed
- All call sites referencing `dish.photoPath` (frontend) removed or rewritten
- All call sites referencing `Dish.PhotoPath` (backend) removed
- `dishes.photo_path` column dropped via migration
- Old `ImageUpload` usage in `Dishes.tsx` replaced with `DishGalleryEditor`
- `ImageUpload` itself is kept (still used by Content/about)
- DTO type definitions updated end-to-end (no `photoPath` left in `front/src/types`)

## 8. Open implementation considerations (decided here, executed in plan)

- **Photo IDs are not stable across `UpdateDish` calls.** Public surface only sees paths, so this is invisible. Admin form treats photos by `tempId` + `relativePath`, never by server-side `Id` after refresh. No client-side persistence of server `Id`s required.
- **Orphan files in `wwwroot/uploads/dish/`** when the user uploads then cancels the dialog: same risk as today's single-photo upload, accepted. Cleanup is a separate, non-blocking concern.
- **Concurrent edits** of the same dish are not protected (same as today). Out of scope.

## 9. Acceptance criteria

1. Backend builds; migration `AddDishPhotos` applies cleanly on a fresh dev DB; seed data continues to work
2. Admin → create a new dish, upload 5 photos at once, reorder via drag, save → reload page → photos appear in the saved order, first one is cover
3. Admin → edit existing dish, delete one photo, change order, save → public menu reflects the new state
4. Public menu → dish with multiple photos shows "+N" badge on grid card; tapping opens modal with swipeable carousel and dots indicator
5. Public menu → dish with single photo shows no badge and no carousel (plain image in modal, as today)
6. Public menu → dish with zero photos shows existing placeholder
7. `dotnet build` and `npm run build` both succeed without warnings about removed `PhotoPath`

## 10. Out of scope (explicit future work)

- Image optimization (resize / WebP)
- Test coverage (project-wide effort; tracked separately)
- Per-photo metadata (alt text)
- Bulk dish import / export
