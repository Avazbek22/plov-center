import { motion, MotionConfig } from 'motion/react';
import { usePublicContent } from '@/hooks/use-public-menu';
import './contacts.css';

const fadeUp = {
  hidden: { opacity: 0, y: 18 },
  visible: { opacity: 1, y: 0, transition: { duration: 0.55, ease: [0.22, 1, 0.36, 1] as const } },
};

const stagger = {
  hidden: {},
  visible: { transition: { staggerChildren: 0.08, delayChildren: 0.05 } },
};

export default function Contacts() {
  const { data: content, isLoading } = usePublicContent();
  const contacts = content?.contacts;

  const hasAny = Boolean(contacts && (contacts.address || contacts.phone || contacts.hours || contacts.mapEmbed));
  const telHref = contacts?.phone ? `tel:${contacts.phone.replace(/\s/g, '')}` : null;

  return (
    <MotionConfig reducedMotion="user">
      <div className="pcc-root">
        <section className="pcc-head">
          <div className="pcc-head-bg" aria-hidden />
          <div className="pc-container">
            <motion.div
              className="pcc-head-inner"
              initial="hidden"
              animate="visible"
              variants={stagger}
            >
              <motion.span className="pc-eyebrow" variants={fadeUp}>Контакты</motion.span>
              <motion.h1 className="pc-h-display" variants={fadeUp} style={{ fontSize: 'clamp(2.2rem, 6vw, 3.4rem)' }}>
                Заходите в гости
              </motion.h1>
              <motion.p className="pc-lead" variants={fadeUp}>
                Будем рады встретить вас лично. Ниже — адрес, часы работы и быстрая связь.
              </motion.p>
            </motion.div>
          </div>
        </section>

        <section className="pcc-body">
          <div className="pc-container">
            {isLoading ? (
              <div className="pcc-empty">Загружаем контакты…</div>
            ) : !hasAny ? (
              <div className="pcc-empty">
                Информация о контактах пока не добавлена. Загляните позже или напишите нам.
              </div>
            ) : (
              <motion.div
                className="pcc-grid"
                initial="hidden"
                animate="visible"
                variants={stagger}
              >
                <motion.div className="pcc-card" variants={fadeUp}>
                  {contacts?.address && (
                    <div className="pcc-row">
                      <span className="pcc-row-icon"><PinIcon /></span>
                      <div className="pcc-row-body">
                        <span className="pcc-row-label">Адрес</span>
                        <span className="pcc-row-value">{contacts.address}</span>
                      </div>
                    </div>
                  )}

                  {contacts?.phone && telHref && (
                    <div className="pcc-row">
                      <span className="pcc-row-icon"><PhoneIcon /></span>
                      <div className="pcc-row-body">
                        <span className="pcc-row-label">Телефон</span>
                        <span className="pcc-row-value">
                          <a href={telHref}>{contacts.phone}</a>
                        </span>
                      </div>
                    </div>
                  )}

                  {contacts?.hours && (
                    <div className="pcc-row">
                      <span className="pcc-row-icon"><ClockIcon /></span>
                      <div className="pcc-row-body">
                        <span className="pcc-row-label">Часы работы</span>
                        <span className="pcc-row-value">{contacts.hours}</span>
                      </div>
                    </div>
                  )}

                  <div className="pcc-actions">
                    {telHref && (
                      <a href={telHref} className="pc-btn pc-btn--primary pc-btn--small">
                        <PhoneSolid /> Позвонить
                      </a>
                    )}
                    {contacts?.address && (
                      <a
                        href={`https://yandex.ru/maps/?text=${encodeURIComponent(contacts.address)}`}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="pc-btn pc-btn--ghost pc-btn--small"
                      >
                        <RouteIcon /> Проложить маршрут
                      </a>
                    )}
                  </div>
                </motion.div>

                <motion.div className="pcc-map-wrap" variants={fadeUp}>
                  {contacts?.mapEmbed ? (
                    <div dangerouslySetInnerHTML={{ __html: contacts.mapEmbed }} />
                  ) : (
                    <div className="pcc-map-fallback">
                      Карта пока не добавлена. Воспользуйтесь кнопкой «Проложить маршрут».
                    </div>
                  )}
                </motion.div>
              </motion.div>
            )}
          </div>
        </section>
      </div>
    </MotionConfig>
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

function PhoneSolid() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor" aria-hidden>
      <path d="M20 15.5c-1.25 0-2.45-.2-3.57-.57a1 1 0 0 0-1.02.24l-2.2 2.2a15.05 15.05 0 0 1-6.6-6.6l2.2-2.2a1 1 0 0 0 .25-1.02A11.36 11.36 0 0 1 8.5 4a1 1 0 0 0-1-1H4a1 1 0 0 0-1 1c0 9.39 7.61 17 17 17a1 1 0 0 0 1-1v-3.5a1 1 0 0 0-1-1z" />
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

function RouteIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
      <circle cx="6" cy="19" r="3" />
      <path d="M9 19h8.5a3.5 3.5 0 0 0 0-7h-11a3.5 3.5 0 0 1 0-7H15" />
      <circle cx="18" cy="5" r="3" />
    </svg>
  );
}
