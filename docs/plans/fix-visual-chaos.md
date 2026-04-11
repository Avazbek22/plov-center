# Fix Visual Chaos — Post-Overhaul Corrections

## Проблемы (по скриншотам Playwright)

### P1. Hero: два конфликтующих изображения
CSS background `hero-bg.jpg` + HTML `<img class="pm-hero-photo">` (about.photoPath из CMS). Два больших фото в hero = визуальный конфликт. На desktop повар из CMS перекрывает фон.

**Fix:** Убрать `pm-hero-photo` из hero когда есть CSS background. About-фото из CMS не нужно в hero — фоновое изображение уже задаёт атмосферу. Если admin загрузит about-фото, показывать его отдельной секцией ПОД hero, а не внутри.

**Файлы:** `PublicMenu.tsx` — убрать `<img className="pm-hero-photo">` из hero, `public-menu.css` — удалить `.pm-hero-photo`, `.pm-hero--with-image`

### P2. Hero overlay слишком тёмный на mobile
`rgba(30, 20, 10, 0.88)` сверху — фон почти не виден на маленьком экране.

**Fix:** Облегчить overlay на mobile: сверху 0.78 вместо 0.88, середина 0.55 вместо 0.72. На desktop оставить как есть.

**Файлы:** `public-menu.css` — media query для `.pm-hero::before`

### P3. Бледная палитра — нет контраста
Кремовый `#FAF7F2` + белые `#FFFFFF` карточки — разница ~3%. Всё сливается.

**Fix:** Усилить фон до `#F5F0E8` (теплее, контрастнее к белым карточкам). Добавить тень карточкам чуть заметнее. Секционные разделители вернуть в полупрозрачный accent вместо muted (перестарались с "дисциплиной").

**Файлы:** `public-menu.css` — `--bg`, `.pm-section-line`

### P4. No-photo placeholder слишком бледный
Gradient `#F5EFE4 → #EDE5D6` почти не отличим от белого фона карточки.

**Fix:** Сделать placeholder заметнее: `#EDE5D6 → #E2D8C8` с чуть более видимой SVG-иконкой (opacity 0.25 → 0.35).

**Файлы:** `public-menu.css` — `.pm-dish-item-placeholder`

### P5. Star ornament — generic
Пятиконечная звезда не передаёт узбекский характер.

**Fix:** Заменить на 8-конечную звезду (традиционный исламский/узбекский узор) или геометрический ромб.

**Файлы:** `PublicMenu.tsx` — SVG в hero ornament

---

## Порядок

1. P1 — убрать about-фото из hero (ломает layout)
2. P2 — облегчить overlay на mobile
3. P3 — усилить контраст палитры
4. P4 — placeholder заметнее
5. P5 — ornament

## Verification

Снять скриншоты Playwright после каждого фикса: mobile 375px, desktop 1280px.
