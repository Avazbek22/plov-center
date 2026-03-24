# Implementation Plan: Admin CRUD Panel

**Branch**: `002-admin-crud-panel` | **Date**: 2026-03-25 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/002-admin-crud-panel/spec.md`

## Summary

Frontend admin panel with full CRUD for Categories, Dishes, and Site Content. Uses existing backend API endpoints (no backend changes). Adds sidebar navigation, data tables, form dialogs, image upload, and toast notifications to the existing React + MUI admin shell from feature 001.

## Technical Context

**Language/Version**: TypeScript 5.9, React 19, Node.js 20+
**Primary Dependencies**: Vite 8, @mui/material 7, react-router-dom 7, @tanstack/react-query 5, react-hook-form 7, zod 4, notistack (NEW)
**Storage**: Backend handles all persistence (PostgreSQL). Frontend uses localStorage for JWT only.
**Testing**: Not requested
**Target Platform**: Modern browsers (Chrome, Firefox, Safari, Edge)
**Project Type**: Web — frontend only (backend exists)
**Performance Goals**: All CRUD operations < 3s perceived, SPA navigation instant
**Constraints**: No pagination needed (cafe scale: ~20 categories, ~100 dishes)
**Scale/Scope**: 1 admin user, 4 admin pages (dashboard, categories, dishes, content), ~15 new files

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. Context-First | PASS | Backend API fully analyzed, existing frontend code read |
| II. Single Source of Truth | PASS | Types defined once in `front/src/types/`, API functions in `front/src/api/` |
| III. Library-First | PASS | notistack for notifications, MUI Table (built-in), react-hook-form+zod (existing) |
| IV. Code Reuse & DRY | PASS | Reuse existing auth, theme, API client, layout patterns from feature 001 |
| V. Strict Type Safety | PASS | TypeScript strict mode, all API responses typed |
| VI. Atomic Task Execution | PASS | Each task = 1 file or logical unit |
| VII. Quality Gates | PASS | tsc --noEmit + npm run build after each phase |
| VIII. Progressive Specification | PASS | spec → plan → tasks → implement |
| IX. Error Handling | PASS | ApiError class reused, server errors displayed via notistack |
| X. Observability | N/A | Frontend-only, backend handles logging |
| XI. Accessibility | RECOMMENDED | MUI components have built-in a11y, keyboard nav via MUI |

**Post-design re-check**: All gates still pass. No violations.

## Project Structure

### Documentation (this feature)

```text
specs/002-admin-crud-panel/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── api.md
├── checklists/
│   └── requirements.md
└── tasks.md
```

### Source Code (frontend only — `front/src/`)

```text
front/src/
├── api/
│   ├── client.ts              # Existing — add uploadImage helper
│   ├── auth.ts                # Existing — no changes
│   ├── categories.ts          # NEW — category API functions
│   ├── dishes.ts              # NEW — dish API functions
│   ├── content.ts             # NEW — site content API functions
│   └── uploads.ts             # NEW — image upload function
├── auth/
│   ├── auth-context.tsx       # Existing — no changes
│   └── ProtectedRoute.tsx     # Existing — no changes
├── components/
│   ├── layout/
│   │   ├── AdminLayout.tsx    # MODIFY — add Drawer sidebar
│   │   └── AdminSidebar.tsx   # NEW — sidebar navigation component
│   └── shared/
│       ├── ConfirmDialog.tsx   # NEW — reusable delete confirmation
│       └── ImageUpload.tsx     # NEW — reusable image upload with preview
├── hooks/
│   ├── use-categories.ts      # NEW — useQuery/useMutation for categories
│   ├── use-dishes.ts          # NEW — useQuery/useMutation for dishes
│   └── use-content.ts         # NEW — useQuery/useMutation for content
├── pages/
│   ├── Landing.tsx             # Existing — no changes
│   ├── NotFound.tsx            # Existing — no changes
│   └── admin/
│       ├── Login.tsx           # Existing — no changes
│       ├── Dashboard.tsx       # Existing — no changes
│       ├── Categories.tsx      # NEW — categories list + CRUD
│       ├── Dishes.tsx          # NEW — dishes list + CRUD
│       └── Content.tsx         # NEW — site content forms
├── types/
│   ├── auth.ts                # Existing — no changes
│   ├── api.ts                 # Existing — no changes
│   ├── category.ts            # NEW — CategoryResponse, CategoryFormData
│   ├── dish.ts                # NEW — DishResponse, DishFormData
│   └── content.ts             # NEW — AdminSiteContentResponse, form types
├── theme/
│   └── theme.ts               # Existing — no changes
├── App.tsx                    # MODIFY — add routes for categories, dishes, content
└── main.tsx                   # MODIFY — add SnackbarProvider (notistack)
```

**Structure Decision**: Frontend-only feature. Extends existing `front/` structure from feature 001. New files organized by domain (categories, dishes, content) across api/, hooks/, pages/, types/ layers. Shared UI components in components/shared/.

## Key Architecture Decisions

### 1. React Query Hooks per Entity

Each entity gets a custom hooks file (`use-categories.ts`, `use-dishes.ts`, `use-content.ts`) that encapsulates:
- `useQuery` for fetching list/detail
- `useMutation` for create/update/delete
- Auto-invalidation on mutation success
- Error handling integration with notistack

### 2. Dialog-Based CRUD (not page-based)

Create/Edit forms open in MUI Dialog overlays, not separate pages. Rationale:
- Keeps user in context of the list
- Simpler routing (no `/admin/categories/:id/edit` routes)
- Appropriate for small forms (3-7 fields)

### 3. Reusable Shared Components

- `ConfirmDialog` — used by categories and dishes for delete confirmation
- `ImageUpload` — used by dishes and content "About" section

### 4. File Upload Flow

```
User selects file → ImageUpload shows preview →
On form submit → POST /api/admin/uploads/image → get photoPath →
Include photoPath in create/update request
```

Upload happens inline during form interaction, before the main entity save.
