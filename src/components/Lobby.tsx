import { useState } from 'react';
import { api } from '../api';

interface Props {
  onGameJoined: (gameId: string, playerId: string) => void;
  defaultJoinCode?: string;
}

type Tab = 'create' | 'join';

export function Lobby({ onGameJoined, defaultJoinCode }: Props) {
  const [tab, setTab] = useState<Tab>(defaultJoinCode ? 'join' : 'create');

  // create form
  const [hostName, setHostName] = useState('');
  const [creating, setCreating] = useState(false);
  const [createError, setCreateError] = useState('');

  // join form
  const [joinCode, setJoinCode] = useState(defaultJoinCode ?? '');
  const [playerName, setPlayerName] = useState('');
  const [joining, setJoining] = useState(false);
  const [joinError, setJoinError] = useState('');

  async function handleCreate(e: React.FormEvent) {
    e.preventDefault();
    const name = hostName.trim();
    if (!name) { setCreateError("Введіть ваше ім'я"); return; }
    setCreating(true);
    setCreateError('');
    try {
      const res = await api.createGame(name);
      onGameJoined(res.gameId, res.playerId);
    } catch (err) {
      setCreateError(err instanceof Error ? err.message : 'Помилка');
    } finally {
      setCreating(false);
    }
  }

  async function handleJoin(e: React.FormEvent) {
    e.preventDefault();
    const code = joinCode.trim().toUpperCase();
    const name = playerName.trim();
    if (!code) { setJoinError('Введіть код гри'); return; }
    if (!name) { setJoinError("Введіть ваше ім'я"); return; }
    setJoining(true);
    setJoinError('');
    try {
      const res = await api.joinGame(code, name);
      onGameJoined(res.gameId, res.playerId);
    } catch (err) {
      setJoinError(err instanceof Error ? err.message : 'Помилка');
    } finally {
      setJoining(false);
    }
  }

  return (
    <div style={styles.page}>
      <div style={styles.card}>
        {/* ── Logo / Title ──────────────────────────── */}
        <div style={styles.logo}>
          <div style={styles.logoIcon}>☢</div>
          <h1 style={styles.title}>БУНКЕР</h1>
          <p style={styles.subtitle}>Дискусійна карткова гра</p>
        </div>

        {/* ── Tabs ──────────────────────────────────── */}
        <div style={styles.tabs}>
          <button
            style={{ ...styles.tab, ...(tab === 'create' ? styles.tabActive : {}) }}
            onClick={() => setTab('create')}
          >
            Створити гру
          </button>
          <button
            style={{ ...styles.tab, ...(tab === 'join' ? styles.tabActive : {}) }}
            onClick={() => setTab('join')}
          >
            Приєднатись
          </button>
        </div>

        {/* ── Create form ───────────────────────────── */}
        {tab === 'create' && (
          <form onSubmit={handleCreate} style={styles.form}>
            <div className="field-group">
              <label className="field-label">Ваше ім&apos;я</label>
              <input
                className="field-input"
                type="text"
                placeholder="Як вас звати?"
                maxLength={24}
                value={hostName}
                onChange={e => setHostName(e.target.value)}
                autoFocus
              />
            </div>
            {createError && <div className="alert alert-error">{createError}</div>}
            <button
              className="btn btn-primary btn-full btn-lg"
              type="submit"
              disabled={creating}
              style={{ marginTop: '1.25rem' }}
            >
              {creating ? 'Створення…' : 'Створити гру'}
            </button>
            <p className="hint" style={{ textAlign: 'center', marginTop: '0.75rem' }}>
              Ви станете ведучим гри та отримаєте код для інших гравців
            </p>
          </form>
        )}

        {/* ── Join form ─────────────────────────────── */}
        {tab === 'join' && (
          <form onSubmit={handleJoin} style={styles.form}>
            <div className="field-group">
              <label className="field-label">Код гри</label>
              <input
                className="field-input"
                type="text"
                placeholder="Наприклад: AB12"
                maxLength={8}
                value={joinCode}
                onChange={e => setJoinCode(e.target.value.toUpperCase())}
                autoFocus
                style={{ letterSpacing: '3px', fontWeight: 700, fontSize: '1.2rem' }}
              />
            </div>
            <div className="field-group">
              <label className="field-label">Ваше ім&apos;я</label>
              <input
                className="field-input"
                type="text"
                placeholder="Як вас звати?"
                maxLength={24}
                value={playerName}
                onChange={e => setPlayerName(e.target.value)}
              />
            </div>
            {joinError && <div className="alert alert-error">{joinError}</div>}
            <button
              className="btn btn-primary btn-full btn-lg"
              type="submit"
              disabled={joining}
              style={{ marginTop: '1.25rem' }}
            >
              {joining ? 'Підключення…' : 'Приєднатись'}
            </button>
          </form>
        )}

        {/* ── About ─────────────────────────────────── */}
        <div style={styles.about}>
          <span style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>
            4–16 гравців · Базовий режим · Порятунок
          </span>
        </div>
      </div>
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  page: {
    minHeight: '100vh',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    padding: '1rem',
    background: 'radial-gradient(ellipse at 50% 0%, rgba(245,197,24,.06) 0%, transparent 70%), var(--bg)',
  },
  card: {
    background: 'var(--bg2)',
    border: '1px solid var(--border)',
    borderRadius: 'var(--radius-lg)',
    boxShadow: 'var(--shadow)',
    maxWidth: 420,
    width: '100%',
    padding: '2rem',
  },
  logo: {
    textAlign: 'center',
    marginBottom: '2rem',
  },
  logoIcon: {
    fontSize: '3rem',
    lineHeight: 1,
    marginBottom: '0.5rem',
  },
  title: {
    fontSize: '2.8rem',
    fontWeight: 700,
    color: 'var(--gold)',
    letterSpacing: '6px',
    lineHeight: 1,
  },
  subtitle: {
    color: 'var(--text-muted)',
    fontSize: '0.8rem',
    letterSpacing: '2px',
    textTransform: 'uppercase',
    marginTop: '0.4rem',
  },
  tabs: {
    display: 'flex',
    borderBottom: '1px solid var(--border)',
    marginBottom: '1.5rem',
  },
  tab: {
    flex: 1,
    background: 'none',
    border: 'none',
    color: 'var(--text-muted)',
    cursor: 'pointer',
    fontFamily: "'Oswald', sans-serif",
    fontSize: '0.85rem',
    fontWeight: 600,
    letterSpacing: '1.5px',
    padding: '0.6rem 0',
    textTransform: 'uppercase',
    transition: 'color 0.15s',
  },
  tabActive: {
    color: 'var(--gold)',
    borderBottom: '2px solid var(--gold)',
    marginBottom: '-1px',
  },
  form: {
    display: 'flex',
    flexDirection: 'column',
  },
  about: {
    textAlign: 'center',
    marginTop: '2rem',
    paddingTop: '1.25rem',
    borderTop: '1px solid var(--border)',
  },
};
