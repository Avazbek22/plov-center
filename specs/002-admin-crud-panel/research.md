# Research: Admin CRUD Panel

**Date**: 2026-03-25
**Feature**: 002-admin-crud-panel

## Library Decisions

### 1. Notifications — notistack

**Decision**: Use `notistack` for toast notifications
**Rationale**: High reputation (score 92.2), 36 code snippets in docs, integrates seamlessly with MUI Snackbar, provides `enqueueSnackbar` globally without prop drilling, supports success/error/warning/info variants out of the box
**Alternatives considered**:
- MUI Snackbar manually — requires custom state management per component, no stacking
- react-hot-toast — doesn't integrate with MUI theme
**Library**: notistack, latest version, MUI-compatible

### 2. Server State — TanStack React Query (already installed)

**Decision**: Use `useMutation` + `invalidateQueries` pattern for all CRUD operations
**Rationale**: Already installed (v5.95.2), `useMutation` handles loading/error/success states, `onSuccess → invalidateQueries` keeps lists in sync after create/update/delete
**Pattern**: Each entity gets query keys: `['categories']`, `['categories', id]`, `['dishes']`, `['dishes', id]`, `['content']`

### 3. Tables — MUI Table (built-in)

**Decision**: Use standard MUI `Table` component, not `@mui/x-data-grid`
**Rationale**: Cafe admin will have ~10-20 categories and ~50-100 dishes max. DataGrid adds 200KB+ bundle for features we don't need (virtual scrolling, column resizing, server-side pagination). Simple MUI Table is sufficient.
**Alternatives considered**:
- @mui/x-data-grid — overkill for this scale, large bundle impact
- TanStack Table — unnecessary abstraction when MUI Table handles it

### 4. Forms — react-hook-form + zod (already installed)

**Decision**: Continue using existing react-hook-form (v7.72) + @hookform/resolvers + zod (v4.3.6)
**Rationale**: Already installed and proven in Login form from feature 001. Same pattern extends to all CRUD forms.

### 5. Confirmation Dialogs — MUI Dialog (built-in)

**Decision**: Use MUI `Dialog` for delete confirmations
**Rationale**: Built into @mui/material, no extra dependency. Simple confirm/cancel pattern.

### 6. File Upload — native FormData + fetch

**Decision**: Use native `FormData` with existing `apiFetch` (modified for multipart) or direct `fetch` for file uploads
**Rationale**: Backend expects `multipart/form-data` on `POST /api/admin/uploads/image`. No library needed — it's one endpoint with one file input. MUI Button + hidden input for UI.
**Pattern**: File input onChange → FormData → fetch → get URL back → set in form field

### 7. Sidebar Navigation — MUI Drawer (built-in)

**Decision**: Use MUI `Drawer` (permanent variant) for sidebar navigation
**Rationale**: Built into @mui/material, standard admin panel pattern. Permanent drawer on desktop, no need for responsive collapse in MVP.
