# UI Overhaul: "Warm Soul" Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Transform the public QR-menu from a generic template into an immersive, appetizing experience with cultural character, and polish the admin panel with motion details.

**Architecture:** Pure frontend changes — CSS overhaul + motion animations on existing React components. No backend changes, no new dependencies. All animations via `motion` (Framer Motion) already installed, or CSS.

**Tech Stack:** React 19, motion 12 (Framer Motion), vanilla CSS, MUI 7

**Spec:** `docs/superpowers/specs/2026-04-11-ui-overhaul-design.md`

**No test projects exist.** Verification is visual — dev server + browser checks after each task.

---

### Task 1: Copy photo assets to public/

**Files:**
- Replace: `front/public/hero-bg.jpg`
- Create: `front/public/divider-1.jpg`
- Create: `front/public/divider-2.jpg`

- [ ] **Step 1: Copy and rename images**

```bash
cp new_images/otabek-xatipov-MHt_fHi6a3M-unsplash.jpg front/public/hero-bg.jpg
cp new_images/otabek-xatipov-IuQl4SNJO3Q-unsplash.jpg front/public/divider-1.jpg
cp new_images/otabek-xatipov-a85Px4ZCSK4-unsplash.jpg front/public/divider-2.jpg
```

- [ ] **Step 2: Commit**

```bash
git add front/public/hero-bg.jpg front/public/divider-1.jpg front/public/divider-2.jpg
git commit -m "assets: replace hero photo, add parallax divider images"
```

---

### Task 2: CSS foundation — tokens, texture, section depth

**Files:**
- Modify: `front/src/pages/public-menu.css` (lines 1–58, tokens + base sections)

This task rewrites the CSS variable system and adds the noise texture + section gradient dividers. No component changes yet.

- [ ] **Step 1: Update CSS tokens and add noise texture**

In `public-menu.css`, replace the tokens block (`.pm { --bg: ... }` through line 37) and add new variables:

```css
.pm {
  /* Existing tokens — keep all current values */
  --bg: #FAF7F2;
  --bg-elevated: #FFFFFF;
  --bg-card: #FFFFFF;
  --bg-card-hover: #FFF9F0;
  --text: #2C2416;
  --text-secondary: #6B5D4D;
  --text-muted: #A09585;
  --accent: #C4841D;
  --accent-hover: #A86E14;
  --border: rgba(0, 0, 0, 0.08);
  --border-accent: rgba(196, 132, 29, 0.25);
  --hero-bg: #3B2F1E;
  --hero-text: #FFFBF5;
  --hero-text-dim: rgba(255, 251, 245, 0.7);
  --shadow-card: 0 2px 12px rgba(0, 0, 0, 0.08), 0 1px 3px rgba(0, 0, 0, 0.04);
  --shadow-card-hover: 0 8px 28px rgba(0, 0, 0, 0.12);
  --shadow-nav: 0 1px 8px rgba(0, 0, 0, 0.08);
  --radius: 14px;
  --radius-sm: 8px;
  --font-display: 'Young Serif', Georgia, 'Times New Roman', serif;
  --font-body: 'DM Sans', system-ui, -apple-system, sans-serif;
  --content-width: 1100px;
  --nav-height: 56px;
  --px: 1rem;
  --card-img-h: 140px;
  --card-body-h: 120px;

  /* New tokens */
  --footer-bg: #2C2416;
  --footer-text: #F9F5EE;
  --footer-text-dim: rgba(249, 245, 238, 0.6);
  --shadow-card-rich: 0 8px 32px rgba(44, 36, 22, 0.14), 0 2px 8px rgba(44, 36, 22, 0.08);
}
```

- [ ] **Step 2: Add noise texture pseudo-element on `.pm`**

After the `.pm` base block (line ~58), add:

```css
/* Parchment noise texture */
.pm::before {
  content: '';
  position: fixed;
  inset: 0;
  z-index: 0;
  pointer-events: none;
  opacity: 0.025;
  mix-blend-mode: multiply;
  background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 256 256' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noise'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.9' numOctaves='4' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noise)'/%3E%3C/svg%3E");
  background-repeat: repeat;
  background-size: 256px 256px;
}

.pm > * {
  position: relative;
  z-index: 1;
}
```

- [ ] **Step 3: Add section gradient divider**

After the `.pm-section` rules (~line 248), add:

```css
.pm-section::after {
  content: '';
  display: block;
  height: 1px;
  margin-top: 2.5rem;
  background: linear-gradient(
    to right,
    transparent,
    rgba(196, 132, 29, 0.12),
    rgba(196, 132, 29, 0.06),
    transparent
  );
}

.pm-section:last-child::after {
  display: none;
}
```

- [ ] **Step 4: Verify in browser**

Run `cd front && npm run dev`, open `http://localhost:3000`. Check:
- Subtle noise texture visible over background (very faint grain)
- Warm gradient lines between category sections
- No layout regressions

- [ ] **Step 5: Commit**

```bash
git add front/src/pages/public-menu.css
git commit -m "style: add parchment texture, section gradient dividers, new tokens"
```

---

### Task 3: Hero overhaul — softer overlay, parallax, title animation, ornament

**Files:**
- Modify: `front/src/pages/PublicMenu.tsx` (lines 12–17 animation variants, lines 143–177 hero JSX)
- Modify: `front/src/pages/public-menu.css` (hero section, lines 73–172)

- [ ] **Step 1: Update hero CSS — softer overlay + parallax-ready**

Replace the entire hero CSS section (`.pm-hero` through `.pm-hero-photo`, lines 76–172) with:

```css
/* ══════════════
   Hero
   ══════════════ */
.pm-hero {
  color: var(--hero-text);
  text-align: center;
  position: relative;
  overflow: hidden;
  min-height: 380px;
}

.pm-hero-bg {
  position: absolute;
  inset: 0;
  background: url('/hero-bg.jpg') center 40% / cover no-repeat;
  z-index: 0;
  will-change: transform;
}

/* Softer gradient overlay — food visible through top */
.pm-hero::before {
  content: '';
  position: absolute;
  inset: 0;
  background:
    linear-gradient(
      to bottom,
      rgba(30, 20, 10, 0.45) 0%,
      rgba(30, 20, 10, 0.35) 30%,
      rgba(30, 20, 10, 0.55) 65%,
      rgba(30, 20, 10, 0.82) 100%
    );
  z-index: 1;
}

/* Warm vignette */
.pm-hero::after {
  content: '';
  position: absolute;
  inset: 0;
  background: radial-gradient(
    ellipse 70% 60% at 50% 45%,
    transparent 0%,
    rgba(20, 12, 5, 0.35) 100%
  );
  z-index: 1;
}

.pm-hero-inner {
  position: relative;
  z-index: 2;
  padding: 5rem var(--px) 3.5rem;
}

.pm-hero--with-image .pm-hero-inner {
  padding-bottom: 2rem;
}

/* Adras ornament divider */
.pm-hero-ornament {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  margin-bottom: 1.5rem;
  opacity: 0.8;
}

.pm-hero-ornament-diamond {
  width: 8px;
  height: 8px;
  transform: rotate(45deg);
  border: 1.5px solid var(--accent);
}

.pm-hero-ornament-diamond:nth-child(2) {
  width: 12px;
  height: 12px;
  background: rgba(196, 132, 29, 0.3);
}

.pm-hero-ornament-diamond:nth-child(4) {
  width: 12px;
  height: 12px;
  background: rgba(196, 132, 29, 0.3);
}

.pm-hero-title {
  font-family: var(--font-display);
  font-weight: 400;
  font-size: 2.8rem;
  line-height: 1.1;
  margin-bottom: 0.75rem;
  text-wrap: balance;
  text-shadow: 0 2px 20px rgba(0, 0, 0, 0.5);
}

.pm-hero-divider {
  width: 56px;
  height: 2px;
  background: var(--accent);
  border: none;
  margin: 0 auto 1.25rem;
  opacity: 0.6;
  box-shadow: 0 0 12px rgba(196, 132, 29, 0.25);
}

.pm-hero-about {
  font-size: 0.95rem;
  line-height: 1.7;
  color: rgba(255, 251, 245, 0.85);
  max-width: 420px;
  margin: 0 auto;
  text-wrap: pretty;
  text-shadow: 0 1px 4px rgba(0, 0, 0, 0.3);
}

.pm-hero-photo {
  width: 100%;
  max-height: 260px;
  object-fit: cover;
  display: block;
  position: relative;
  z-index: 2;
  mask-image: linear-gradient(to bottom, black 60%, transparent 100%);
  -webkit-mask-image: linear-gradient(to bottom, black 60%, transparent 100%);
}
```

- [ ] **Step 2: Update hero responsive breakpoints**

In the responsive sections, update hero references. At `@media (min-width: 480px)`:

```css
.pm-hero {
  min-height: 420px;
}

.pm-hero-title {
  font-size: 3.4rem;
}
```

At `@media (min-width: 960px)`:

```css
.pm-hero {
  min-height: 520px;
}

.pm-hero-inner {
  padding-top: 6rem;
  padding-bottom: 4rem;
}

.pm-hero-title {
  font-size: 4.2rem;
}

.pm-hero-about {
  font-size: 1.05rem;
  max-width: 500px;
}
```

- [ ] **Step 3: Update PublicMenu.tsx — hero parallax + new animations + adras ornament**

At the top of `PublicMenu.tsx`, add a `useScrollY` hook and update the animation variants. Replace lines 12–28:

```tsx
/* ── Scroll parallax hook ── */

function useScrollY() {
  const [scrollY, setScrollY] = useState(0);
  useEffect(() => {
    let ticking = false;
    const onScroll = () => {
      if (!ticking) {
        requestAnimationFrame(() => {
          setScrollY(window.scrollY);
          ticking = false;
        });
        ticking = true;
      }
    };
    window.addEventListener('scroll', onScroll, { passive: true });
    return () => window.removeEventListener('scroll', onScroll);
  }, []);
  return scrollY;
}

/* ── Animation variants ── */

const heroTitle = {
  hidden: { opacity: 0, letterSpacing: '0.25em' },
  visible: {
    opacity: 1,
    letterSpacing: '0.02em',
    transition: { duration: 0.8, ease: [0.22, 1, 0.36, 1] },
  },
};

const fadeUp = {
  hidden: { opacity: 0, y: 16 },
  visible: { opacity: 1, y: 0, transition: { duration: 0.5, ease: [0.22, 1, 0.36, 1] } },
};

const staggerChildren = {
  hidden: {},
  visible: { transition: { staggerChildren: 0.08 } },
};

const sectionHeaderReveal = {
  hidden: { opacity: 0, clipPath: 'inset(0 100% 0 0)' },
  visible: {
    opacity: 1,
    clipPath: 'inset(0 0% 0 0)',
    transition: { duration: 0.6, ease: [0.22, 1, 0.36, 1] },
  },
};

const lineGrow = {
  hidden: { width: 0 },
  visible: {
    width: 32,
    transition: { duration: 0.4, ease: 'easeOut', delay: 0.2 },
  },
};

const dishCard = {
  hidden: { opacity: 0, y: 20, scale: 0.96 },
  visible: {
    opacity: 1,
    y: 0,
    scale: 1,
    transition: { duration: 0.35, ease: 'easeOut' },
  },
};

const staggerDishes = {
  hidden: {},
  visible: { transition: { staggerChildren: 0.08 } },
};
```

- [ ] **Step 4: Update hero JSX — add parallax bg div + adras ornament**

Replace the hero `<header>` block (lines 147–177) with:

```tsx
{/* Hero */}
<header className={`pm-hero${about?.photoPath ? ' pm-hero--with-image' : ''}`}>
  <motion.div
    className="pm-hero-bg"
    style={{ y: scrollY * 0.3 }}
  />
  <motion.div
    className="pm-hero-inner"
    initial="hidden"
    animate="visible"
    variants={staggerChildren}
  >
    <motion.div className="pm-hero-ornament" variants={fadeUp}>
      <span className="pm-hero-ornament-diamond" />
      <span className="pm-hero-ornament-diamond" />
      <span className="pm-hero-ornament-diamond" />
      <span className="pm-hero-ornament-diamond" />
      <span className="pm-hero-ornament-diamond" />
    </motion.div>
    <motion.h1 className="pm-hero-title" variants={heroTitle}>
      Плов Центр
    </motion.h1>
    <motion.hr className="pm-hero-divider" variants={fadeUp} />
    {about?.text && (
      <motion.p className="pm-hero-about" variants={fadeUp}>
        {about.text}
      </motion.p>
    )}
  </motion.div>
  {about?.photoPath && (
    <img
      className="pm-hero-photo"
      src={imageUrl(about.photoPath)!}
      alt="Плов Центр"
    />
  )}
</header>
```

Add `scrollY` to the component:

```tsx
export default function PublicMenu() {
  const { data: menu, isLoading: menuLoading, error: menuError } = usePublicMenu();
  const { data: content, isLoading: contentLoading } = usePublicContent();
  const scrollY = useScrollY();
  // ... rest unchanged
```

- [ ] **Step 5: Verify in browser**

Check:
- Hero shows food through softer overlay
- Title animates with letter-spacing "breathe in" effect
- Adras diamond ornament instead of SVG star
- Background parallaxes on scroll (moves slower than page)
- No jank, smooth 60fps on scroll

- [ ] **Step 6: Commit**

```bash
git add front/src/pages/PublicMenu.tsx front/src/pages/public-menu.css
git commit -m "feat: hero overhaul — softer overlay, parallax, title animation, adras ornament"
```

---

### Task 4: Card micro-interactions + section header animations

**Files:**
- Modify: `front/src/pages/public-menu.css` (card sections, lines ~283–420; section header styles)
- Modify: `front/src/pages/PublicMenu.tsx` (MenuSection component, lines 247–275)

- [ ] **Step 1: Update card CSS — photo zoom, richer hover, mobile press**

Replace the dish card CSS section (`.pm-dish-card` through `.pm-dish-card-price`, lines ~286–348):

```css
/* ══════════════
   Dish Card (with photo) — vertical layout
   ══════════════ */
.pm-dish-card {
  display: flex;
  flex-direction: column;
  height: calc(var(--card-img-h) + var(--card-body-h) + 2px);
  background: var(--bg-card);
  border-radius: var(--radius);
  overflow: hidden;
  border: 1px solid var(--border);
  box-shadow: var(--shadow-card);
  transition: box-shadow 0.4s ease, border-color 0.4s ease;
}

.pm-dish-card-photo-wrap {
  width: 100%;
  height: var(--card-img-h);
  overflow: hidden;
  flex-shrink: 0;
}

.pm-dish-card-photo {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
  transition: transform 0.5s ease-out;
}

@media (hover: hover) {
  .pm-dish-card:hover {
    box-shadow: var(--shadow-card-rich);
    border-color: var(--border-accent);
  }

  .pm-dish-card:hover .pm-dish-card-photo {
    transform: scale(1.08);
  }
}

/* Mobile press feedback */
@media (hover: none) {
  .pm-dish-card:active {
    transform: scale(0.97);
    transition: transform 0.15s ease;
  }
}

.pm-dish-card-body {
  height: var(--card-body-h);
  padding: 0.75rem 1rem;
  display: flex;
  flex-direction: column;
  gap: 0.2rem;
  overflow: hidden;
}

.pm-dish-card-name {
  font-size: 1rem;
  font-weight: 600;
  line-height: 1.3;
  color: var(--text);
}

.pm-dish-card-desc {
  font-size: 0.82rem;
  line-height: 1.5;
  color: var(--text-secondary);
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  text-wrap: pretty;
}

.pm-dish-card-price {
  font-size: 1rem;
  font-weight: 700;
  color: var(--accent);
  margin-top: auto;
  padding-top: 0.5rem;
  font-variant-numeric: tabular-nums;
}
```

Also update `.pm-dish-item` hover to match:

```css
@media (hover: hover) {
  .pm-dish-item:hover {
    box-shadow: var(--shadow-card-rich);
    border-color: var(--border-accent);
  }
}

@media (hover: none) {
  .pm-dish-item:active {
    transform: scale(0.97);
    transition: transform 0.15s ease;
  }
}
```

- [ ] **Step 2: Add photo wrapper div in DishCard component**

In `PublicMenu.tsx`, update the `DishCard` component (lines 279–297):

```tsx
function DishCard({ dish }: { dish: PublicMenuDish }) {
  return (
    <div className="pm-dish-card">
      <div className="pm-dish-card-photo-wrap">
        <img
          className="pm-dish-card-photo"
          src={imageUrl(dish.photoPath)!}
          alt={dish.name}
          loading="lazy"
        />
      </div>
      <div className="pm-dish-card-body">
        <div className="pm-dish-card-name">{dish.name}</div>
        {dish.description && (
          <div className="pm-dish-card-desc">{dish.description}</div>
        )}
        <div className="pm-dish-card-price">{formatPrice(dish.price)} сум</div>
      </div>
    </div>
  );
}
```

- [ ] **Step 3: Update MenuSection to use new stagger + card variants**

Replace the `MenuSection` component (lines 247–275):

```tsx
function MenuSection({ category, onDishClick, ref }: MenuSectionProps) {
  return (
    <motion.section
      className="pm-section"
      ref={ref}
      initial="hidden"
      whileInView="visible"
      viewport={{ once: true, margin: '0px 0px -10% 0px' }}
      variants={staggerChildren}
    >
      <motion.h2
        className="pm-section-header"
        variants={sectionHeaderReveal}
      >
        {category.name}
      </motion.h2>
      <motion.div className="pm-section-line" variants={lineGrow} />

      <motion.div
        className="pm-section-dishes"
        variants={staggerDishes}
      >
        {category.dishes.map((dish) => (
          <motion.div key={dish.id} variants={dishCard} onClick={() => onDishClick(dish)} style={{ cursor: 'pointer' }}>
            {dish.photoPath ? (
              <DishCard dish={dish} />
            ) : (
              <DishListItem dish={dish} />
            )}
          </motion.div>
        ))}
      </motion.div>
    </motion.section>
  );
}
```

- [ ] **Step 4: Verify in browser**

Check:
- Section headers reveal left-to-right with clip-path
- Underline draws from 0 to 32px with delay
- Card photos zoom smoothly on hover (desktop)
- Shadow deepens, border glows accent on hover
- Mobile: tap press-down feedback
- Cards stagger in with slide-up + scale when scrolling into view
- No overflow glitches on photo zoom

- [ ] **Step 5: Commit**

```bash
git add front/src/pages/PublicMenu.tsx front/src/pages/public-menu.css
git commit -m "feat: card micro-interactions — photo zoom, rich hover, mobile press, stagger"
```

---

### Task 5: Animated tab navigation — layoutId sliding pill

**Files:**
- Modify: `front/src/pages/PublicMenu.tsx` (nav JSX, lines 180–195)
- Modify: `front/src/pages/public-menu.css` (nav styles, lines ~177–234)

- [ ] **Step 1: Update nav CSS for relative positioning**

Replace `.pm-nav-item` and `.pm-nav-item--active` styles:

```css
.pm-nav-item {
  flex-shrink: 0;
  padding: 8px 18px;
  border-radius: 100px;
  font-family: var(--font-body);
  font-size: 0.85rem;
  font-weight: 500;
  color: var(--text-secondary);
  background: transparent;
  border: 1.5px solid var(--border);
  cursor: pointer;
  transition: color 0.25s ease, border-color 0.25s ease;
  white-space: nowrap;
  user-select: none;
  -webkit-tap-highlight-color: transparent;
  position: relative;
  z-index: 1;
}

.pm-nav-item:hover {
  color: var(--text);
  border-color: var(--text-muted);
}

.pm-nav-item--active {
  color: #FFFBF5;
  border-color: transparent;
}

.pm-nav-item--active:hover {
  color: #FFFBF5;
  border-color: transparent;
}

.pm-nav-pill {
  position: absolute;
  inset: 0;
  background: var(--accent);
  border-radius: 100px;
  z-index: -1;
}
```

- [ ] **Step 2: Update nav JSX with layoutId indicator**

Replace the nav section (lines 180–195):

```tsx
{/* Category navigation */}
{categories.length > 0 && (
  <nav className="pm-nav" ref={navRef}>
    <div className="pm-nav-scroll" ref={navScrollRef}>
      {categories.map((cat, i) => (
        <button
          key={cat.id}
          className={`pm-nav-item${i === activeTab ? ' pm-nav-item--active' : ''}`}
          onClick={() => handleTabClick(i)}
          type="button"
        >
          {cat.name}
          {i === activeTab && (
            <motion.div
              className="pm-nav-pill"
              layoutId="nav-pill"
              transition={{ type: 'spring', stiffness: 400, damping: 30 }}
            />
          )}
        </button>
      ))}
    </div>
  </nav>
)}
```

- [ ] **Step 3: Verify in browser**

Check:
- Clicking a different category tab → pill slides smoothly to new position
- Scrolling through sections → pill animates as activeTab changes
- No layout jump or flash during transition

- [ ] **Step 4: Commit**

```bash
git add front/src/pages/PublicMenu.tsx front/src/pages/public-menu.css
git commit -m "feat: animated tab navigation with sliding pill indicator"
```

---

### Task 6: Parallax photo divider between categories

**Files:**
- Modify: `front/src/pages/PublicMenu.tsx` (main content area, lines 198–221)
- Modify: `front/src/pages/public-menu.css` (add divider styles)

- [ ] **Step 1: Add parallax divider CSS**

Add to `public-menu.css`:

```css
/* ══════════════
   Parallax Photo Divider
   ══════════════ */
.pm-divider {
  position: relative;
  height: 200px;
  overflow: hidden;
  margin: 2rem calc(-1 * var(--px));
}

.pm-divider-bg {
  position: absolute;
  inset: -60px 0;
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  will-change: transform;
}

.pm-divider::after {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(
    to bottom,
    rgba(30, 20, 10, 0.5),
    rgba(30, 20, 10, 0.35),
    rgba(30, 20, 10, 0.5)
  );
}

.pm-divider-ornament {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  z-index: 1;
  display: flex;
  align-items: center;
  gap: 12px;
  color: rgba(255, 251, 245, 0.6);
}

.pm-divider-ornament-line {
  width: 40px;
  height: 1px;
  background: currentColor;
}

.pm-divider-ornament-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  border: 1px solid currentColor;
}

@media (min-width: 640px) {
  .pm-divider {
    height: 240px;
    margin-left: calc(-1 * var(--px));
    margin-right: calc(-1 * var(--px));
  }
}

@media (min-width: 960px) {
  .pm-divider {
    height: 280px;
  }
}
```

- [ ] **Step 2: Add PhotoDivider component and insert into content**

Add a new component in `PublicMenu.tsx`:

```tsx
function PhotoDivider({ image, scrollY }: { image: string; scrollY: number }) {
  return (
    <div className="pm-divider">
      <motion.div
        className="pm-divider-bg"
        style={{
          backgroundImage: `url(${image})`,
          y: scrollY * 0.15,
        }}
      />
      <div className="pm-divider-ornament">
        <span className="pm-divider-ornament-line" />
        <span className="pm-divider-ornament-dot" />
        <span className="pm-divider-ornament-line" />
      </div>
    </div>
  );
}
```

Update the `<main>` content area to insert dividers:

```tsx
<main className="pm-content pm-container">
  {categories.map((category, index) => (
    <div key={category.id}>
      <MenuSection
        category={category}
        onDishClick={setSelectedDish}
        ref={(el: HTMLDivElement | null) => {
          if (el) sectionRefs.current.set(index, el);
          else sectionRefs.current.delete(index);
        }}
      />
      {index === Math.floor(categories.length / 2) - 1 && categories.length >= 3 && (
        <PhotoDivider image="/divider-1.jpg" scrollY={scrollY} />
      )}
    </div>
  ))}

  {categories.length === 0 && (
    <div className="pm-empty" style={{ minHeight: '40vh' }}>
      <p className="pm-empty-text">Меню скоро появится</p>
      {contacts?.phone && (
        <a className="pm-empty-action" href={`tel:${contacts.phone.replace(/\s/g, '')}`}>
          Позвонить: {contacts.phone}
        </a>
      )}
    </div>
  )}
</main>
```

- [ ] **Step 3: Verify in browser**

Check:
- Photo divider appears between the middle categories
- Parallax effect on the divider background
- Ornament centered
- No overflow issues at any breakpoint

- [ ] **Step 4: Commit**

```bash
git add front/src/pages/PublicMenu.tsx front/src/pages/public-menu.css
git commit -m "feat: parallax photo divider between menu categories"
```

---

### Task 7: Modal upgrade — bottom sheet on mobile, spring on desktop, Ken Burns

**Files:**
- Modify: `front/src/pages/PublicMenu.tsx` (DishModal component, lines 324–377)
- Modify: `front/src/pages/public-menu.css` (modal styles, lines ~657–755)

- [ ] **Step 1: Add useIsMobile hook**

Add near the top of `PublicMenu.tsx`, after `useScrollY`:

```tsx
function useIsMobile() {
  const [isMobile, setIsMobile] = useState(false);
  useEffect(() => {
    const mq = window.matchMedia('(max-width: 639px)');
    setIsMobile(mq.matches);
    const handler = (e: MediaQueryListEvent) => setIsMobile(e.matches);
    mq.addEventListener('change', handler);
    return () => mq.removeEventListener('change', handler);
  }, []);
  return isMobile;
}
```

- [ ] **Step 2: Update DishModal component**

Replace the entire `DishModal` function:

```tsx
function DishModal({ dish, onClose }: { dish: PublicMenuDish; onClose: () => void }) {
  const isMobile = useIsMobile();

  useEffect(() => {
    document.body.style.overflow = 'hidden';
    const handleKey = (e: KeyboardEvent) => { if (e.key === 'Escape') onClose(); };
    window.addEventListener('keydown', handleKey);
    return () => {
      document.body.style.overflow = '';
      window.removeEventListener('keydown', handleKey);
    };
  }, [onClose]);

  return (
    <motion.div
      className="pm-modal-overlay"
      initial={{ opacity: 0, backdropFilter: 'blur(0px)' }}
      animate={{ opacity: 1, backdropFilter: 'blur(8px)' }}
      exit={{ opacity: 0, backdropFilter: 'blur(0px)' }}
      transition={{ duration: 0.25 }}
      onClick={onClose}
    >
      <motion.div
        className={`pm-modal ${isMobile ? 'pm-modal--sheet' : ''}`}
        initial={isMobile
          ? { y: '100%' }
          : { opacity: 0, scale: 0.92 }
        }
        animate={isMobile
          ? { y: 0 }
          : { opacity: 1, scale: 1 }
        }
        exit={isMobile
          ? { y: '100%' }
          : { opacity: 0, scale: 0.92 }
        }
        transition={isMobile
          ? { type: 'spring', stiffness: 350, damping: 35 }
          : { type: 'spring', stiffness: 300, damping: 25 }
        }
        drag={isMobile ? 'y' : false}
        dragConstraints={{ top: 0 }}
        dragElastic={0.1}
        onDragEnd={(_e, info) => {
          if (info.offset.y > 100) onClose();
        }}
        onClick={(e) => e.stopPropagation()}
        style={{ position: 'relative' }}
      >
        {isMobile && <div className="pm-modal-handle" />}

        <button className="pm-modal-close" onClick={onClose} type="button" aria-label="Закрыть">
          &times;
        </button>

        {dish.photoPath ? (
          <div className="pm-modal-photo-wrap">
            <motion.img
              className="pm-modal-photo"
              src={imageUrl(dish.photoPath)!}
              alt={dish.name}
              initial={{ scale: 1 }}
              animate={{ scale: 1.06 }}
              transition={{ duration: 8, ease: 'linear' }}
            />
          </div>
        ) : (
          <div className="pm-modal-placeholder">
            <svg viewBox="0 0 64 64" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
              <ellipse cx="32" cy="38" rx="22" ry="10" />
              <path d="M10 38c0-12 9.8-22 22-22s22 10 22 22" />
              <path d="M32 16v-4M28 13l4-4 4 4" />
            </svg>
          </div>
        )}

        <div className="pm-modal-body">
          <div className="pm-modal-name">{dish.name}</div>
          {dish.description && <div className="pm-modal-desc">{dish.description}</div>}
          <div className="pm-modal-price">{formatPrice(dish.price)} сум</div>
        </div>
      </motion.div>
    </motion.div>
  );
}
```

- [ ] **Step 3: Update modal CSS**

Replace the modal CSS section (lines ~657–755):

```css
/* ══════════════
   Dish Modal
   ══════════════ */
.pm-modal-overlay {
  position: fixed;
  inset: 0;
  z-index: 100;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1rem;
}

.pm-modal {
  background: var(--bg-card);
  border-radius: var(--radius);
  overflow: hidden;
  max-width: 480px;
  width: 100%;
  max-height: 85vh;
  max-height: 85dvh;
  overflow-y: auto;
  box-shadow: 0 24px 64px rgba(0, 0, 0, 0.3);
}

/* Bottom sheet mode (mobile) */
.pm-modal--sheet {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  max-width: 100%;
  max-height: 90dvh;
  border-radius: 20px 20px 0 0;
  margin: 0;
  touch-action: pan-y;
}

.pm-modal-handle {
  width: 36px;
  height: 4px;
  border-radius: 2px;
  background: var(--text-muted);
  opacity: 0.3;
  margin: 10px auto 0;
}

.pm-modal-photo-wrap {
  width: 100%;
  max-height: 320px;
  overflow: hidden;
}

.pm-modal-photo {
  width: 100%;
  max-height: 320px;
  object-fit: cover;
  display: block;
  will-change: transform;
}

.pm-modal-placeholder {
  width: 100%;
  height: 200px;
  background: linear-gradient(135deg, #F5EFE4 0%, #EDE5D6 100%);
  display: flex;
  align-items: center;
  justify-content: center;
}

.pm-modal-placeholder svg {
  width: 64px;
  height: 64px;
  opacity: 0.2;
  color: var(--text-muted);
}

.pm-modal-body {
  padding: 1.25rem 1.25rem 1.5rem;
}

.pm-modal-name {
  font-family: var(--font-display);
  font-size: 1.35rem;
  font-weight: 400;
  color: var(--text);
  line-height: 1.3;
  margin-bottom: 0.5rem;
}

.pm-modal-desc {
  font-size: 0.9rem;
  line-height: 1.65;
  color: var(--text-secondary);
  margin-bottom: 1rem;
  text-wrap: pretty;
}

.pm-modal-price {
  font-size: 1.2rem;
  font-weight: 700;
  color: var(--accent);
  font-variant-numeric: tabular-nums;
}

.pm-modal-close {
  position: absolute;
  top: 12px;
  right: 12px;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border: none;
  background: rgba(0, 0, 0, 0.4);
  color: #fff;
  font-size: 1.2rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background 0.2s ease;
  -webkit-tap-highlight-color: transparent;
  z-index: 2;
}

.pm-modal-close:hover {
  background: rgba(0, 0, 0, 0.6);
}

.pm-modal--sheet .pm-modal-close {
  top: 16px;
}
```

- [ ] **Step 4: Verify in browser**

Desktop check:
- Modal opens with spring scale animation
- Backdrop blurs in
- Photo slowly zooms (Ken Burns)

Mobile check (resize to < 640px or use devtools):
- Modal slides up from bottom as a sheet
- Drag handle visible at top
- Drag down > 100px → dismisses
- Top corners rounded, bottom flush

- [ ] **Step 5: Commit**

```bash
git add front/src/pages/PublicMenu.tsx front/src/pages/public-menu.css
git commit -m "feat: modal upgrade — bottom sheet mobile, spring desktop, Ken Burns photo"
```

---

### Task 8: Footer redesign — dark background, stroke-draw icons, gradient transition

**Files:**
- Modify: `front/src/pages/PublicMenu.tsx` (ContactsFooter component, lines 414–472)
- Modify: `front/src/pages/public-menu.css` (footer styles, lines ~428–498)

- [ ] **Step 1: Update footer CSS**

Replace the footer CSS section:

```css
/* ══════════════
   Contacts Footer
   ══════════════ */
.pm-footer-transition {
  height: 60px;
  background: linear-gradient(to bottom, var(--bg), var(--footer-bg));
  margin-top: 2rem;
}

.pm-footer {
  background: var(--footer-bg);
  color: var(--footer-text);
  padding-top: 2.5rem;
  padding-bottom: 3rem;
  margin-top: 0;
  border-top: none;
}

.pm-footer-divider {
  display: none;
}

.pm-footer-title {
  font-family: var(--font-display);
  font-weight: 600;
  font-size: 1.3rem;
  color: var(--footer-text);
  margin-bottom: 1.5rem;
  text-wrap: balance;
}

.pm-footer-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.pm-footer-row {
  display: flex;
  align-items: flex-start;
  gap: 0.85rem;
}

.pm-footer-icon {
  flex-shrink: 0;
  width: 20px;
  height: 20px;
  color: var(--accent);
  margin-top: 2px;
}

.pm-footer-text {
  font-size: 0.9rem;
  color: var(--footer-text-dim);
  line-height: 1.5;
}

.pm-footer-phone {
  font-size: 0.9rem;
  font-weight: 600;
  color: var(--accent);
  text-decoration: none;
  transition: color 0.2s ease;
}

.pm-footer-phone:hover {
  color: var(--accent-hover);
}

.pm-footer-map {
  margin-top: 1.75rem;
  border-radius: var(--radius);
  overflow: hidden;
  line-height: 0;
  border: 1px solid rgba(255, 255, 255, 0.08);
}

.pm-footer-map iframe {
  width: 100% !important;
  border-radius: var(--radius);
}
```

- [ ] **Step 2: Update ContactsFooter component JSX**

Replace the `ContactsFooter` function:

```tsx
function ContactsFooter({ contacts }: { contacts: PublicContacts }) {
  return (
    <>
      <div className="pm-footer-transition" />
      <footer className="pm-footer pm-container">
        <motion.div
          initial="hidden"
          whileInView="visible"
          viewport={{ once: true, margin: '-40px' }}
          variants={staggerChildren}
        >
          <motion.h2 className="pm-footer-title" variants={fadeUp}>
            Контакты
          </motion.h2>

          <div className="pm-footer-list">
            {contacts.address && (
              <motion.div className="pm-footer-row" variants={fadeUp}>
                <svg className="pm-footer-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z" />
                  <circle cx="12" cy="10" r="3" />
                </svg>
                <span className="pm-footer-text">{contacts.address}</span>
              </motion.div>
            )}

            {contacts.phone && (
              <motion.div className="pm-footer-row" variants={fadeUp}>
                <svg className="pm-footer-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z" />
                </svg>
                <a className="pm-footer-phone" href={`tel:${contacts.phone.replace(/\s/g, '')}`}>
                  {contacts.phone}
                </a>
              </motion.div>
            )}

            {contacts.hours && (
              <motion.div className="pm-footer-row" variants={fadeUp}>
                <svg className="pm-footer-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <circle cx="12" cy="12" r="10" />
                  <polyline points="12 6 12 12 16 14" />
                </svg>
                <span className="pm-footer-text">{contacts.hours}</span>
              </motion.div>
            )}
          </div>

          {contacts.mapEmbed && (
            <motion.div
              className="pm-footer-map"
              variants={fadeUp}
              dangerouslySetInnerHTML={{ __html: contacts.mapEmbed }}
            />
          )}
        </motion.div>
      </footer>
    </>
  );
}
```

- [ ] **Step 3: Verify in browser**

Check:
- Smooth gradient transition from light content to dark footer
- Footer text is light on dark background
- Accent color still works on dark bg
- Map embed has subtle border
- Scroll into view → elements fade up

- [ ] **Step 4: Commit**

```bash
git add front/src/pages/PublicMenu.tsx front/src/pages/public-menu.css
git commit -m "feat: footer redesign — dark warm background with gradient transition"
```

---

### Task 9: Skeleton loading upgrade — warmer shimmer

**Files:**
- Modify: `front/src/pages/public-menu.css` (skeleton styles, lines ~780–876)

- [ ] **Step 1: Update skeleton shimmer colors**

In the skeleton section, update the shimmer gradients:

```css
.pm-skeleton-bone {
  background: linear-gradient(90deg, rgba(196,132,29,0.06) 25%, rgba(196,132,29,0.12) 50%, rgba(196,132,29,0.06) 75%);
  background-size: 200% 100%;
  animation: pm-shimmer 1.5s ease-in-out infinite;
  border-radius: var(--radius-sm);
}

.pm-skeleton-bone--light {
  background: linear-gradient(90deg, rgba(196,132,29,0.03) 25%, rgba(196,132,29,0.07) 50%, rgba(196,132,29,0.03) 75%);
  background-size: 200% 100%;
  animation: pm-shimmer 1.5s ease-in-out infinite;
  border-radius: var(--radius-sm);
}
```

- [ ] **Step 2: Verify — reload page with throttled network to see skeleton**

In browser devtools, throttle network to Slow 3G, reload. Check:
- Skeleton shimmer has warm golden tones instead of gray

- [ ] **Step 3: Commit**

```bash
git add front/src/pages/public-menu.css
git commit -m "style: warmer skeleton shimmer matching brand palette"
```

---

### Task 10: Admin — Dashboard animated counters

**Files:**
- Modify: `front/src/pages/admin/Dashboard.tsx`

- [ ] **Step 1: Add animated counter component**

Add imports and the `AnimatedNumber` component at the top of `Dashboard.tsx`:

```tsx
import { useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { animate } from 'motion';
import Box from '@mui/material/Box';
// ... rest of existing imports
```

Add before the `Dashboard` function:

```tsx
function AnimatedNumber({ value }: { value: number }) {
  const ref = useRef<HTMLSpanElement>(null);
  const prevValue = useRef(0);

  useEffect(() => {
    const el = ref.current;
    if (!el) return;

    const controls = animate(prevValue.current, value, {
      duration: 0.8,
      ease: [0.22, 1, 0.36, 1],
      onUpdate: (v) => {
        el.textContent = Math.round(v).toString();
      },
    });

    prevValue.current = value;
    return () => controls.stop();
  }, [value]);

  return <span ref={ref}>0</span>;
}
```

- [ ] **Step 2: Replace static number display with animated version**

In the stats card rendering, replace the Typography that shows `{s.value}`:

```tsx
<Typography
  sx={{
    fontSize: '2.5rem',
    fontWeight: 700,
    color: 'primary.main',
    lineHeight: 1,
  }}
>
  <AnimatedNumber value={s.value} />
</Typography>
```

- [ ] **Step 3: Remove unused `useState` import if present, add `useRef`**

Ensure the imports line reads:

```tsx
import { useEffect, useRef } from 'react';
```

(Remove `useState` only if not used elsewhere in the file — it's not used in Dashboard.)

- [ ] **Step 4: Verify in browser**

Navigate to `/admin`. Check:
- Numbers animate from 0 to their values on page load
- Animation is smooth with ease-out feel
- Navigating away and back replays the animation

- [ ] **Step 5: Commit**

```bash
git add front/src/pages/admin/Dashboard.tsx
git commit -m "feat: admin dashboard animated counters"
```

---

### Task 11: Admin — Sidebar animated active indicator

**Files:**
- Modify: `front/src/components/layout/AdminSidebar.tsx`

- [ ] **Step 1: Add motion import and layoutId indicator**

Replace the entire `AdminSidebar.tsx`:

```tsx
import { Link, useLocation } from 'react-router-dom';
import { motion } from 'motion/react';
import List from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import DashboardIcon from '@mui/icons-material/Dashboard';
import CategoryIcon from '@mui/icons-material/Category';
import RestaurantMenuIcon from '@mui/icons-material/RestaurantMenu';
import ArticleIcon from '@mui/icons-material/Article';

const navItems = [
  { label: 'Дашборд', icon: <DashboardIcon />, path: '/admin', exact: true },
  { label: 'Категории', icon: <CategoryIcon />, path: '/admin/categories', exact: false },
  { label: 'Блюда', icon: <RestaurantMenuIcon />, path: '/admin/dishes', exact: false },
  { label: 'Контент', icon: <ArticleIcon />, path: '/admin/content', exact: false },
] as const;

export default function AdminSidebar() {
  const { pathname } = useLocation();

  return (
    <List component="nav" sx={{ px: 1, pt: 1.5 }}>
      {navItems.map(({ label, icon, path, exact }) => {
        const isActive = exact ? pathname === path : pathname.startsWith(path);

        return (
          <ListItemButton
            key={path}
            component={Link}
            to={path}
            selected={isActive}
            sx={{ mb: 0.5, py: 1, position: 'relative' }}
          >
            {isActive && (
              <motion.div
                layoutId="sidebar-indicator"
                transition={{ type: 'spring', stiffness: 400, damping: 30 }}
                style={{
                  position: 'absolute',
                  left: 0,
                  top: 4,
                  bottom: 4,
                  width: 3,
                  borderRadius: 2,
                  background: '#C4841D',
                }}
              />
            )}
            <ListItemIcon sx={{ fontSize: 20 }}>{icon}</ListItemIcon>
            <ListItemText
              primary={label}
              primaryTypographyProps={{ fontSize: '0.9rem', fontWeight: 500 }}
            />
          </ListItemButton>
        );
      })}
    </List>
  );
}
```

- [ ] **Step 2: Verify in browser**

Navigate between admin pages. Check:
- Orange indicator bar on left of active item
- Bar slides smoothly between items with spring physics
- Works on both desktop sidebar and mobile drawer

- [ ] **Step 3: Commit**

```bash
git add front/src/components/layout/AdminSidebar.tsx
git commit -m "feat: admin sidebar animated active indicator"
```

---

### Task 12: Admin — Route transitions

**Files:**
- Modify: `front/src/components/layout/AdminLayout.tsx`

- [ ] **Step 1: Add AnimatePresence wrapper around Outlet**

Update `AdminLayout.tsx`. Add imports:

```tsx
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { AnimatePresence, motion } from 'motion/react';
```

Replace the `<Outlet />` in the main content area:

```tsx
<Box component="main" sx={{ flexGrow: 1, p: { xs: 1.5, sm: 3 }, bgcolor: 'background.default' }}>
  <AnimatePresence mode="wait">
    <motion.div
      key={useLocation().pathname}
      initial={{ opacity: 0, y: 8 }}
      animate={{ opacity: 1, y: 0 }}
      exit={{ opacity: 0, y: -8 }}
      transition={{ duration: 0.2, ease: 'easeOut' }}
    >
      <Outlet />
    </motion.div>
  </AnimatePresence>
</Box>
```

Note: We need to extract `location` to a variable since we can't call hooks in JSX. Update the component:

```tsx
export default function AdminLayout() {
  const { session, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));
  const [mobileOpen, setMobileOpen] = useState(false);
  // ... rest unchanged

  // Then in JSX:
  <Box component="main" sx={{ flexGrow: 1, p: { xs: 1.5, sm: 3 }, bgcolor: 'background.default' }}>
    <AnimatePresence mode="wait">
      <motion.div
        key={location.pathname}
        initial={{ opacity: 0, y: 8 }}
        animate={{ opacity: 1, y: 0 }}
        exit={{ opacity: 0, y: -8 }}
        transition={{ duration: 0.2, ease: 'easeOut' }}
      >
        <Outlet />
      </motion.div>
    </AnimatePresence>
  </Box>
```

- [ ] **Step 2: Verify in browser**

Navigate between admin pages. Check:
- Page content fades out with slight upward motion
- New page fades in with slight downward motion
- Transition is quick (200ms), not sluggish
- No flash of blank content

- [ ] **Step 3: Warm up table row hover**

In `front/src/theme/theme.ts`, update the `MuiTableRow` hover opacity:

Change `rgba(196, 132, 29, 0.04)` to `rgba(196, 132, 29, 0.08)` in the `MuiTableRow` styleOverrides:

```ts
MuiTableRow: {
  styleOverrides: {
    root: {
      transition: 'background-color 0.2s ease',
      '&:hover': { backgroundColor: 'rgba(196, 132, 29, 0.08)' },
      '&:last-child td': { borderBottom: 0 },
    },
  },
},
```

- [ ] **Step 4: Commit**

```bash
git add front/src/components/layout/AdminLayout.tsx front/src/theme/theme.ts
git commit -m "feat: admin route transitions, warmer table hover"
```

---

### Task 13: Final pass — verify all features together, lint, build

**Files:** None new — verification only.

- [ ] **Step 1: Run lint**

```bash
cd front && npm run lint
```

Fix any errors.

- [ ] **Step 2: Run build**

```bash
cd front && npm run build
```

Fix any type errors.

- [ ] **Step 3: Full visual QA in browser**

Run dev server, check the full flow:

Public menu:
- [ ] Hero: parallax, letter-spacing title animation, adras ornament, soft overlay
- [ ] Noise texture on background (faint grain)
- [ ] Nav: pill slides between tabs
- [ ] Cards: photo zoom on hover, rich shadow, mobile press
- [ ] Staggered card entrance on scroll
- [ ] Section gradient dividers
- [ ] Parallax photo divider between categories
- [ ] Modal desktop: spring open, Ken Burns photo, blur backdrop
- [ ] Modal mobile: bottom sheet, drag to dismiss
- [ ] Footer: dark bg, gradient transition from content
- [ ] Skeleton: warm shimmer

Admin:
- [ ] Dashboard: counters animate from 0
- [ ] Sidebar: indicator slides between items
- [ ] Route transitions: fade + slide between pages

- [ ] **Step 4: Final commit if any QA fixes needed**

```bash
git add -A
git commit -m "fix: visual QA adjustments"
```
