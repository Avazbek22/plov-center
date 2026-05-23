import { useEffect, useState, type ReactNode } from 'react';
import { AnimatePresence, motion } from 'motion/react';
import './splash.css';

const STORAGE_KEY = 'pc-splash-shown-v1';
const SPLASH_DURATION = 1700;

export default function SplashGate({ children }: { children: ReactNode }) {
  const [show, setShow] = useState(() => {
    try {
      return sessionStorage.getItem(STORAGE_KEY) !== '1';
    } catch {
      return true;
    }
  });

  useEffect(() => {
    if (!show) return;
    const timer = window.setTimeout(() => {
      setShow(false);
      try {
        sessionStorage.setItem(STORAGE_KEY, '1');
      } catch {
        /* ignore storage errors */
      }
    }, SPLASH_DURATION);
    return () => window.clearTimeout(timer);
  }, [show]);

  return (
    <>
      <AnimatePresence>{show && <Splash />}</AnimatePresence>
      {children}
    </>
  );
}

function Splash() {
  return (
    <motion.div
      className="pc-splash"
      exit={{ opacity: 0, transition: { duration: 0.45, ease: 'easeOut' } }}
      role="status"
      aria-label="Загрузка"
    >
      <motion.div
        className="pc-splash-logo"
        initial={{ opacity: 0, y: 8, scale: 0.96 }}
        animate={{ opacity: 1, y: 0, scale: 1 }}
        transition={{ duration: 0.55, ease: [0.22, 1, 0.36, 1] }}
      >
        <div className="pc-splash-mark" aria-hidden>П</div>
        <div className="pc-splash-brand">Плов Центр</div>
      </motion.div>

      <motion.div
        className="pc-splash-pill"
        initial={{ opacity: 0, y: 8 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ delay: 0.25, duration: 0.4, ease: [0.22, 1, 0.36, 1] }}
      >
        Developed by Avazbek Olimov
      </motion.div>

      <motion.div
        className="pc-splash-loader"
        initial={{ opacity: 0 }}
        animate={{ opacity: 1 }}
        transition={{ delay: 0.4, duration: 0.4 }}
      >
        <div className="pc-splash-progress" aria-hidden>
          <div className="pc-splash-progress-fill" />
        </div>
        <div className="pc-splash-percent">LOADING</div>
      </motion.div>
    </motion.div>
  );
}
