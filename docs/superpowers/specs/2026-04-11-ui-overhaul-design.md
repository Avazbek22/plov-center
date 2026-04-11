# UI Overhaul: "Warm Soul" Design Spec

## Scope

Full visual upgrade of the public QR-menu page + light polish of admin panel. No functional changes, no new endpoints, no new dependencies. Everything built on existing `motion` + vanilla CSS + MUI stack.

## Assets

Copy `new_images/` photos to `front/public/`:
- `otabek-xatipov-MHt_fHi6a3M-unsplash.jpg` → `hero-bg.jpg` (replaces current)
- `otabek-xatipov-IuQl4SNJO3Q-unsplash.jpg` → `divider-1.jpg`
- `otabek-xatipov-a85Px4ZCSK4-unsplash.jpg` → `divider-2.jpg`
- `otabek-xatipov-Zz37fc9SZ5U-unsplash.jpg` → spare/footer background

---

## Public Menu — Changes

### 1. Hero Overhaul

**Background**: New hero photo (lyagan top-down, `MHt_fHi6a3M`). Softer gradient overlay — bottom-heavy, so the food is visible through the top half. Current overlay is 88% opaque; target is ~55-65% with gradient.

**Parallax**: Background image shifts at 0.3x scroll speed via CSS `transform: translateY(calc(var(--scroll) * 0.3))` updated by a lightweight scroll listener (requestAnimationFrame throttled).

**Title animation**: On mount, letter-spacing transitions from `0.3em` to `0.02em` over 800ms with opacity fade-in. Creates a "breathing in" effect.

**Ornament**: Replace generic SVG star with CSS adras-pattern divider — a row of diamond shapes inspired by the colorful patterns on the lyagan bowls in the photos. Pure CSS using `clip-path` or border tricks, colored with accent palette.

**About text**: Fade in 200ms after title, with subtle y-translate.

### 2. Background Texture & Depth

**Noise overlay**: CSS pseudo-element on `.pm` with a base64-encoded tiny noise PNG, `opacity: 0.03`, `mix-blend-mode: multiply`. Gives parchment/fabric feel without loading an image.

**Section gradient dividers**: Between each category section, a subtle warm gradient band (transparent → `rgba(196,132,29,0.04)` → transparent) via `::after` pseudo-element on `.pm-section`.

**Parallax photo divider**: After the 2nd category section (or after 50% of categories if fewer), insert a full-width parallax photo strip:
- Height: 200px mobile, 280px desktop
- One of the remaining photos as background, fixed attachment
- Semi-transparent warm overlay
- Optional: a short quote or "---" ornament centered

### 3. Card Micro-interactions

**Photo zoom on hover**: `.pm-dish-card-photo` wrapped in `overflow: hidden` container. On card hover → `img { transform: scale(1.08); transition: transform 500ms ease-out }`.

**Shadow & border**: On hover, shadow deepens from `--shadow-card` to a richer shadow, border transitions to `--border-accent`.

**Mobile press**: On `:active` → `transform: scale(0.97)` with 150ms transition. Touch feedback.

**Staggered entrance**: Change stagger from 40ms to 80ms. Each card: `opacity 0→1, y 20→0, scale 0.96→1` over 350ms with `easeOut`.

### 4. Animated Tab Navigation

**layoutId pill**: Instead of toggling `.pm-nav-item--active` class for background color, render a separate `motion.div` with `layoutId="nav-indicator"` behind the active tab button. This div has the accent background and `border-radius: 100px`. When active tab changes, motion auto-animates the indicator sliding to the new position.

**Implementation**: Each nav button is `position: relative`, the indicator is `position: absolute; inset: 0` inside the active button, with `layoutId` causing it to animate between buttons.

### 5. Section Header Animations

**Clip-path reveal**: Section headers use `clip-path: inset(0 100% 0 0)` → `clip-path: inset(0 0% 0 0)` animation when entering viewport. 600ms ease-out.

**Line draw**: The `<hr>` under section headers animates `width: 0 → 32px` from center, 400ms, 200ms delay after header.

### 6. Modal Upgrade

**Mobile (< 640px)**: Bottom sheet style.
- Enters from bottom: `y: "100%"` → `y: 0`.
- Border-radius only on top corners (16px).
- Max-height: 90dvh, scrollable.
- Drag-to-dismiss: `dragConstraints={{ top: 0 }}`, `onDragEnd` checks if dragged >100px down → close.

**Desktop (>= 640px)**: Current centered modal, but:
- Enter: `scale: 0.9, opacity: 0` → `scale: 1, opacity: 1` with spring physics (`stiffness: 300, damping: 25`).
- Backdrop blur increases from 0 to 8px.

**Ken Burns on photo**: Modal photo slowly zooms from `scale(1.0)` to `scale(1.06)` over 8 seconds, linear. Subtle but adds life.

### 7. Footer Redesign

**Dark warm background**: Footer gets `background: #2C2416` (the dark brown from theme), text becomes light. Creates visual weight at the bottom and contrast with the light menu area.

**Stroke-draw icons**: Contact icons animate their `stroke-dashoffset` from full to 0 when scrolling into view. 600ms per icon, staggered 150ms.

**Transition from content**: A gradient band from `--bg` to the dark footer color, about 60px tall, so the transition isn't harsh.

### 8. Skeleton Loading Upgrade

**Warmer shimmer**: Match the warm palette instead of generic gray. Shimmer gradient uses `rgba(196,132,29,0.06)` tones.

**Staggered cards**: Skeleton cards appear with stagger too, not all at once.

---

## Admin Panel — Light Polish

### 1. Dashboard Animated Counters

Numbers animate from 0 to value using `motion`'s `useMotionValue` + `useTransform` + `animate`. Duration 800ms, ease-out. Triggers on mount.

### 2. Route Transitions

Wrap admin routes in `AnimatePresence` with a simple fade+slide transition (opacity + y: 8px). 200ms in, 150ms out.

### 3. Table Polish

Row hover: warmer, more visible background transition (current `rgba(196,132,29,0.04)` → `0.08`).

### 4. Loading States

Replace any bare `CircularProgress` with skeleton shimmers matching the content layout (table skeleton, form skeleton).

### 5. Sidebar Active Indicator

Animated indicator bar (2px wide, accent color) on the left edge of active sidebar item, using `motion layoutId`.

---

## Files to Modify

### Public Menu
- `front/src/pages/PublicMenu.tsx` — hero, cards, nav, modal, footer, parallax logic
- `front/src/pages/public-menu.css` — all visual styles, textures, responsive updates
- `front/public/hero-bg.jpg` — replace with new photo
- `front/public/divider-1.jpg` — new asset
- `front/public/divider-2.jpg` — new asset

### Admin
- `front/src/pages/admin/Dashboard.tsx` — animated counters
- `front/src/components/layout/AdminLayout.tsx` — route transitions, sidebar indicator
- Admin page files — minimal loading state improvements

### No changes to
- Backend (no API changes)
- Types, hooks, API functions
- Auth flow
- Routing structure

## Technical Constraints

- No new npm dependencies
- All animations via `motion` (already installed) or CSS
- Parallax via rAF-throttled scroll listener, not scroll event spam
- All images lazy-loaded where appropriate
- `prefers-reduced-motion` respected (already have MotionConfig reducedMotion="user")
- Mobile-first responsive, no regressions on existing breakpoints
