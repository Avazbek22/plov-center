# Tasks: Admin CRUD Panel

**Input**: Design documents from `/specs/002-admin-crud-panel/`
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/api.md, quickstart.md

**Tests**: Not requested in spec. No test tasks included.

**Organization**: Tasks grouped by user story. US1 (Navigation) comes first as it enables access to all other sections. US2 (Categories) before US3 (Dishes) because dishes depend on categories. US4 (Content) is P2.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2, US3, US4)
- All paths relative to repository root, frontend lives in `front/`

---

## Phase 0: Planning (Executor Assignment)

**Purpose**: Prepare for implementation by analyzing requirements, creating necessary agents, and assigning executors.

- [ ] P001 Analyze all tasks and identify required agent types and capabilities
- [ ] P002 Create missing agents using meta-agent-v3 (launch N calls in single message, 1 per agent), then ask user restart
- [ ] P003 Assign executors to all tasks: MAIN (trivial only), existing agents (100% match), or specific agent names
- [ ] P004 Resolve research tasks: simple (solve with tools now), complex (create prompts in research/)

**Rules**:
- **MAIN executor**: ONLY for trivial tasks (1-2 line fixes, simple imports, single npm install)
- **Existing agents**: ONLY if 100% capability match after thorough examination
- **Agent creation**: Launch all meta-agent-v3 calls in single message for parallel execution
- **After P002**: Must restart claude-code before proceeding to P003

**Artifacts**:
- Updated tasks.md with [EXECUTOR: name], [SEQUENTIAL]/[PARALLEL-GROUP-X] annotations
- .claude/agents/{domain}/{type}/{name}.md (if new agents created)
- research/*.md (if complex research identified)

---

## Phase 1: Setup (Dependencies & Directory Structure)

**Purpose**: Install new dependencies and create directory structure for new files.

- [x] T001 Install notistack in `front/` via `npm install notistack`
- [x] T002 Create directory structure: `front/src/hooks/`, `front/src/components/shared/`

**Checkpoint**: `cd front && npx tsc --noEmit` passes. Directories exist.

---

## Phase 2: Foundational (Types, API Layer, Shared Components, App Wiring)

**Purpose**: Types, API functions, React Query hooks, shared components, and app-level wiring. ALL user stories depend on these.

**CRITICAL**: No user story work can begin until this phase is complete.

### Types (parallel — different files)

- [x] T003 [P] Define category types in `front/src/types/category.ts`: `CategoryResponse` interface (id, name, sortOrder, isVisible, dishCount, createdUtc, updatedUtc), `CategoryFormData` interface (name, sortOrder, isVisible), `ReorderCategoryItem` interface (categoryId, sortOrder), Zod schemas for form validation per data-model.md
- [x] T004 [P] Define dish types in `front/src/types/dish.ts`: `DishResponse` interface (id, categoryId, categoryName, name, description, price, photoPath, sortOrder, isVisible, createdUtc, updatedUtc), `DishFormData` interface (categoryId, name, description, price, photoPath, sortOrder, isVisible), Zod schema for form validation per data-model.md
- [x] T005 [P] Define content and upload types in `front/src/types/content.ts`: `AdminSiteContentResponse` interface with nested `AboutContent` (text, photoPath) and `ContactsContent` (address, phone, hours, mapEmbed), `AboutFormData`, `ContactsFormData`, `UploadImageResponse` interface (relativePath, url, fileName, size)

### API Functions (parallel — depend on types)

- [x] T006 [P] Implement image upload API in `front/src/api/uploads.ts`: `uploadImage(file: File, area: 'dish' | 'about')` function using `FormData` + `fetch` with auth header (NOT `apiFetch` — multipart needs different Content-Type handling), returns `UploadImageResponse`
- [x] T007 [P] Implement categories API in `front/src/api/categories.ts`: `getCategories()`, `getCategory(id)`, `createCategory(data)`, `updateCategory(id, data)`, `setCategoryVisibility(id, isVisible)`, `reorderCategories(items)`, `deleteCategory(id)` — all using `apiFetch` from client.ts per contracts/api.md
- [x] T008 [P] Implement dishes API in `front/src/api/dishes.ts`: `getDishes(categoryId?)`, `getDish(id)`, `createDish(data)`, `updateDish(id, data)`, `setDishVisibility(id, isVisible)`, `deleteDish(id)` — all using `apiFetch` per contracts/api.md
- [x] T009 [P] Implement content API in `front/src/api/content.ts`: `getSiteContent()`, `updateAboutContent(data)`, `updateContactsContent(data)` — all using `apiFetch` per contracts/api.md

### React Query Hooks (parallel — depend on API functions)

- [x] T010 [P] Implement `useCategories` hook in `front/src/hooks/use-categories.ts`: `useCategoriesQuery()` (useQuery with key `['categories']`), `useCreateCategory()`, `useUpdateCategory()`, `useSetCategoryVisibility()`, `useReorderCategories()`, `useDeleteCategory()` — all useMutation with `onSuccess → invalidateQueries(['categories'])`, error notifications via notistack `enqueueSnackbar`
- [x] T011 [P] Implement `useDishes` hook in `front/src/hooks/use-dishes.ts`: `useDishesQuery(categoryId?)` (useQuery with key `['dishes', { categoryId }]`), `useCreateDish()`, `useUpdateDish()`, `useSetDishVisibility()`, `useDeleteDish()` — all useMutation with invalidation and notistack notifications
- [x] T012 [P] Implement `useContent` hook in `front/src/hooks/use-content.ts`: `useContentQuery()` (useQuery with key `['content']`), `useUpdateAbout()`, `useUpdateContacts()` — useMutation with invalidation and notistack notifications

### Shared Components (parallel — no dependencies on hooks)

- [x] T013 [P] Implement `ConfirmDialog` in `front/src/components/shared/ConfirmDialog.tsx`: MUI Dialog with title, message, "Отмена"/"Удалить" buttons. Props: `open`, `title`, `message`, `onConfirm`, `onCancel`, `loading` (disable buttons during mutation). Reused by categories and dishes for delete confirmation
- [x] T014 [P] Implement `ImageUpload` in `front/src/components/shared/ImageUpload.tsx`: MUI Button triggering hidden file input, shows image preview (via URL.createObjectURL or existing photoPath URL), upload progress indicator, calls `uploadImage()` from uploads API on file selection, returns `relativePath` via `onChange` callback. Props: `value` (current photoPath), `onChange` (new relativePath), `area` ('dish' | 'about')

### App Wiring (sequential — depends on types and shared components)

- [x] T015 Add `SnackbarProvider` from notistack to `front/src/main.tsx`: wrap app with `<SnackbarProvider maxSnack={3} autoHideDuration={3000} anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}>` inside existing provider tree
- [x] T016 Add admin routes in `front/src/App.tsx`: add child routes under `/admin` for `categories` → lazy Categories page, `dishes` → lazy Dishes page, `content` → lazy Content page. Import pages from `front/src/pages/admin/`

**Checkpoint**: `cd front && npx tsc --noEmit && npm run build` passes. All types, API functions, hooks, and shared components compile correctly.

---

## Phase 3: User Story 1 — Навигация админ-панели (Priority: P1)

**Goal**: Администратор видит боковое меню с пунктами: Дашборд, Категории, Блюда, Контент. Текущий раздел подсвечен.

**Independent Test**: Login → sidebar visible with all items → click each item → correct page opens, item highlighted.

### Implementation

- [x] T017 [US1] Implement `AdminSidebar` in `front/src/components/layout/AdminSidebar.tsx`: MUI `List` with `ListItemButton` + `ListItemIcon` + `ListItemText` for: Дашборд (DashboardIcon, `/admin`), Категории (CategoryIcon, `/admin/categories`), Блюда (RestaurantMenuIcon, `/admin/dishes`), Контент (ArticleIcon, `/admin/content`). Use `useLocation()` from react-router to highlight active item via `selected` prop
- [x] T018 [US1] Modify `AdminLayout` in `front/src/components/layout/AdminLayout.tsx`: replace simple Box layout with MUI permanent `Drawer` (width 240px) containing `AdminSidebar`, keep existing AppBar with username and logout button, render `Outlet` in main content area to the right of the drawer

**Checkpoint**: US1 complete. Login → sidebar visible → click "Категории" → navigates to `/admin/categories` with item highlighted. All nav items work.

---

## Phase 4: User Story 2 — Управление категориями (Priority: P1)

**Goal**: Администратор видит таблицу категорий, может создавать, редактировать, переключать видимость, удалять.

**Independent Test**: Navigate to `/admin/categories` → create "Супы" → edit to "Первые блюда" → toggle visibility → delete.

**Depends on**: Phase 2 (types, API, hooks, shared components), Phase 3 (navigation to reach the page)

### Implementation

- [x] T019 [US2] Implement Categories page in `front/src/pages/admin/Categories.tsx`: MUI Container with "Категории" heading and "Добавить категорию" button. MUI Table with columns: Название, Порядок, Видимость (Switch), Блюд (count), Действия (edit/delete IconButtons). Create/Edit form in MUI Dialog with react-hook-form + Zod (name required, sortOrder required integer >= 0, isVisible checkbox). Use `useCategoriesQuery()` for data, `useCreateCategory()`/`useUpdateCategory()` for form submit, `useSetCategoryVisibility()` for switch toggle, `useDeleteCategory()` with `ConfirmDialog`. Show loading skeleton while data fetches. Display server errors via ApiError handling in mutations

**Checkpoint**: US2 complete. Full CRUD for categories works. Create, edit, toggle visibility, delete (with confirmation). Server errors displayed.

---

## Phase 5: User Story 3 — Управление блюдами (Priority: P1)

**Goal**: Администратор видит таблицу блюд с фильтром по категории, может CRUD с загрузкой фото.

**Independent Test**: Navigate to `/admin/dishes` → filter by category → create "Плов" with photo → edit price → toggle visibility → delete.

**Depends on**: Phase 2, Phase 3, Phase 4 (categories must exist to assign dishes)

### Implementation

- [x] T020 [US3] Implement Dishes page in `front/src/pages/admin/Dishes.tsx`: MUI Container with "Блюда" heading, category filter (MUI Select using `useCategoriesQuery()` for options + "Все категории" default), and "Добавить блюдо" button. MUI Table with columns: Фото (small thumbnail or placeholder), Название, Категория, Цена (formatted), Видимость (Switch), Действия (edit/delete). Create/Edit form in MUI Dialog with: categoryId (Select from categories), name (TextField required), description (TextField multiline optional), price (TextField number required > 0), sortOrder (TextField number), isVisible (Checkbox), photoPath via `ImageUpload` component. Use `useDishesQuery(selectedCategoryId)` for filtered data, all dish mutations from `useDishes` hook, `ConfirmDialog` for delete

**Checkpoint**: US3 complete. Full CRUD for dishes works. Category filter works. Photo upload with preview works. Server errors displayed.

---

## Phase 6: User Story 4 — Управление контентом сайта (Priority: P2)

**Goal**: Администратор редактирует "О нас" (текст + фото) и "Контакты" (адрес, телефон, часы, карта).

**Independent Test**: Navigate to `/admin/content` → edit "О нас" text + upload photo → edit contacts → both save successfully.

**Depends on**: Phase 2, Phase 3

### Implementation

- [x] T021 [US4] Implement Content page in `front/src/pages/admin/Content.tsx`: MUI Container with two MUI Card sections. **"О нас" card**: react-hook-form with text (TextField multiline), photoPath (ImageUpload with area='about'), "Сохранить" button calling `useUpdateAbout()`. **"Контакты" card**: react-hook-form with address, phone, hours (TextFields), mapEmbed (TextField multiline for embed code), "Сохранить" button calling `useUpdateContacts()`. Load current values via `useContentQuery()` and populate forms with `reset()` on data load. Show success notifications via notistack on save

**Checkpoint**: US4 complete. Both forms load current values, save successfully, show notifications.

---

## Phase 7: Polish & Quality Gates

**Purpose**: Final quality validation.

- [x] T022 Run quality gate: `cd front && npx tsc --noEmit && npm run build` — both must pass with zero errors
- [ ] T023 Run quickstart.md validation: start backend (`dotnet run --project src/WebApi`), start frontend (`cd front && npm run dev`), verify all CRUD flows per quickstart.md scenarios

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 (notistack must be installed)
- **US1 Navigation (Phase 3)**: Depends on Phase 2 (needs routes in App.tsx)
- **US2 Categories (Phase 4)**: Depends on Phase 2 + Phase 3 (needs hooks + sidebar to navigate)
- **US3 Dishes (Phase 5)**: Depends on Phase 2 + Phase 3 + Phase 4 (needs category data for filter/select)
- **US4 Content (Phase 6)**: Depends on Phase 2 + Phase 3 (independent of US2/US3)
- **Polish (Phase 7)**: Depends on all stories complete

### Within-Phase Dependencies

**Phase 2**:
```
T003, T004, T005 (types) ──────────┐
                                    ├→ T006, T007, T008, T009 (API functions)
                                    │       │
                                    │       ├→ T010, T011, T012 (React Query hooks)
                                    │
T013, T014 (shared components) ─────┤
                                    ├→ T015 (SnackbarProvider) → T016 (routes)
```

**Phase 3**: T017 (AdminSidebar) → T018 (AdminLayout modification)

### Parallel Opportunities

- T003 + T004 + T005: all type files, independent
- T006 + T007 + T008 + T009: all API files, independent
- T010 + T011 + T012: all hook files, independent
- T013 + T014: both shared components, independent
- T021 (Content page) can run in parallel with T019 (Categories) or T020 (Dishes) if Phase 2+3 are complete

---

## Implementation Strategy

### MVP First (US1 + US2)

1. Complete Phase 1: Setup (T001-T002)
2. Complete Phase 2: Foundation (T003-T016)
3. Complete Phase 3: US1 Navigation (T017-T018)
4. Complete Phase 4: US2 Categories (T019)
5. **STOP and VALIDATE**: Categories CRUD works end-to-end

### Incremental Delivery

1. Setup + Foundation → App compiles with new types/hooks
2. Add US1 Navigation → Sidebar visible, all routes accessible
3. Add US2 Categories → First CRUD operational (MVP!)
4. Add US3 Dishes → Second CRUD with photo upload
5. Add US4 Content → Site content editable
6. Polish → Quality gates pass

### Single Developer Strategy

Execute phases sequentially: 1 → 2 → 3 → 4 → 5 → 6 → 7. Total: 23 tasks.

---

## Summary

| Metric | Value |
|--------|-------|
| Total tasks | 23 (T001-T023) |
| Phase 1 Setup | 2 tasks |
| Phase 2 Foundational | 14 tasks |
| Phase 3 US1 Navigation | 2 tasks |
| Phase 4 US2 Categories | 1 task |
| Phase 5 US3 Dishes | 1 task |
| Phase 6 US4 Content | 1 task |
| Phase 7 Polish | 2 tasks |
| Parallel opportunities | 5 groups (types, API, hooks, shared, pages) |
| MVP scope | Phase 1-4 (19 tasks) |

## Notes

- No test tasks — not requested in spec
- All file paths prefixed with `front/` since backend lives in `src/`
- Backend must be running for CRUD operations to actually work
- [P] tasks within same phase = different files, safe to parallelize
- Dialog-based CRUD (not page-based) per plan.md architecture decision
- notistack is the only new dependency
