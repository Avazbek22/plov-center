# Plan: Public Menu Page (QR-меню для посетителей)

## Context

PlovCenter — сайт-меню для кафе, доступ через QR-код в ресторане. Бэкенд полностью готов (`GET /api/public/menu`, `GET /api/public/content`), админка работает. Не хватает главного — публичная страница меню, которую увидит посетитель со смартфона.

Текущий лендинг (`Landing.tsx`) — заглушка с названием и кнопкой "Админ-панель". Нужно заменить на полноценное мобильное меню.

## Approach

Заменяем `Landing.tsx` на полноценную публичную страницу меню. Одна страница, вертикальный скролл, три секции: hero, меню с категориями, контакты.

## Data Available from Backend

```
GET /api/public/menu → { categories: [{ id, name, sortOrder, dishes: [{ id, name, description, price, photoPath, sortOrder }] }] }
GET /api/public/content → { about: { text, photoPath }, contacts: { address, phone, hours, mapEmbed } }
```

## New Files

### 1. `front/src/types/public.ts` — типы для публичных эндпоинтов
```
PublicMenuCategory { id, name, sortOrder, dishes: PublicMenuDish[] }
PublicMenuDish { id, name, description?, price, photoPath?, sortOrder }
PublicMenu { categories: PublicMenuCategory[] }
PublicAbout { text?, photoPath? }
PublicContacts { address?, phone?, hours?, mapEmbed? }
PublicSiteContent { about: PublicAbout, contacts: PublicContacts }
```

### 2. `front/src/api/public.ts` — API функции для публичных данных
- `getPublicMenu()` → `apiFetch<PublicMenu>('/api/public/menu')`
- `getPublicContent()` → `apiFetch<PublicSiteContent>('/api/public/content')`

### 3. `front/src/hooks/use-public-menu.ts` — React Query хуки
- `usePublicMenu()` — query для меню
- `usePublicContent()` — query для контента

### 4. `front/src/pages/PublicMenu.tsx` — главная страница меню

**Структура:**
```
Hero section
  — "Плов Центр" + описание из about.text + фото about.photoPath

Sticky category tabs (горизонтальный скролл)
  — MUI Tabs, variant="scrollable", при клике scroll-to-section

Menu sections (по категориям)
  — Название категории как заголовок секции
  — Карточки блюд: фото (если есть) + название + описание + цена
  — Блюда без фото: компактный вид (строка с ценой)

Footer — контакты
  — Адрес, телефон (кликабельный tel:), часы работы
  — mapEmbed если заполнен (iframe карта)
```

**UI-решения:**
- Mobile-first, `maxWidth="sm"` (600px) — оптимально для телефона
- Sticky Tabs с `position: sticky, top: 0, zIndex` — не уезжают при скролле
- Scroll-to-section через `useRef` + `scrollIntoView({ behavior: 'smooth' })`
- Карточки блюд: `Card` с `CardMedia` для фото, `CardContent` для текста/цены
- Цена — жирным, выравнена вправо
- Цвета из существующей темы: оранжевый primary, кремовый фон
- `CircularProgress` на загрузку, сообщение об ошибке если API недоступен

## Modified Files

### 5. `front/src/App.tsx` — обновить роутинг
- Заменить `/` с `<Landing />` на `<PublicMenu />`
- Убрать импорт Landing (файл удалим)

### 6. Удалить `front/src/pages/Landing.tsx` — больше не нужен

## Verification

1. `cd front && npm run build` — сборка без ошибок
2. `cd front && npm run lint` — линтер чистый
3. Запустить бэкенд + фронт, открыть `localhost:3000` — видно меню
4. Проверить на мобильной ширине (Chrome DevTools, 375px) — всё читаемо
5. Проверить sticky tabs — при скролле категории прилипают
6. Проверить клик по категории — скроллит к секции
7. Проверить телефон — кликабельная ссылка `tel:`
