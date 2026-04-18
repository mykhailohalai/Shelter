import type { GameState, Player } from '../types';
import { CARD_TYPE_UA, CARD_TYPE_COLOR } from '../types';
import { api } from '../api';
import { useState } from 'react';

interface Props {
  state: GameState;
  gameId: string;
  refresh: () => Promise<void>;
}

export function ExileReveal({ state, gameId, refresh }: Props) {
  const [busy, setBusy] = useState(false);
  const exiled = state.players.find(p => p.id === state.exiledPlayerId);

  if (!exiled) return null;

  async function continueGame() {
    setBusy(true);
    try {
      await api.nextPhase(gameId);
      await refresh();
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="overlay">
      <div style={styles.modal}>
        <div style={styles.skull}>☠</div>
        <h2 style={styles.title}>Вигнанець визначений!</h2>
        <p style={styles.name}>{exiled.name}</p>
        <p style={styles.sub}>залишає бункер…</p>

        {/* Reveal all cards */}
        <div style={styles.cardsSection}>
          <p style={styles.cardsLabel}>Всі картки гравця:</p>
          <div style={styles.cardsGrid}>
            {exiled.cards.map(card => (
              <div key={card.id} style={styles.cardSlot}>
                <div
                  style={{
                    ...styles.cardHeader,
                    background: CARD_TYPE_COLOR[card.type],
                  }}
                >
                  {CARD_TYPE_UA[card.type]}
                </div>
                <div style={styles.cardBody}>
                  {card.content ?? '—'}
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Special card info if played */}
        {exiled.specialCard && (
          <div style={styles.special}>
            <span style={{ fontSize: '0.65rem', color: '#b388ff', textTransform: 'uppercase', letterSpacing: 1.5 }}>
              Особлива умова
            </span>
            <strong style={{ color: '#e0b0ff' }}>{exiled.specialCard.title}</strong>
            <p style={{ fontSize: '0.78rem', color: 'var(--text-dim)' }}>
              {exiled.specialCard.description}
            </p>
          </div>
        )}

        {state.isHost && (
          <button
            className="btn btn-primary btn-full"
            style={{ marginTop: '1.5rem' }}
            onClick={continueGame}
            disabled={busy}
          >
            {busy ? 'Продовження…' : 'Продовжити гру →'}
          </button>
        )}
        {!state.isHost && (
          <p style={{ color: 'var(--text-muted)', fontSize: '0.8rem', textAlign: 'center', marginTop: '1rem' }}>
            Очікування ведучого…
          </p>
        )}
      </div>
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  modal: {
    background: 'var(--bg2)',
    border: '2px solid var(--danger)',
    borderRadius: 'var(--radius-lg)',
    boxShadow: 'var(--shadow)',
    maxWidth: 520,
    width: '100%',
    padding: '2rem',
    textAlign: 'center',
    maxHeight: '90vh',
    overflowY: 'auto',
  },
  skull: {
    fontSize: '3.5rem',
    marginBottom: '0.5rem',
  },
  title: {
    color: 'var(--danger)',
    fontSize: '1.4rem',
    marginBottom: '0.5rem',
  },
  name: {
    fontFamily: "'Oswald', sans-serif",
    fontSize: '2rem',
    fontWeight: 700,
    color: 'var(--text)',
    letterSpacing: '2px',
  },
  sub: {
    color: 'var(--text-muted)',
    fontSize: '0.85rem',
    marginBottom: '1.5rem',
  },
  cardsSection: {
    textAlign: 'left',
  },
  cardsLabel: {
    fontSize: '0.7rem',
    color: 'var(--text-muted)',
    textTransform: 'uppercase',
    letterSpacing: '1.5px',
    marginBottom: '0.75rem',
  },
  cardsGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(3, 1fr)',
    gap: '0.5rem',
  },
  cardSlot: {
    borderRadius: 'var(--radius)',
    overflow: 'hidden',
    boxShadow: 'var(--shadow-card)',
  },
  cardHeader: {
    padding: '0.25rem 0.4rem',
    fontSize: '0.55rem',
    fontFamily: "'Oswald', sans-serif",
    fontWeight: 700,
    letterSpacing: '1.5px',
    color: '#fff',
    textTransform: 'uppercase',
  },
  cardBody: {
    background: '#f5e49c',
    color: '#1a1200',
    fontSize: '0.72rem',
    fontWeight: 700,
    fontFamily: "'Oswald', sans-serif",
    textTransform: 'uppercase',
    padding: '0.5rem',
    minHeight: 44,
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    textAlign: 'center',
  },
  special: {
    marginTop: '1rem',
    background: 'rgba(74,20,140,0.2)',
    border: '1px solid rgba(179,136,255,0.3)',
    borderRadius: 'var(--radius)',
    padding: '0.75rem',
    display: 'flex',
    flexDirection: 'column',
    gap: '0.25rem',
    textAlign: 'left',
  },
};
