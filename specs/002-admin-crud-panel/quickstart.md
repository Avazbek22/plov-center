# Quickstart: Admin CRUD Panel

## Prerequisites

1. Backend running on `http://localhost:5288` (`dotnet run --project src/WebApi`)
2. Frontend running on `http://localhost:3000` (`cd front && npm run dev`)
3. Admin credentials: `admin` / `Admin123!`

## Verification Scenarios

### 1. Navigation

| Step | Action | Expected |
|------|--------|----------|
| 1 | Login at `/admin/login` | Redirect to `/admin` |
| 2 | Check sidebar | Sidebar visible with: Дашборд, Категории, Блюда, Контент |
| 3 | Click "Категории" | Navigate to `/admin/categories`, item highlighted |
| 4 | Click "Блюда" | Navigate to `/admin/dishes`, item highlighted |
| 5 | Click "Контент" | Navigate to `/admin/content`, item highlighted |
| 6 | Click "Дашборд" | Navigate to `/admin`, item highlighted |

### 2. Categories CRUD

| Step | Action | Expected |
|------|--------|----------|
| 1 | Go to `/admin/categories` | Table shows existing categories (or empty) |
| 2 | Click "Добавить категорию" | Form dialog opens |
| 3 | Fill "Супы", sortOrder 1, visible | — |
| 4 | Click "Сохранить" | Dialog closes, "Супы" appears in table, success notification |
| 5 | Click edit on "Супы" | Form dialog opens with current values |
| 6 | Change name to "Первые блюда", save | Name updates in table, success notification |
| 7 | Click visibility toggle | isVisible toggles, notification |
| 8 | Click delete on empty category | Confirmation dialog |
| 9 | Confirm delete | Category removed, notification |

### 3. Dishes CRUD

| Step | Action | Expected |
|------|--------|----------|
| 1 | Go to `/admin/dishes` | Table shows all dishes |
| 2 | Select category filter | Table filters to selected category |
| 3 | Click "Добавить блюдо" | Form dialog opens |
| 4 | Fill name "Плов", category, price 45000 | — |
| 5 | Upload photo | Photo preview appears |
| 6 | Click "Сохранить" | Dialog closes, dish in table, notification |
| 7 | Click edit, change price to 50000 | Price updates in table |
| 8 | Toggle visibility | Visibility changes |
| 9 | Delete dish | Removed from table |

### 4. Site Content

| Step | Action | Expected |
|------|--------|----------|
| 1 | Go to `/admin/content` | Two forms: "О нас" and "Контакты" with current values |
| 2 | Edit "О нас" text, upload photo | — |
| 3 | Click "Сохранить" | Success notification, values persist on reload |
| 4 | Edit contacts: address, phone, hours | — |
| 5 | Click "Сохранить" | Success notification, values persist on reload |

### 5. Error Handling

| Step | Action | Expected |
|------|--------|----------|
| 1 | Create category with empty name | Client validation error shown |
| 2 | Create dish with price 0 | Client validation error shown |
| 3 | Delete category with dishes | Server error "category has dishes" displayed |
| 4 | Stop backend, try to save | Network error displayed |

## Quality Gates

```bash
cd front && npx tsc --noEmit    # TypeScript check — 0 errors
cd front && npm run build        # Production build — succeeds
```
