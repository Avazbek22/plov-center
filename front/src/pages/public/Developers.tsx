import { motion, MotionConfig } from 'motion/react';
import './developers.css';

const fadeUp = {
  hidden: { opacity: 0, y: 18 },
  visible: { opacity: 1, y: 0, transition: { duration: 0.55, ease: [0.22, 1, 0.36, 1] as const } },
};

const stagger = {
  hidden: {},
  visible: { transition: { staggerChildren: 0.1, delayChildren: 0.05 } },
};

const developers = [
  {
    name: 'Avazbek Olimov',
    initials: 'AO',
    role: 'Full-Stack .NET Developer, UI/UX Designer',
    description:
      'Разработка серверной логики на ASP.NET Core и клиентской части на Blazor. Ответственность за UI-дизайн, адаптивную вёрстку, пользовательский опыт, а также за SEO оптимизацию.',
    skills: ['C#', '.NET', 'Blazor', 'LINQ', 'UI/UX', 'SEO', 'Clean architecture', 'CQRS', 'Testing'],
    email: 'mailto:avazbek.olimov@gmail.com',
    github: 'https://github.com/Avazbek22',
  },
  {
    name: 'Firuz Isoboev',
    initials: 'FI',
    role: 'Backend Developer, Team Lead, Solution Architect',
    description:
      'Архитектурные решения, аутентификация, интеграция с базой данных и построение серверной логики. Ведущий команды и дизайнер архитектуры системы, а также DevOps.',
    skills: ['C#', 'Design-Patterns', 'C++', 'Backend', 'DevOps', 'Python', 'SignalR'],
    email: 'mailto:firuzisoboev0026@gmail.com',
    github: 'https://github.com/firuz-isoboev',
  },
];

const stats = [
  { icon: <PeopleIcon />, value: '2', label: 'Разработчика' },
  { icon: <CodeIcon />, value: '200+', label: 'Часов кода' },
  { icon: <CoffeeIcon />, value: '∞', label: 'Чашек кофе' },
  { icon: <HeartIcon />, value: '+1', label: 'Проект мечты' },
];

export default function Developers() {
  return (
    <MotionConfig reducedMotion="user">
      <div className="pcd-root">
        {/* Hero */}
        <section className="pcd-head">
          <div className="pcd-head-bg" aria-hidden />
          <div className="pc-container">
            <motion.div
              className="pcd-head-inner"
              initial="hidden"
              animate="visible"
              variants={stagger}
            >
              <motion.span className="pc-eyebrow" variants={fadeUp}>Команда</motion.span>
              <motion.h1 className="pc-h-display" variants={fadeUp}>
                Команда разработчиков
              </motion.h1>
              <motion.hr className="pc-divider-rule" variants={fadeUp} />
              <motion.p className="pc-lead" variants={fadeUp} style={{ textAlign: 'center' }}>
                Мы создали Плов Центр с нуля. Нашли ошибку или есть идея? — напишите нам!
              </motion.p>
            </motion.div>
          </div>
        </section>

        {/* Cards */}
        <section className="pc-container">
          <motion.div
            className="pcd-grid"
            initial="hidden"
            whileInView="visible"
            viewport={{ once: true, margin: '-60px' }}
            variants={stagger}
          >
            {developers.map((dev) => (
              <motion.article key={dev.name} className="pcd-card" variants={fadeUp}>
                <div className="pcd-avatar">
                  {dev.initials}
                  <span className="pcd-avatar-dot" />
                </div>
                <h2 className="pcd-name">{dev.name}</h2>
                <p className="pcd-role">{dev.role}</p>
                <p className="pcd-desc">{dev.description}</p>
                <div className="pcd-skills">
                  {dev.skills.map((s) => (
                    <span key={s} className="pcd-chip">{s}</span>
                  ))}
                </div>
                <div className="pcd-actions">
                  <a href={dev.email} className="pc-btn pc-btn--primary pc-btn--small">
                    <MailIcon /> Написать
                  </a>
                  <a href={dev.github} target="_blank" rel="noopener noreferrer" className="pc-btn pc-btn--ghost pc-btn--small">
                    <GithubIcon /> GitHub
                  </a>
                </div>
              </motion.article>
            ))}
          </motion.div>
        </section>

        {/* Contact card */}
        <section className="pc-container">
          <motion.div
            className="pcd-contact"
            initial="hidden"
            whileInView="visible"
            viewport={{ once: true, margin: '-60px' }}
            variants={stagger}
          >
            <motion.span className="pcd-contact-icon" variants={fadeUp}><MailIcon /></motion.span>
            <motion.h3 className="pcd-contact-title" variants={fadeUp}>Связаться с нами</motion.h3>
            <motion.p className="pcd-contact-subtitle" variants={fadeUp}>
              Есть вопросы или предложения? Мы всегда рады общению!
            </motion.p>
            <motion.div variants={fadeUp}>
              <a href="mailto:firuzisoboev0026@gmail.com" className="pc-btn pc-btn--primary pc-btn--small">
                <SendIcon /> Написать нам
              </a>
            </motion.div>
            <motion.p className="pcd-contact-note" variants={fadeUp}>Ответим в течение суток</motion.p>
          </motion.div>
        </section>

        {/* Stats */}
        <section className="pc-container">
          <motion.div
            className="pcd-stats"
            initial="hidden"
            whileInView="visible"
            viewport={{ once: true, margin: '-40px' }}
            variants={stagger}
          >
            {stats.map((s) => (
              <motion.div key={s.label} className="pcd-stat" variants={fadeUp}>
                <span className="pcd-stat-icon">{s.icon}</span>
                <span className="pcd-stat-value">{s.value}</span>
                <span className="pcd-stat-label">{s.label}</span>
              </motion.div>
            ))}
          </motion.div>
        </section>
      </div>
    </MotionConfig>
  );
}

/* ── Icons ── */

function MailIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
      <rect width="20" height="16" x="2" y="4" rx="2" />
      <path d="m22 7-8.97 5.7a1.94 1.94 0 0 1-2.06 0L2 7" />
    </svg>
  );
}

function GithubIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="currentColor" aria-hidden>
      <path d="M12 0C5.37 0 0 5.37 0 12c0 5.31 3.435 9.795 8.205 11.385.6.105.825-.255.825-.57 0-.285-.015-1.23-.015-2.235-3.015.555-3.795-.735-4.035-1.41-.135-.345-.72-1.41-1.23-1.695-.42-.225-1.02-.78-.015-.795.945-.015 1.62.87 1.845 1.23 1.08 1.815 2.805 1.305 3.495.99.105-.78.42-1.305.765-1.605-2.67-.3-5.46-1.335-5.46-5.925 0-1.305.465-2.385 1.23-3.225-.12-.3-.54-1.53.12-3.18 0 0 1.005-.315 3.3 1.23.96-.27 1.98-.405 3-.405s2.04.135 3 .405c2.295-1.56 3.3-1.23 3.3-1.23.66 1.65.24 2.88.12 3.18.765.84 1.23 1.905 1.23 3.225 0 4.605-2.805 5.625-5.475 5.925.435.375.81 1.095.81 2.22 0 1.605-.015 2.895-.015 3.3 0 .315.225.69.825.57A12.02 12.02 0 0 0 24 12c0-6.63-5.37-12-12-12z" />
    </svg>
  );
}

function SendIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
      <path d="m22 2-7 20-4-9-9-4z" />
      <path d="M22 2 11 13" />
    </svg>
  );
}

function PeopleIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
      <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" />
      <circle cx="9" cy="7" r="4" />
      <path d="M22 21v-2a4 4 0 0 0-3-3.87" />
      <path d="M16 3.13a4 4 0 0 1 0 7.75" />
    </svg>
  );
}

function CodeIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
      <polyline points="16 18 22 12 16 6" />
      <polyline points="8 6 2 12 8 18" />
    </svg>
  );
}

function CoffeeIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
      <path d="M10 2v2" />
      <path d="M14 2v2" />
      <path d="M16 8a1 1 0 0 1 1 1v8a4 4 0 0 1-4 4H7a4 4 0 0 1-4-4V9a1 1 0 0 1 1-1h14a4 4 0 1 1 0 8h-1" />
      <path d="M6 2v2" />
    </svg>
  );
}

function HeartIcon() {
  return (
    <svg viewBox="0 0 24 24" fill="currentColor" stroke="none" aria-hidden>
      <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z" />
    </svg>
  );
}
