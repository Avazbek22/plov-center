import { useEffect, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { motion, MotionConfig } from 'motion/react';
import { usePublicContent } from '@/hooks/use-public-menu';
import { imageUrl } from '@/utils/image-url';
import type { PublicContacts } from '@/types/public';
import './home.css';

const fadeUp = {
  hidden: { opacity: 0, y: 18 },
  visible: { opacity: 1, y: 0, transition: { duration: 0.55, ease: [0.22, 1, 0.36, 1] as const } },
};

const stagger = {
  hidden: {},
  visible: { transition: { staggerChildren: 0.1, delayChildren: 0.1 } },
};

function useScrollY() {
  const [y, setY] = useState(0);
  useEffect(() => {
    let ticking = false;
    function onScroll() {
      if (!ticking) {
        requestAnimationFrame(() => {
          setY(window.scrollY);
          ticking = false;
        });
        ticking = true;
      }
    }
    window.addEventListener('scroll', onScroll, { passive: true });
    return () => window.removeEventListener('scroll', onScroll);
  }, []);
  return y;
}

export default function Home() {
  const { data: content } = usePublicContent();
  const scrollY = useScrollY();

  const about = content?.about;
  const contacts = content?.contacts;
  const aboutPhoto = imageUrl(about?.photoPath ?? null) ?? '/hero-bg.jpg';

  return (
    <MotionConfig reducedMotion="user">
      <div className="pc-home">
        <Hero scrollY={scrollY} contacts={contacts} />
        <AboutSection text={about?.text ?? null} photo={aboutPhoto} />
        <FeaturesSection />
        <MenuCtaSection />
        <ContactsPreview contacts={contacts} />
      </div>
    </MotionConfig>
  );
}

/* ── Hero ── */

function Hero({ scrollY, contacts }: { scrollY: number; contacts: PublicContacts | undefined }) {
  return (
    <section className="ph-hero">
      <motion.div
        className="ph-hero-bg"
        style={{ y: scrollY * 0.25 }}
        aria-hidden
      />
      <div className="pc-container">
        <motion.div
          className="ph-hero-inner"
          initial="hidden"
          animate="visible"
          variants={stagger}
        >
          <motion.div className="pc-eyebrow ph-hero-eyebrow" variants={fadeUp}>
            Узбекская кухня
          </motion.div>
          <motion.h1 className="ph-hero-title" variants={fadeUp}>
            Плов Центр
          </motion.h1>
          <motion.p className="ph-hero-tagline" variants={fadeUp}>
            Готовим настоящий узбекский плов и блюда восточной кухни. Тёплый зал, чай в чайнике, неспешный вечер.
          </motion.p>
          <motion.div className="ph-hero-ctas" variants={fadeUp}>
            <RouterLink to="/menu" className="pc-btn pc-btn--primary">
              Открыть меню
              <ArrowIcon />
            </RouterLink>
            <RouterLink to="/contacts" className="pc-btn pc-btn--ghost">
              Как нас найти
            </RouterLink>
          </motion.div>
          {(contacts?.address || contacts?.hours || contacts?.phone) && (
            <motion.div className="ph-hero-meta" variants={fadeUp}>
              {contacts?.address && (
                <span className="ph-hero-meta-item">
                  <PinIcon />
                  <span>{contacts.address}</span>
                </span>
              )}
              {contacts?.hours && (
                <span className="ph-hero-meta-item">
                  <ClockIcon />
                  <span>{contacts.hours}</span>
                </span>
              )}
              {contacts?.phone && (
                <span className="ph-hero-meta-item">
                  <PhoneIcon />
                  <a href={`tel:${contacts.phone.replace(/\s/g, '')}`} style={{ color: 'var(--pc-text)' }}>
                    <strong>{contacts.phone}</strong>
                  </a>
                </span>
              )}
            </motion.div>
          )}
        </motion.div>
      </div>
    </section>
  );
}

/* ── About ── */

function AboutSection({ text, photo }: { text: string | null; photo: string }) {
  const paragraphs = (text ?? defaultAboutText)
    .split(/\n{2,}/)
    .map((p) => p.trim())
    .filter(Boolean);

  return (
    <section className="ph-about pc-section">
      <div className="pc-container">
        <motion.div
          className="ph-about-grid"
          initial="hidden"
          whileInView="visible"
          viewport={{ once: true, margin: '-80px' }}
          variants={stagger}
        >
          <motion.div className="ph-about-text" variants={fadeUp}>
            <span className="pc-eyebrow">О нас</span>
            <h2 className="pc-h-section">Дом восточного гостеприимства</h2>
            <hr className="pc-divider-rule" />
            <div className="pc-prose">
              {paragraphs.map((p, i) => (
                <p key={i}>{p}</p>
              ))}
            </div>
          </motion.div>
          <motion.div className="ph-about-photo" variants={fadeUp}>
            <img src={photo} alt="Атмосфера Плов Центра" loading="lazy" />
          </motion.div>
        </motion.div>
      </div>
    </section>
  );
}

const defaultAboutText = `Плов Центр — место, куда приходят за тёплой узбекской кухней и неспешным застольем. Мы готовим плов на дровах, по семейному рецепту, без полуфабрикатов.

В зале — большие столы для компаний, ароматный чай в чайнике и атмосфера дастархана: где еда — это повод собраться вместе.`;

/* ── Features ── */

function FeaturesSection() {
  return (
    <section className="ph-features pc-section">
      <div className="pc-container">
        <motion.div
          className="ph-features-head"
          initial="hidden"
          whileInView="visible"
          viewport={{ once: true, margin: '-80px' }}
          variants={stagger}
        >
          <motion.span className="pc-eyebrow" variants={fadeUp}>Что у нас особенного</motion.span>
          <motion.h2 className="pc-h-section" variants={fadeUp}>Готовим как для своих</motion.h2>
        </motion.div>

        <motion.div
          className="ph-features-grid"
          initial="hidden"
          whileInView="visible"
          viewport={{ once: true, margin: '-60px' }}
          variants={stagger}
        >
          {features.map((f) => (
            <motion.article key={f.title} className="ph-feature" variants={fadeUp}>
              <span className="ph-feature-icon">{f.icon}</span>
              <h3 className="ph-feature-title">{f.title}</h3>
              <p className="ph-feature-text">{f.text}</p>
            </motion.article>
          ))}
        </motion.div>
      </div>
    </section>
  );
}

const features = [
  {
    title: 'Плов на дровах',
    text: 'Готовим в казане на живом огне — так, как делали наши деды. Ни одного дня без свежей партии.',
    icon: <FlameIcon />,
  },
  {
    title: 'Свежие продукты',
    text: 'Мясо и овощи — каждый день с рынка. Никаких заморозок, никаких полуфабрикатов.',
    icon: <LeafIcon />,
  },
  {
    title: 'Тёплая атмосфера',
    text: 'Уютный зал, чайник зелёного чая и место для большой компании. Здесь не торопят.',
    icon: <TeapotIcon />,
  },
];

/* ── Menu CTA ── */

function MenuCtaSection() {
  return (
    <section className="ph-menu-cta pc-section">
      <div className="ph-menu-cta-bg" aria-hidden />
      <div className="pc-container">
        <motion.div
          className="ph-menu-cta-inner"
          initial="hidden"
          whileInView="visible"
          viewport={{ once: true, margin: '-60px' }}
          variants={stagger}
        >
          <motion.span className="pc-eyebrow" variants={fadeUp}>Наше меню</motion.span>
          <motion.h2 className="pc-h-section" variants={fadeUp}>
            От плова и шашлыка до домашних супов
          </motion.h2>
          <motion.p className="pc-lead" variants={fadeUp} style={{ textAlign: 'center', margin: '0 auto' }}>
            Загляните в меню — категории, фотографии блюд, актуальные цены. Удобно листать с телефона.
          </motion.p>
          <motion.div variants={fadeUp}>
            <RouterLink to="/menu" className="pc-btn pc-btn--primary">
              Посмотреть меню
              <ArrowIcon />
            </RouterLink>
          </motion.div>
        </motion.div>
      </div>
    </section>
  );
}

/* ── Contacts preview ── */

function ContactsPreview({ contacts }: { contacts: PublicContacts | undefined }) {
  if (!contacts || (!contacts.address && !contacts.phone && !contacts.hours && !contacts.mapEmbed)) {
    return null;
  }

  return (
    <section className="ph-contacts pc-section">
      <div className="pc-container">
        <motion.div
          className="ph-contacts-grid"
          initial="hidden"
          whileInView="visible"
          viewport={{ once: true, margin: '-80px' }}
          variants={stagger}
        >
          <motion.div className="ph-contacts-head" variants={fadeUp}>
            <span className="pc-eyebrow">Контакты</span>
            <h2 className="pc-h-section">Заходите в гости</h2>
            <p className="pc-lead">
              Будем рады встретить вас лично. Ниже — адрес, часы работы и телефон для брони стола.
            </p>

            <div className="ph-contacts-list">
              {contacts.address && (
                <div className="ph-contacts-row">
                  <PinIcon />
                  <div className="ph-contacts-row-text">
                    <span className="ph-contacts-row-label">Адрес</span>
                    <span className="ph-contacts-row-value">{contacts.address}</span>
                  </div>
                </div>
              )}
              {contacts.phone && (
                <div className="ph-contacts-row">
                  <PhoneIcon />
                  <div className="ph-contacts-row-text">
                    <span className="ph-contacts-row-label">Телефон</span>
                    <span className="ph-contacts-row-value">
                      <a href={`tel:${contacts.phone.replace(/\s/g, '')}`}>{contacts.phone}</a>
                    </span>
                  </div>
                </div>
              )}
              {contacts.hours && (
                <div className="ph-contacts-row">
                  <ClockIcon />
                  <div className="ph-contacts-row-text">
                    <span className="ph-contacts-row-label">Часы работы</span>
                    <span className="ph-contacts-row-value">{contacts.hours}</span>
                  </div>
                </div>
              )}
            </div>

            <div className="ph-contacts-cta">
              <RouterLink to="/contacts" className="pc-btn pc-btn--ghost pc-btn--small">
                Все контакты
                <ArrowIcon />
              </RouterLink>
            </div>
          </motion.div>

          <motion.div className="ph-contacts-map" variants={fadeUp}>
            {contacts.mapEmbed ? (
              <div dangerouslySetInnerHTML={{ __html: contacts.mapEmbed }} />
            ) : (
              <div className="ph-contacts-map-fallback">
                Карта пока не добавлена. Свяжитесь с нами, и мы подскажем дорогу.
              </div>
            )}
          </motion.div>
        </motion.div>
      </div>
    </section>
  );
}

/* ── Icons ── */

function ArrowIcon() {
  return (
    <svg className="pc-btn-arrow" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
      <path d="M5 12h14" />
      <path d="M13 5l7 7-7 7" />
    </svg>
  );
}

function PinIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
      <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z" />
      <circle cx="12" cy="10" r="3" />
    </svg>
  );
}

function PhoneIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
      <path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z" />
    </svg>
  );
}

function ClockIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
      <circle cx="12" cy="12" r="10" />
      <polyline points="12 6 12 12 16 14" />
    </svg>
  );
}

function FlameIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
      <path d="M8.5 14.5A2.5 2.5 0 0 0 11 12c0-1.38-.5-2-1-3-1.072-2.143-.224-4.054 2-6 .5 2.5 2 4.9 4 6.5 2 1.6 3 3.5 3 5.5a7 7 0 1 1-14 0c0-1.153.433-2.294 1-3a2.5 2.5 0 0 0 2.5 2.5z" />
    </svg>
  );
}

function LeafIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
      <path d="M11 20A7 7 0 0 1 9.8 6.1C15.5 5 17 4.48 19 2c1 2 2 4.18 2 8 0 5.5-4.78 10-10 10z" />
      <path d="M2 21c0-3 1.85-5.36 5.08-6" />
    </svg>
  );
}

function TeapotIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
      <path d="M4 11h12a4 4 0 0 1 0 8H8a4 4 0 0 1-4-4v-4z" />
      <path d="M16 13h2a3 3 0 0 1 0 6" />
      <path d="M9 7c0-1.5 1-2.5 1-4" />
      <path d="M13 7c0-1.5 1-2.5 1-4" />
    </svg>
  );
}
