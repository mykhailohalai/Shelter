import { useState } from 'react';
import { api } from '../api';
import type { GameState } from '../types';
import { copyToClipboard, joinUrl } from '../utils';

interface Props {
  state: GameState;
  gameId: string;
  refresh: () => Promise<void>;
  onLeave: () => void;
}

export function WaitingRoom({ state, gameId, refresh, onLeave }: Props) {
  const [starting, setStarting] = useState(false);
  const [error, setError] = useState('');
  const [copied, setCopied] = useState(false);

  const minPlayers = 4;
  const canStart = state.isHost && state.players.length >= minPlayers;

  async function handleStart() {
    setStarting(true);
    setError('');
    try {
      await api.startGame(gameId);
      await refresh();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Помилка');
    } finally {
      setStarting(false);
    }
  }

  function handleCopy(text: string) {
    copyToClipboard(text);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  }

  const url = joinUrl(state.code);

  return (
    <div style={styles.page}>
      <div style={styles.container}>
        {/* Header */}
        <div style={styles.header}>
          <div style={styles.headerLeft}>
            <span style={styles.logo}>☢ БУНКЕР</span>
          </div>
          <button className="btn btn-ghost btn-sm" onClick={onLeave}>
            Вийти
          </button>
        </div>

        {/* Code block */}
        <div style={styles.codeBlock}>
          <p style={styles.codeLabel}>Код гри</p>
          <p style={styles.codeValue}>{state.code}</p>
          <div style={{ display: 'flex', gap: '0.5rem', justifyContent: 'center', flexWrap: 'wrap' }}>
            <button
              className="btn btn-outline btn-sm"
              onClick={() => handleCopy(state.code)}
            >
              {copied ? '✓ Скопійовано' : 'Копіювати код'}
            </button>
            <button
              className="btn btn-outline btn-sm"
              onClick={() => handleCopy(url)}
            >
              Копіювати посилання
            </button>
          </div>
          <p style={styles.hint}>
            Поділіться кодом або посиланням з іншими гравцями
          </p>
        </div>

        {/* Player list */}
        <div style={styles.section}>
          <div style={styles.sectionHeader}>
            <h3>Гравці</h3>
            <span className="badge badge-gold">
              {state.players.length} / {state.players.length < 4 ? '4 мін.' : '16 макс.'}
            </span>
          </div>

          <div style={styles.playerList}>
            {state.players.map((p, i) => (
              <div key={p.id} style={styles.playerRow}>
                <span style={styles.playerNum}>{i + 1}</span>
                <span style={styles.playerName}>{p.name}</span>
                {p.id === state.myId && (
                  <span className="badge badge-info" style={{ fontSize: '0.6rem' }}>Ви</span>
                )}
                {p.id === state.myId && state.isHost && (
                  <span className="badge badge-gold" style={{ fontSize: '0.6rem', marginLeft: 4 }}>Ведучий</span>
                )}
              </div>
            ))}

            {state.players.length < minPlayers && (
              <div style={styles.waitingSlots}>
                {Array.from({ length: minPlayers - state.players.length }).map((_, i) => (
                  <div key={i} style={{ ...styles.playerRow, opacity: 0.3 }}>
                    <span style={styles.playerNum}>{state.players.length + i + 1}</span>
                    <span style={styles.playerName}>Очікування…</span>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Host controls */}
        {state.isHost && (
          <div style={styles.section}>
            {error && <div className="alert alert-error" style={{ marginBottom: '0.75rem' }}>{error}</div>}
            <button
              className="btn btn-primary btn-full btn-lg"
              onClick={handleStart}
              disabled={!canStart || starting}
            >
              {starting ? 'Запуск…' : 'Почати гру'}
            </button>
            {!canStart && (
              <p className="hint" style={{ textAlign: 'center', marginTop: '0.5rem' }}>
                Потрібно щонайменше {minPlayers} гравці
              </p>
            )}
          </div>
        )}

        {!state.isHost && (
          <div style={styles.waitingMsg}>
            <div className="spinner" style={{ width: 22, height: 22, borderWidth: 2 }} />
            <span>Очікування ведучого…</span>
          </div>
        )}
      </div>
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  page: {
    minHeight: '100vh',
    background: 'var(--bg)',
    display: 'flex',
    justifyContent: 'center',
    padding: '1rem',
  },
  container: {
    maxWidth: 520,
    width: '100%',
    paddingTop: '1rem',
  },
  header: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    marginBottom: '2rem',
  },
  headerLeft: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.5rem',
  },
  logo: {
    fontFamily: "'Oswald', sans-serif",
    fontSize: '1.3rem',
    fontWeight: 700,
    color: 'var(--gold)',
    letterSpacing: '3px',
  },
  codeBlock: {
    background: 'var(--bg2)',
    border: '1px solid var(--border)',
    borderRadius: 'var(--radius-lg)',
    padding: '1.5rem',
    textAlign: 'center',
    marginBottom: '1.5rem',
  },
  codeLabel: {
    color: 'var(--text-muted)',
    fontSize: '0.7rem',
    letterSpacing: '2px',
    textTransform: 'uppercase',
    marginBottom: '0.5rem',
  },
  codeValue: {
    fontFamily: "'Oswald', sans-serif",
    fontSize: '3.5rem',
    fontWeight: 700,
    color: 'var(--gold)',
    letterSpacing: '10px',
    marginBottom: '1rem',
  },
  hint: {
    fontSize: '0.75rem',
    color: 'var(--text-muted)',
    marginTop: '0.75rem',
  },
  section: {
    background: 'var(--bg2)',
    border: '1px solid var(--border)',
    borderRadius: 'var(--radius-lg)',
    padding: '1.25rem',
    marginBottom: '1rem',
  },
  sectionHeader: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    marginBottom: '1rem',
  },
  playerList: {
    display: 'flex',
    flexDirection: 'column',
    gap: '0.4rem',
  },
  waitingSlots: {},
  playerRow: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.75rem',
    padding: '0.5rem 0.75rem',
    background: 'var(--bg3)',
    borderRadius: 'var(--radius)',
  },
  playerNum: {
    color: 'var(--text-muted)',
    fontSize: '0.75rem',
    fontWeight: 700,
    width: 18,
    textAlign: 'center',
  },
  playerName: {
    flex: 1,
    fontWeight: 700,
    fontSize: '0.95rem',
  },
  waitingMsg: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    gap: '0.75rem',
    color: 'var(--text-muted)',
    padding: '1.5rem',
    fontSize: '0.9rem',
  },
};
