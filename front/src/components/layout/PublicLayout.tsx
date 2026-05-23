import { useEffect, useState } from 'react';
import { Link as RouterLink, NavLink, Outlet, useLocation } from 'react-router-dom';
import './public.css';

export default function PublicLayout() {
  const location = useLocation();
  const [scrolled, setScrolled] = useState(false);
  const [mobileOpen, setMobileOpen] = useState(false);
  const [lastPath, setLastPath] = useState(location.pathname);

  if (lastPath !== location.pathname) {
    setLastPath(location.pathname);
    setMobileOpen(false);
  }

  useEffect(() => {
    window.scrollTo({ top: 0, left: 0, behavior: 'auto' });
  }, [location.pathname]);

  useEffect(() => {
    function onScroll() {
      setScrolled(window.scrollY > 12);
    }
    onScroll();
    window.addEventListener('scroll', onScroll, { passive: true });
    return () => window.removeEventListener('scroll', onScroll);
  }, []);

  useEffect(() => {
    document.body.style.overflow = mobileOpen ? 'hidden' : '';
    return () => {
      document.body.style.overflow = '';
    };
  }, [mobileOpen]);

  return (
    <div className="pc-root">
      <PublicHeader scrolled={scrolled} mobileOpen={mobileOpen} onToggleMobile={() => setMobileOpen((open) => !open)} onCloseMobile={() => setMobileOpen(false)} />
      <main className="pc-main">
        <Outlet />
      </main>
      <PublicFooter />
    </div>
  );
}

interface PublicHeaderProps {
  scrolled: boolean;
  mobileOpen: boolean;
  onToggleMobile: () => void;
  onCloseMobile: () => void;
}

function PublicHeader({ scrolled, mobileOpen, onToggleMobile, onCloseMobile }: PublicHeaderProps) {
  return (
    <header className={`pc-header${scrolled || mobileOpen ? ' pc-header--scrolled' : ''}`}>
      <div className="pc-header-inner">
        <RouterLink to="/" className="pc-logo" onClick={onCloseMobile}>
          <span className="pc-logo-mark" aria-hidden>П</span>
          <span className="pc-logo-text">Плов Центр</span>
        </RouterLink>

        <nav className="pc-nav-desktop" aria-label="Основная навигация">
          <NavLink to="/" end className={navLinkClass}>Главная</NavLink>
          <NavLink to="/menu" className={navLinkClass}>Меню</NavLink>
          <NavLink to="/contacts" className={navLinkClass}>Контакты</NavLink>
        </nav>

        <button
          type="button"
          className={`pc-burger${mobileOpen ? ' pc-burger--open' : ''}`}
          aria-label="Открыть навигацию"
          aria-expanded={mobileOpen}
          aria-controls="pc-mobile-nav"
          onClick={onToggleMobile}
        >
          <span />
          <span />
          <span />
        </button>
      </div>

      <div id="pc-mobile-nav" className={`pc-nav-mobile${mobileOpen ? ' pc-nav-mobile--open' : ''}`}>
        <NavLink to="/" end className={mobileNavLinkClass} onClick={onCloseMobile}>Главная</NavLink>
        <NavLink to="/menu" className={mobileNavLinkClass} onClick={onCloseMobile}>Меню</NavLink>
        <NavLink to="/contacts" className={mobileNavLinkClass} onClick={onCloseMobile}>Контакты</NavLink>
      </div>
    </header>
  );
}

function navLinkClass({ isActive }: { isActive: boolean }): string {
  return `pc-nav-link${isActive ? ' pc-nav-link--active' : ''}`;
}

function mobileNavLinkClass({ isActive }: { isActive: boolean }): string {
  return `pc-nav-mlink${isActive ? ' pc-nav-mlink--active' : ''}`;
}

function PublicFooter() {
  const year = new Date().getFullYear();
  return (
    <footer className="pc-footer">
      <div className="pc-footer-inner">
        <div className="pc-footer-brand">
          <div className="pc-footer-logo">Плов Центр</div>
          <p className="pc-footer-tag">
            Уютная узбекская кухня. Готовим плов на дровах каждый день, по семейному рецепту.
          </p>
        </div>
        <nav className="pc-footer-nav" aria-label="Подвал сайта">
          <RouterLink to="/">Главная</RouterLink>
          <RouterLink to="/menu">Меню</RouterLink>
          <RouterLink to="/contacts">Контакты</RouterLink>
          <RouterLink to="/privacy">Политика конфиденциальности</RouterLink>
          <RouterLink to="/terms">Условия использования</RouterLink>
          <RouterLink to="/developers">Разработчики</RouterLink>
        </nav>
        <div className="pc-footer-meta">© {year} Плов Центр. Все права защищены.</div>
      </div>
    </footer>
  );
}
