# Research: Admin Frontend Setup

## Library Decisions

### Build Tool: Vite 6

- **Decision**: Vite 6 с шаблоном `react-ts`
- **Rationale**: Используется в Farovon-LMS, быстрый HMR, нативный proxy для бэкенда
- **Alternatives**: CRA (deprecated), Next.js (overkill для SPA-админки)

### UI Framework: MUI v7

- **Decision**: `@mui/material` v7
- **Rationale**: Уже используется в Farovon-LMS, полный набор компонентов для админки (TextField, Button, Card, AppBar, Drawer). Тема кастомизируется через `createTheme`
- **Alternatives**: shadcn/ui (нет готовых layout-компонентов), Ant Design (тяжелее, другой стиль)

### Routing: React Router v7

- **Decision**: `react-router-dom` v7 с `createBrowserRouter`
- **Rationale**: Farovon-LMS использует v7. Поддерживает middleware для auth guard, lazy loading через `React.lazy`
- **Alternatives**: TanStack Router (менее зрелый), Wouter (нет middleware/guards)

### State Management: React Context + useReducer

- **Decision**: Auth state через React Context, как в Farovon-LMS (`auth-state.tsx` паттерн)
- **Rationale**: Для одного слоя состояния (auth) не нужна библиотека. Context + useReducer достаточен
- **Alternatives**: Zustand (лишняя зависимость для одного store), Redux (overkill)

### Data Fetching: TanStack React Query v5

- **Decision**: `@tanstack/react-query` v5
- **Rationale**: Кэширование, автоматический refetch, mutation API. Используется в Farovon-LMS
- **Alternatives**: SWR (меньше возможностей для mutations), чистый fetch (нет кэширования)

### Form Validation: React Hook Form + Zod

- **Decision**: `react-hook-form` + `@hookform/resolvers` + `zod`
- **Rationale**: Используется в Farovon-LMS. Валидация на клиенте через Zod-схемы, минимальные ре-рендеры
- **Alternatives**: Formik (больше ре-рендеров), ручная валидация (больше кода)

## Backend API

Бэкенд PlovCenter уже имеет нужные эндпоинты:

- `POST /api/admin/auth/login` — `{ username, password }` → `{ token, expiresAtUtc, admin: { id, username, isActive } }`
- `GET /api/admin/auth/me` — Bearer token → `{ id, username, isActive }`
- Ошибки: `{ code, message, traceId, errors? }` (стандартный `ApiErrorResponse`)

## Token Storage

- **Decision**: `localStorage` (ключ `plov-center-auth`)
- **Rationale**: Простота, достаточно для admin-only панели. Farovon-LMS использует тот же подход
- **Alternatives**: httpOnly cookies (требует изменений на бэкенде), sessionStorage (теряется при закрытии вкладки)

## Proxy Configuration

Dev-сервер Vite проксирует `/api/*` на `http://localhost:5000` (порт ASP.NET по умолчанию для HTTP). Конфиг через `server.proxy` в `vite.config.ts`.
