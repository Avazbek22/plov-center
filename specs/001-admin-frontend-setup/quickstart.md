# Quickstart: PlovCenter Frontend

## Prerequisites

- Node.js 20+
- npm
- PlovCenter backend running on `http://localhost:5000` (or `5241` HTTPS)

## Setup

```bash
cd front
npm install
npm run dev
```

Dev-сервер стартует на `http://localhost:3000`. API-запросы `/api/*` проксируются на бэкенд.

## Backend

```bash
# В корне репозитория
dotnet run --project src/WebApi
```

Бэкенд применяет миграции и сидит админа (`admin / Admin123!`) при старте.

## Структура фронтенда

```
front/
├── index.html
├── package.json
├── tsconfig.json
├── vite.config.ts
└── src/
    ├── main.tsx                  # Entry: BrowserRouter, QueryClient, ThemeProvider
    ├── App.tsx                   # Routes definition
    ├── api/
    │   ├── client.ts             # fetch wrapper с auth header
    │   └── auth.ts               # login(), getMe()
    ├── auth/
    │   ├── auth-context.tsx       # AuthProvider, useAuth()
    │   └── ProtectedRoute.tsx     # Route guard → redirect /admin/login
    ├── pages/
    │   ├── Landing.tsx            # Публичная главная (/)
    │   ├── NotFound.tsx           # 404
    │   └── admin/
    │       ├── Login.tsx          # /admin/login
    │       └── Dashboard.tsx      # /admin (заглушка)
    ├── components/
    │   └── layout/
    │       └── AdminLayout.tsx    # Sidebar + header shell для админки
    └── theme/
        └── theme.ts              # MUI createTheme для PlovCenter
```

## Routes

| Path           | Component     | Auth Required |
|----------------|---------------|---------------|
| `/`            | Landing       | No            |
| `/admin/login` | Login         | No            |
| `/admin`       | Dashboard     | Yes           |
| `*`            | NotFound      | No            |
