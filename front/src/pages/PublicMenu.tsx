import { useCallback, useEffect, useRef, useState } from 'react';
import { AnimatePresence, motion, MotionConfig } from 'motion/react';
import { usePublicMenu, usePublicContent } from '@/hooks/use-public-menu';
import { imageUrl } from '@/utils/image-url';
import type { PublicMenuCategory, PublicMenuDish, PublicContacts } from '@/types/public';
import './public-menu.css';

function formatPrice(price: number): string {
  return price.toLocaleString('ru-RU');
}

/* ── Animation variants ── */

const fadeUp = {
  hidden: { opacity: 0, y: 12 },
  visible: { opacity: 1, y: 0, transition: { duration: 0.3, ease: 'easeOut' as const } },
};

const staggerChildren = {
  hidden: {},
  visible: { transition: { staggerChildren: 0.04 } },
};

const dishFade = {
  hidden: { opacity: 0 },
  visible: { opacity: 1, transition: { duration: 0.25, ease: 'easeOut' as const } },
};

/* ── Page ── */

export default function PublicMenu() {
  const { data: menu, isLoading: menuLoading, error: menuError } = usePublicMenu();
  const { data: content, isLoading: contentLoading } = usePublicContent();

  const [activeTab, setActiveTab] = useState(0);
  const sectionRefs = useRef<Map<number, HTMLDivElement>>(new Map());
  const navRef = useRef<HTMLDivElement>(null);
  const navScrollRef = useRef<HTMLDivElement>(null);
  const isScrollingByClick = useRef(false);

  const [selectedDish, setSelectedDish] = useState<PublicMenuDish | null>(null);

  const categories = menu?.categories ?? [];

  const handleTabClick = useCallback((index: number) => {
    setActiveTab(index);
    const section = sectionRefs.current.get(index);
    if (section && navRef.current) {
      isScrollingByClick.current = true;
      const navHeight = navRef.current.getBoundingClientRect().height;
      const top = section.getBoundingClientRect().top + window.scrollY - navHeight;
      window.scrollTo({ top, behavior: 'smooth' });
      setTimeout(() => { isScrollingByClick.current = false; }, 800);
    }
  }, []);

  /* scroll-sync: update active tab via IntersectionObserver */
  useEffect(() => {
    if (categories.length === 0) return;

    const navHeight = navRef.current?.getBoundingClientRect().height ?? 0;
    const visibleSections = new Set<number>();

    const elementToIndex = new Map<Element, number>();
    sectionRefs.current.forEach((el, index) => {
      elementToIndex.set(el, index);
    });

    const observer = new IntersectionObserver(
      (entries) => {
        if (isScrollingByClick.current) return;

        for (const entry of entries) {
          const index = elementToIndex.get(entry.target);
          if (index === undefined) continue;

          if (entry.isIntersecting) {
            visibleSections.add(index);
          } else {
            visibleSections.delete(index);
          }
        }

        if (visibleSections.size > 0) {
          setActiveTab(Math.min(...visibleSections));
        }
      },
      {
        rootMargin: `-${navHeight}px 0px -50% 0px`,
        threshold: 0,
      },
    );

    sectionRefs.current.forEach((el) => {
      observer.observe(el);
    });

    return () => observer.disconnect();
  }, [categories.length]);

  /* scroll active tab button into view — manual scrollLeft to avoid interrupting page scroll */
  useEffect(() => {
    const container = navScrollRef.current;
    if (!container) return;
    const btn = container.children[activeTab] as HTMLElement | undefined;
    if (!btn) return;
    const left = btn.offsetLeft - container.offsetWidth / 2 + btn.offsetWidth / 2;
    container.scrollTo({ left, behavior: 'smooth' });
  }, [activeTab]);

  /* nav scroll hint: detect end of scroll */
  useEffect(() => {
    const scrollEl = navScrollRef.current;
    const navEl = navRef.current;
    if (!scrollEl || !navEl) return;

    function checkScroll() {
      if (!scrollEl || !navEl) return;
      const atEnd = scrollEl.scrollLeft >= scrollEl.scrollWidth - scrollEl.clientWidth - 4;
      navEl.classList.toggle('pm-nav--scrolled-end', atEnd);
    }

    checkScroll();
    scrollEl.addEventListener('scroll', checkScroll, { passive: true });
    return () => scrollEl.removeEventListener('scroll', checkScroll);
  }, [categories.length]);

  if (menuLoading || contentLoading) {
    return <MenuSkeleton />;
  }

  if (menuError) {
    return (
      <div className="pm pm-error">
        <p className="pm-error-text">Не удалось загрузить меню</p>
      </div>
    );
  }

  const about = content?.about;
  const contacts = content?.contacts;

  return (
    <MotionConfig reducedMotion="user">
    <div className="pm">
      {/* Hero */}
      <header className={`pm-hero${about?.photoPath ? ' pm-hero--with-image' : ''}`}>
        <motion.div
          className="pm-hero-inner"
          initial="hidden"
          animate="visible"
          variants={staggerChildren}
        >
          <motion.div className="pm-hero-ornament" variants={fadeUp}>
            <svg width="40" height="40" viewBox="0 0 40 40" fill="none">
              <path d="M20 2L26 14H38L28 22L32 38L20 28L8 38L12 22L2 14H14L20 2Z" stroke="currentColor" strokeWidth="1" fill="currentColor" fillOpacity="0.15" />
              <circle cx="20" cy="20" r="4" stroke="currentColor" strokeWidth="1" fill="currentColor" fillOpacity="0.3" />
            </svg>
          </motion.div>
          <motion.h1 className="pm-hero-title" variants={fadeUp}>
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
              </button>
            ))}
          </div>
        </nav>
      )}

      {/* Menu sections */}
      <main className="pm-content pm-container">
        {categories.map((category, index) => (
          <MenuSection
            key={category.id}
            category={category}
            onDishClick={setSelectedDish}
            ref={(el: HTMLDivElement | null) => {
              if (el) sectionRefs.current.set(index, el);
              else sectionRefs.current.delete(index);
            }}
          />
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

      {/* Contacts */}
      {contacts && (contacts.address || contacts.phone || contacts.hours) && (
        <ContactsFooter contacts={contacts} />
      )}

      {/* Dish Modal */}
      <AnimatePresence>
        {selectedDish && (
          <DishModal dish={selectedDish} onClose={() => setSelectedDish(null)} />
        )}
      </AnimatePresence>
    </div>
    </MotionConfig>
  );
}

/* ── Menu Section ── */

interface MenuSectionProps {
  category: PublicMenuCategory;
  onDishClick: (dish: PublicMenuDish) => void;
  ref?: React.Ref<HTMLDivElement>;
}

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
      <motion.h2 className="pm-section-header" variants={fadeUp}>
        {category.name}
      </motion.h2>
      <motion.hr className="pm-section-line" variants={fadeUp} />

      <div className="pm-section-dishes">
        {category.dishes.map((dish) => (
          <motion.div key={dish.id} variants={dishFade} onClick={() => onDishClick(dish)} style={{ cursor: 'pointer' }}>
            {dish.photoPath ? (
              <DishCard dish={dish} />
            ) : (
              <DishListItem dish={dish} />
            )}
          </motion.div>
        ))}
      </div>
    </motion.section>
  );
}

/* ── Dish Card (with photo) ── */

function DishCard({ dish }: { dish: PublicMenuDish }) {
  return (
    <div className="pm-dish-card">
      <img
        className="pm-dish-card-photo"
        src={imageUrl(dish.photoPath)!}
        alt={dish.name}
        loading="lazy"
      />
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

/* ── Dish List Item (no photo, classic dotted line) ── */

function DishListItem({ dish }: { dish: PublicMenuDish }) {
  return (
    <div className="pm-dish-item">
      <div className="pm-dish-item-placeholder">
        <svg viewBox="0 0 64 64" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
          <ellipse cx="32" cy="38" rx="22" ry="10" />
          <path d="M10 38c0-12 9.8-22 22-22s22 10 22 22" />
          <path d="M32 16v-4M28 13l4-4 4 4" />
        </svg>
      </div>
      <div className="pm-dish-item-body">
        <div className="pm-dish-item-name">{dish.name}</div>
        {dish.description && (
          <div className="pm-dish-item-desc">{dish.description}</div>
        )}
        <div className="pm-dish-item-price">{formatPrice(dish.price)} сум</div>
      </div>
    </div>
  );
}

/* ── Dish Modal ── */

function DishModal({ dish, onClose }: { dish: PublicMenuDish; onClose: () => void }) {
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
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      exit={{ opacity: 0 }}
      transition={{ duration: 0.2 }}
      onClick={onClose}
    >
      <motion.div
        className="pm-modal"
        initial={{ opacity: 0, scale: 0.95, y: 16 }}
        animate={{ opacity: 1, scale: 1, y: 0 }}
        exit={{ opacity: 0, scale: 0.95, y: 16 }}
        transition={{ duration: 0.25, ease: 'easeOut' }}
        onClick={(e) => e.stopPropagation()}
        style={{ position: 'relative' }}
      >
        <button className="pm-modal-close" onClick={onClose} type="button" aria-label="Закрыть">
          &times;
        </button>

        {dish.photoPath ? (
          <img className="pm-modal-photo" src={imageUrl(dish.photoPath)!} alt={dish.name} />
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

/* ── Skeleton Loading ── */

function MenuSkeleton() {
  return (
    <div className="pm pm-skeleton">
      <div className="pm-skeleton-hero">
        <div className="pm-skeleton-bone" style={{ width: 40, height: 40, borderRadius: '50%' }} />
        <div className="pm-skeleton-bone" style={{ width: 200, height: 32 }} />
        <div className="pm-skeleton-bone" style={{ width: 56, height: 2 }} />
        <div className="pm-skeleton-bone" style={{ width: 280, height: 14 }} />
        <div className="pm-skeleton-bone" style={{ width: 220, height: 14 }} />
      </div>
      <div className="pm-skeleton-nav">
        {[80, 96, 72, 88].map((w, i) => (
          <div key={i} className="pm-skeleton-bone--light pm-skeleton-tab" style={{ width: w }} />
        ))}
      </div>
      <div className="pm-skeleton-grid">
        {Array.from({ length: 6 }).map((_, i) => (
          <div key={i} className="pm-skeleton-card">
            <div className="pm-skeleton-bone--light pm-skeleton-card-img" />
            <div className="pm-skeleton-card-body">
              <div className="pm-skeleton-bone--light" style={{ width: '70%', height: 16 }} />
              <div className="pm-skeleton-bone--light" style={{ width: '90%', height: 12 }} />
              <div className="pm-skeleton-bone--light" style={{ width: 80, height: 16, marginTop: 'auto' }} />
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

/* ── Contacts Footer ── */

function ContactsFooter({ contacts }: { contacts: PublicContacts }) {
  return (
    <footer className="pm-footer pm-container">
      <hr className="pm-footer-divider" />

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
  );
}
