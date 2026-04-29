# Плов Центр — фронтенд

React 19 SPA: публичное QR-меню (`/`) + админка (`/admin`).

## Стек
TypeScript 5.9, React 19, Vite 8, MUI 7, react-router-dom 7, TanStack Query 5, react-hook-form + zod, notistack, motion (Framer Motion).

## Команды

```bash
npm install      # один раз
npm run dev      # dev-сервер на :3000, проксирует /api на localhost:5288
npm run build    # production-сборка в dist/
npm run lint     # ESLint
```

Бэкенд должен быть запущен (`dotnet run --project src/WebApi` из корня репо) — иначе `/api` отдаст 502.

## Структура

```
src/
  api/          apiFetch + функции на ресурс (auth, categories, dishes, content, uploads, public)
  auth/         AuthProvider + ProtectedRoute
  components/   layout/ (AdminLayout, Sidebar), shared/ (ConfirmDialog, ImageUpload, DishGalleryEditor)
  hooks/        TanStack Query хуки (use-categories, use-dishes, use-content, use-public-menu)
  pages/        PublicMenu (/) + admin/* (Login, Dashboard, Categories, Dishes, Content)
  theme/        MUI-тема
  types/        TS-типы под бэкенд DTO
  utils/        imageUrl и прочие
```

`apiFetch` подшивает JWT из localStorage (`plov-center-auth`), на 401 чистит токен и редиректит на `/admin/login`.

Публичные страницы (`/`) ходят через `api/public.ts` без авторизации, рендерятся на чистом CSS (`public-menu.css`) + `motion`, без MUI.

## Конвенции

- Алиас `@` -> `src/`
- Каждый ресурс: `api/<resource>.ts` -> `hooks/use-<resource>.ts` -> страница
- Мутации инвалидируют кэш и показывают тосты через `handleMutationError`
