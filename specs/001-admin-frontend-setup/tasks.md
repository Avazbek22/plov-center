# Tasks: Admin Frontend Setup — Landing Page & Login

**Input**: Design documents from `/specs/001-admin-frontend-setup/`
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/api.md

**Tests**: Not requested in spec. No test tasks included.

**Organization**: Tasks grouped by user story. US1 and US2 are both P1 but US2 depends on foundational auth infrastructure, so US1 (Landing) comes first as quickest standalone deliverable.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2, US3)
- All paths relative to repository root, frontend lives in `front/`

---

## Phase 0: Planning (Executor Assignment)

**Purpose**: Prepare for implementation by analyzing requirements, creating necessary agents, and assigning executors.

- [x] P001 Analyze all tasks and identify required agent types and capabilities
- [x] P002 Create missing agents using meta-agent-v3 (launch N calls in single message, 1 per agent), then ask user restart
- [x] P003 Assign executors to all tasks: MAIN (trivial only), existing agents (100% match), or specific agent names
- [x] P004 Resolve research tasks: simple (solve with tools now), complex (create prompts in research/)

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

## Phase 1: Setup (Project Initialization)

**Purpose**: Create Vite + React + TypeScript project in `front/`, install all dependencies, configure build tooling.

- [x] T001 Scaffold Vite React-TS project in `front/` using `npm create vite@latest front -- --template react-ts`, then remove boilerplate files (App.css, assets/react.svg, default App.tsx content)
- [x] T002 Install runtime dependencies: `@mui/material @mui/icons-material @emotion/react @emotion/styled react-router-dom @tanstack/react-query react-hook-form @hookform/resolvers zod` in `front/`
- [x] T003 Configure Vite dev server proxy and port in `front/vite.config.ts`: port 3000, proxy `/api/*` to `http://localhost:5000`, set `changeOrigin: true`
- [x] T004 Configure TypeScript strict mode in `front/tsconfig.app.json`: ensure `strict: true`, add path alias `@/*` → `./src/*`
- [x] T005 Create directory structure under `front/src/`: `api/`, `auth/`, `pages/admin/`, `components/layout/`, `theme/`, `types/`

**Checkpoint**: `cd front && npm run dev` starts without errors, shows default React page. `npm run build` succeeds. `tsc --noEmit` passes.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared types, API client, auth context, theme, and routing shell. ALL user stories depend on these.

**CRITICAL**: No user story work can begin until this phase is complete.

- [x] T006 [P] Define TypeScript types in `front/src/types/auth.ts`: `AdminUser`, `AuthSession`, `LoginCredentials` interfaces per data-model.md
- [x] T007 [P] Define TypeScript types in `front/src/types/api.ts`: `LoginResponse`, `ApiErrorResponse` interfaces per contracts/api.md
- [x] T008 Implement API client in `front/src/api/client.ts`: typed `apiFetch<T>()` wrapper around fetch — reads JWT from localStorage (`plov-center-auth`), sets `Authorization: Bearer` header, parses JSON response, throws typed `ApiErrorResponse` on non-2xx
- [x] T009 Implement auth API functions in `front/src/api/auth.ts`: `login(credentials: LoginCredentials): Promise<LoginResponse>` calling `POST /api/admin/auth/login`, `getMe(): Promise<AdminUser>` calling `GET /api/admin/auth/me` — both using `apiFetch` from client.ts
- [x] T010 Implement auth context in `front/src/auth/auth-context.tsx`: `AuthProvider` with `useReducer` (actions: LOGIN, LOGOUT, RESTORE), `useAuth()` hook returning `{ session, login, logout, isLoading }`. Persist token + admin to localStorage on LOGIN, clear on LOGOUT. On mount, restore from localStorage and validate via `getMe()` — if 401, clear session
- [x] T011 [P] Create MUI theme in `front/src/theme/theme.ts`: `createTheme()` with PlovCenter palette (warm colors suitable for a plov restaurant — e.g. primary amber/orange, secondary brown), Cyrillic-friendly typography (Roboto)
- [x] T012 Create route definitions in `front/src/App.tsx`: `createBrowserRouter` with routes — `/` → Landing, `/admin/login` → Login, `/admin` → ProtectedRoute wrapping AdminLayout > Dashboard, `*` → NotFound. Export `RouterProvider`
- [x] T013 Create app entry in `front/src/main.tsx`: render `<ThemeProvider>` + `<CssBaseline>` + `<QueryClientProvider>` + `<AuthProvider>` + `<RouterProvider>`. Configure QueryClient with sensible defaults (staleTime 5min, refetchOnWindowFocus false)

**Checkpoint**: App renders at localhost:3000, navigating to `/` shows empty Landing component, `/admin/login` shows empty Login, `/admin` redirects to `/admin/login` when not authenticated.

---

## Phase 3: User Story 1 — Landing Page (Priority: P1) MVP

**Goal**: Посетитель открывает `/` и видит простую главную страницу кафе "Плов Центр" с навигацией.

**Independent Test**: Open `http://localhost:3000/` — page shows cafe name, description, and link to admin panel.

### Implementation

- [x] T014 [US1] Implement Landing page in `front/src/pages/Landing.tsx`: MUI Container + Typography with "Плов Центр" heading, brief cafe description paragraph, navigation link "Админ-панель" pointing to `/admin/login` using react-router `Link`. Use MUI `Box`, `Button`, and `Stack` for layout. Keep it minimal — no images or complex layout

**Checkpoint**: US1 complete. Open `/` — see "Плов Центр", description, and clickable "Админ-панель" link navigating to `/admin/login`.

---

## Phase 4: User Story 2 — Admin Login (Priority: P1)

**Goal**: Администратор вводит логин/пароль на `/admin/login`, получает JWT, попадает в dashboard.

**Independent Test**: Navigate to `/admin/login`, enter `admin` / `Admin123!`, click "Войти" — redirect to `/admin` showing dashboard placeholder.

**Depends on**: Phase 2 (auth context, API client, types)

### Implementation

- [x] T015 [US2] Implement Login page in `front/src/pages/admin/Login.tsx`: MUI Card centered on page, react-hook-form with Zod schema (`username` required, `password` required), MUI TextField for each field, Button "Войти" submitting form. On submit: call `login()` from `useAuth()`, on success navigate to `/admin`, on API error display error message (MUI Alert), on validation errors show per-field errors. Disable button during loading
- [x] T016 [US2] Implement AdminLayout in `front/src/components/layout/AdminLayout.tsx`: MUI `Box` with `AppBar` (title "Плов Центр — Админ"), `Outlet` for child routes. Include username from `useAuth()` session in AppBar and "Выйти" button calling `logout()`. Minimal sidebar not required yet — just top bar + content area
- [x] T017 [US2] Implement Dashboard placeholder in `front/src/pages/admin/Dashboard.tsx`: MUI Container + Typography "Добро пожаловать, {username}!" using admin data from `useAuth()`. Simple card with text "Панель управления" as placeholder content

**Checkpoint**: US2 complete. Full login flow works: enter creds → API call → JWT stored → redirect to `/admin` → see dashboard with username. Wrong creds show error. Empty form shows validation.

---

## Phase 5: User Story 3 — Route Protection & Logout (Priority: P2)

**Goal**: Неавторизованные пользователи не могут попасть в `/admin/*`. Администратор может выйти.

**Independent Test**: Open `/admin` without token → redirect to `/admin/login`. Login → see dashboard → click "Выйти" → redirect to login.

**Depends on**: Phase 2 (auth context), Phase 4 (Login + Dashboard exist to verify flow)

### Implementation

- [x] T018 [US3] Implement ProtectedRoute in `front/src/auth/ProtectedRoute.tsx`: component checks `useAuth()` session — if `isLoading` show MUI CircularProgress centered, if not authenticated redirect to `/admin/login` via `Navigate`, if authenticated render `Outlet`. Wire into App.tsx routes wrapping `/admin` children
- [x] T019 [US3] Handle token expiration in `front/src/api/client.ts`: if any API response returns 401, clear auth from localStorage and redirect to `/admin/login` (via window.location or dispatching LOGOUT). This ensures expired tokens don't leave user stuck

**Checkpoint**: US3 complete. All `/admin/*` routes protected. Expired/missing token → redirect to login. Logout clears session and redirects.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: 404 page, final quality validation.

- [x] T020 [P] Implement NotFound page in `front/src/pages/NotFound.tsx`: MUI Container + Typography "404 — Страница не найдена", `Link` back to `/`. Ensure catch-all `*` route in App.tsx renders this
- [x] T021 Run quality gate: `cd front && npx tsc --noEmit && npm run build` — both must pass with zero errors
- [x] T022 Run quickstart.md validation: start backend (`dotnet run --project src/WebApi`), start frontend (`cd front && npm run dev`), verify all routes manually per quickstart.md table

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 completion — BLOCKS all user stories
- **US1 Landing (Phase 3)**: Depends on Phase 2 (needs routing shell). No dependency on other stories
- **US2 Login (Phase 4)**: Depends on Phase 2 (needs auth context, API client, types). No dependency on US1
- **US3 Route Protection (Phase 5)**: Depends on Phase 2. Functionally benefits from US2 existing for verification
- **Polish (Phase 6)**: Depends on all stories complete

### Within-Phase Dependencies

**Phase 2**:
```
T006, T007 (types) ──┐
                      ├→ T008 (api client) → T009 (auth api) → T010 (auth context)
T011 (theme) ─────────┤
                      ├→ T012 (routes) → T013 (main.tsx)
```

**Phase 4**: T015 (Login) and T016 (AdminLayout) and T017 (Dashboard) are sequential — Layout wraps Dashboard, Login navigates to Dashboard.

### Parallel Opportunities

- T006 + T007 + T011: all independent files, can run in parallel
- T014 (Landing) can run in parallel with T015-T017 (Login flow) if Phase 2 is complete
- T020 (NotFound) can run in parallel with any Phase 5 tasks

---

## Parallel Example: Phase 2 Foundation

```
# Group A — types (parallel):
T006: Define auth types in front/src/types/auth.ts
T007: Define API types in front/src/types/api.ts
T011: Create MUI theme in front/src/theme/theme.ts

# Group B — sequential (depends on types):
T008: API client → T009: Auth API → T010: Auth context

# Group C — sequential (can run after types + theme):
T012: Route definitions → T013: Main entry
```

---

## Implementation Strategy

### MVP First (US1 Only)

1. Complete Phase 1: Setup (T001-T005)
2. Complete Phase 2: Foundation (T006-T013)
3. Complete Phase 3: US1 Landing (T014)
4. **STOP and VALIDATE**: Landing page renders at `/`
5. Deployable as static site

### Incremental Delivery

1. Setup + Foundation → App skeleton running
2. Add US1 Landing → Public page visible (MVP!)
3. Add US2 Login → Admin can authenticate
4. Add US3 Route Protection → Admin panel secured
5. Polish → 404 page, quality gates pass

### Single Developer Strategy

Execute phases sequentially: 1 → 2 → 3 → 4 → 5 → 6. Total: 22 tasks.

---

## Summary

| Metric | Value |
|--------|-------|
| Total tasks | 22 (T001-T022) |
| Phase 1 Setup | 5 tasks |
| Phase 2 Foundational | 8 tasks |
| Phase 3 US1 Landing | 1 task |
| Phase 4 US2 Login | 3 tasks |
| Phase 5 US3 Protection | 2 tasks |
| Phase 6 Polish | 3 tasks |
| Parallel opportunities | 3 groups (types, pages, NotFound) |
| MVP scope | Phase 1-3 (14 tasks) |

## Notes

- No test tasks — not requested in spec
- All file paths prefixed with `front/` since backend lives in `src/`
- Commit after each task or logical group
- [P] tasks within same phase = different files, safe to parallelize
- Backend must be running for US2 login to actually work
