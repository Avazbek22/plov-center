# UI/UX Overhaul — Public Menu + Admin Panel

## Context

Критика текущего состояния выявила ряд проблем: монотонный колоринг с размазанным accent, шаблонная типографика (Playfair Display), пустые серые плашки у блюд без фото, низкая плотность на мобильных, отсутствие skeleton loading, нереспонсивная админка, хардкод цветов. Цель — довести до уровня продакшен QR-меню.

---

## 1. Типографика

**Замена:** Playfair Display → **Young Serif** (display font). DM Sans остаётся для body.

**Файлы:**
- `front/index.html` — обновить Google Fonts link
- `front/src/pages/public-menu.css` — обновить `@import` + `--font-display`
- `front/src/components/layout/AdminLayout.tsx` — font-family в брендинге
- `front/src/pages/admin/Login.tsx` — font-family заголовка
- `front/src/pages/NotFound.tsx` — font-family 404

---

## 2. Цветовая дисциплина

**Проблема:** MUI theme (#C08B3F) и public-menu.css (#C4841D) — разные accent-цвета. Accent используется для всего подряд.

**Решение:**
- Единый accent: `#C4841D` везде (более насыщенный, лучше для еды)
- Обновить `front/src/theme/theme.ts` — primary → #C4841D, primaryDark → #A86E14, выровнять bg/text цвета с public-menu.css
- В public-menu.css ограничить accent: **только** цены, активный таб, CTA-ссылки. Разделители и декор → приглушённый тон (`--text-muted` или opacity)
- Секционные `<hr>` — сделать `--text-muted` вместо `--accent`

---

## 3. No-photo карточки

**Текущее:** `.pm-dish-item::before` рисует серый прямоугольник 140px.

**Решение:** Заменить серый блок на карточку с декоративным SVG-элементом (стилизованная пиала/узор) по центру в muted-цвете. Карточка остаётся той же высоты. Текст снизу — как с фото.

**Файлы:**
- `front/src/pages/public-menu.css` — переделать `.pm-dish-item::before` → `.pm-dish-item-placeholder` внутри JSX
- `front/src/pages/PublicMenu.tsx` — добавить inline SVG placeholder в `DishListItem`

---

## 4. Фон контентной области

**Проблема:** `background: url('/hero-bg.png') center / cover no-repeat fixed` + оверлей 82% — мутное просвечивание.

**Решение:** Убрать background-image из `.pm-content`. Вместо этого — чистый `var(--bg)` с опциональной subtle CSS-текстурой (noise/grain через pseudo-element, чисто CSS). Удалить `::before` overlay.

**Файлы:**
- `front/src/pages/public-menu.css` — `.pm-content` и `.pm-content::before`

---

## 5. Навигация — scroll hint

**Решение:** Добавить fade-out gradient на правом краю `.pm-nav` когда есть скролл.

**Реализация:** CSS `::after` pseudo-element с `linear-gradient(to right, transparent, var(--bg-elevated))` + JS/CSS проверка `scrollLeft < scrollWidth - clientWidth`.

**Файлы:**
- `front/src/pages/public-menu.css` — `.pm-nav::after`
- Можно чисто CSS через `mask-image` на `.pm-nav-scroll`

---

## 6. Skeleton Loading

**Вместо** спиннера + "Плов Центр" — skeleton-карточки.

**Реализация:** Чистый CSS skeleton (shimmer animation) без дополнительных зависимостей. Компонент `MenuSkeleton` в PublicMenu.tsx — nav placeholder + 6 карточек-скелетонов.

**Файлы:**
- `front/src/pages/public-menu.css` — стили `.pm-skeleton-*`
- `front/src/pages/PublicMenu.tsx` — компонент `MenuSkeleton`, заменить блок `pm-loading`

---

## 7. Dish Detail Modal

**При тапе на карточку** — модал с увеличенным фото, полным описанием, ценой.

**Реализация:** Простой CSS modal (не MUI Dialog — публичная страница не использует MUI). motion для enter/exit анимации. Overlay + centered card.

**Файлы:**
- `front/src/pages/public-menu.css` — стили `.pm-modal-*`
- `front/src/pages/PublicMenu.tsx` — компонент `DishModal`, state в `PublicMenu`

---

## 8. Admin Responsive Sidebar

**Проблема:** `Drawer variant="permanent"` — 260px всегда, мобилка сломана.

**Решение:**
- Desktop (≥768px): permanent drawer как есть
- Mobile (<768px): temporary drawer + hamburger в AppBar
- `useMediaQuery` из MUI

**Файлы:**
- `front/src/components/layout/AdminLayout.tsx` — responsive drawer logic
- Без новых файлов

---

## 9. Dashboard

**Текущее:** 2 карточки с числами + ссылка.

**Добавить:**
- Quick actions: кнопки "Добавить блюдо", "Добавить категорию" прямо на дашборде
- Ссылка на публичное меню с QR-code hint

**Файлы:**
- `front/src/pages/admin/Dashboard.tsx`

---

## 10. Login — theme tokens

**Заменить хардкод:**
- `'#1C1410'` → `theme.palette.secondary.main` или CSS var
- `'#C08B3F'` → `theme.palette.primary.main`
- `fontFamily: '"Playfair Display"...'` → Young Serif

**Файлы:**
- `front/src/pages/admin/Login.tsx`

---

## 11. Декоративные элементы / бренд

**Минимальные улучшения:**
- Hero ornament `✻` → заменить на SVG узор в узбекском стиле (геометрический ромбовидный паттерн)
- Hero divider — чуть шире, с decorative end caps
- Footer — добавить тот же бренд-элемент

**Файлы:**
- `front/src/pages/PublicMenu.tsx` — inline SVG
- `front/src/pages/public-menu.css` — стили

---

## Порядок реализации

1. Типографика + цвета (theme.ts, public-menu.css, index.html) — база для всего остального
2. Фон контентной области — убрать overlay
3. No-photo карточки — placeholder SVG
4. Accent дисциплина — разделители, декор
5. Декоративные элементы — hero ornament, dividers
6. Skeleton loading
7. Nav scroll hint
8. Dish detail modal
9. Admin responsive sidebar
10. Dashboard improvements
11. Login theme tokens
12. NotFound.tsx — обновить шрифт + цвета

## Verification

```bash
cd front && npm run dev    # визуальная проверка на localhost:3000
cd front && npm run build  # проверка что нет TS/build ошибок
cd front && npm run lint   # линтинг
```

Проверить:
- Публичное меню на мобильном viewport (375px, 640px, 960px)
- Карточки с фото и без фото — одинаковая высота
- Skeleton при загрузке
- Тап по карточке → модал
- Админка на мобильном — бургер + drawer
- Login страница — шрифт + цвета из темы
