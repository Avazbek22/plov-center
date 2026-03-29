# Implementation Plan: Admin Frontend Setup — Landing Page & Login

**Branch**: `001-admin-frontend-setup` | **Date**: 2026-03-24 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/001-admin-frontend-setup/spec.md`

## Summary

Создать фронтенд-проект в `front/` с React 19 + Vite + TypeScript + MUI v7. Реализовать публичную главную страницу, страницу логина для админки и защиту маршрутов через JWT-авторизацию с существующим бэкендом PlovCenter.

## Technical Context

**Language/Version**: TypeScript 5.8, React 19, Node.js 20+
**Primary Dependencies**: Vite 6, @mui/material 7, react-router-dom 7, @tanstack/react-query 5, react-hook-form 7, zod 4
**Storage**: localStorage для JWT-токена
**Testing**: Нет на данном этапе (первоначальная настройка)
**Target Platform**: SPA, браузер (desktop + mobile)
**Project Type**: Web application (frontend-only, бэкенд уже существует в `src/`)
**Performance Goals**: Время загрузки главной < 2с, логин < 15с полный путь
**Constraints**: Dev proxy на localhost:5000, CORS настроен в бэкенде
**Scale/Scope**: 4 страницы (Landing, Login, Dashboard, 404), ~15 файлов

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. Context-First | PASS | Изучен бэкенд API, Farovon-LMS структура |
| II. Single Source of Truth | PASS | Типы в `src/types/`, тема в `src/theme/` |
| III. Library-First | PASS | MUI, React Router, RHF, Zod — всё библиотечное |
| IV. Code Reuse | PASS | Паттерны из Farovon-LMS адаптированы |
| V. Strict Type Safety | PASS | TypeScript strict, Zod для валидации |
| VI. Atomic Tasks | PASS | Будет в tasks.md |
| VII. Quality Gates | PASS | tsc --noEmit + vite build перед коммитом |
| VIII. Progressive Spec | PASS | spec → plan → tasks → implement |
| IX. Error Handling | PASS | ApiErrorResponse типизирован, ошибки сети обработаны |
| X. Observability | N/A | Фронтенд SPA, нет серверного логирования |
| XI. Accessibility | PASS | MUI компоненты имеют встроенную a11y |

**Post-Phase 1 re-check**: PASS — никаких нарушений.

## Project Structure

### Documentation (this feature)

```text
specs/001-admin-frontend-setup/
├── plan.md              # This file
├── spec.md              # Feature specification
├── research.md          # Library decisions
├── data-model.md        # Frontend TypeScript types
├── quickstart.md        # Setup instructions
├── contracts/
│   └── api.md           # Backend API contracts
└── checklists/
    └── requirements.md  # Spec quality checklist
```

### Source Code (repository root)

```text
front/
├── index.html
├── package.json
├── tsconfig.json
├── tsconfig.app.json
├── tsconfig.node.json
├── vite.config.ts
└── src/
    ├── main.tsx                    # Entry: BrowserRouter, QueryClient, ThemeProvider
    ├── App.tsx                     # Route definitions
    ├── vite-env.d.ts
    ├── api/
    │   ├── client.ts               # fetch wrapper: base URL, auth header, error parsing
    │   └── auth.ts                 # login(), getMe() — typed API calls
    ├── auth/
    │   ├── auth-context.tsx         # AuthProvider + useAuth() hook (Context + useReducer)
    │   └── ProtectedRoute.tsx       # Checks auth → children or redirect to /admin/login
    ├── pages/
    │   ├── Landing.tsx              # Public home page (/)
    │   ├── NotFound.tsx             # 404 page
    │   └── admin/
    │       ├── Login.tsx            # /admin/login — form with RHF + Zod
    │       └── Dashboard.tsx        # /admin — placeholder
    ├── components/
    │   └── layout/
    │       └── AdminLayout.tsx      # Sidebar + top bar wrapper for admin routes
    ├── theme/
    │   └── theme.ts                 # MUI createTheme for PlovCenter
    └── types/
        ├── auth.ts                  # AuthSession, AdminUser, LoginCredentials
        └── api.ts                   # ApiErrorResponse, LoginResponse
```

**Structure Decision**: Backend уже живёт в `src/` (C#). Frontend в `front/` — полное разделение. Нет monorepo tooling, просто отдельная npm-директория.

## Complexity Tracking

Нет нарушений конституции — таблица не требуется.
