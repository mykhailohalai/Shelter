import { useState } from 'react';
import type { GameState } from '../types';
import { PlayerCard } from './GameCard';
import { api } from '../api';
import { getMyPlayer, getUnrevealedCards, canIRevealCard, canPlaySpecial } from '../utils';

interface Props {
  state: GameState;
  gameId: string;
  refresh: () => Promise<void>;
}

export function PlayerHand({ state, gameId, refresh }: Props) {
  const [selectedCardId, setSelectedCardId] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');

  const me = getMyPlayer(state);
  if (!me) return null;

  const myTurn = canIRevealCard(state);
  const canSpecial = canPlaySpecial(state);
  const unrevealed = getUnrevealedCards(me);
  const isRound1 = state.round === 1;

  async function handleReveal() {
    const cardId = isRound1
      ? me!.cards.find(c => c.type === 'profession')?.id
      : selectedCardId;
    if (!cardId) return;
    setBusy(true);
    setError('');
    try {
      await api.revealCard(gameId, cardId);
      await refresh();
      setSelectedCardId(null);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Помилка');
    } finally {
      setBusy(false);
    }
  }

  async function handlePlaySpecial() {
    setBusy(true);
    setError('');
    try {
      await api.playSpecialCard(gameId);
      await refresh();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Помилка');
    } finally {
      setBusy(false);
    }
  }

  return (
    <div style={styles.panel}>
      <div style={styles.header}>
        <h3 style={{ color: 'var(--gold)', fontSize: '0.85rem' }}>Ваші картки — {me.name}</h3>
        {myTurn && (
          <span className="badge badge-gold" style={{ animation: 'none' }}>Ваша черга</span>
        )}
      </div>

      {/* Cards row */}
      <div style={styles.cardsRow}>
        {me.cards.map(card => {
          const canSelect = myTurn && !card.revealed && card.content !== null && !isRound1;
          return (
            <PlayerCard
              key={card.id}
              card={card}
              selectable={canSelect}
              selected={selectedCardId === card.id}
              onClick={() => canSelect && setSelectedCardId(
                selectedCardId === card.id ? null : card.id,
              )}
              size="md"
            />
          );
        })}
      </div>

      {/* Action area */}
      {myTurn && (
        <div style={styles.actions}>
          {isRound1 ? (
            <>
              <p style={styles.hint}>
                Перший раунд: всі відкривають картку <strong>Професії</strong>
              </p>
              <button
                className="btn btn-primary"
                onClick={handleReveal}
                disabled={busy}
              >
                {busy ? 'Відкривається…' : 'Відкрити Професію'}
              </button>
            </>
          ) : (
            <>
              {unrevealed.length > 0 ? (
                <>
                  <p style={styles.hint}>Оберіть картку для відкриття</p>
                  <button
                    className="btn btn-primary"
                    onClick={handleReveal}
                    disabled={!selectedCardId || busy}
                  >
                    {busy ? 'Відкривається…' : 'Відкрити вибрану картку'}
                  </button>
                </>
              ) : (
                <p style={styles.hint}>Всі картки вже відкриті</p>
              )}
            </>
          )}
          {error && <div className="alert alert-error">{error}</div>}
        </div>
      )}

      {/* Special condition card */}
      {me.specialCard && !me.specialCard.played && (
        <div style={styles.specialCard}>
          <div style={styles.specialHeader}>
            <span style={{ fontSize: '0.6rem', letterSpacing: 1.5, textTransform: 'uppercase', color: '#b388ff' }}>
              Особлива умова
            </span>
            <strong style={{ color: '#e0b0ff', fontSize: '0.85rem' }}>{me.specialCard.title}</strong>
          </div>
          <p style={{ fontSize: '0.78rem', color: 'var(--text-dim)', marginBottom: '0.5rem' }}>
            {me.specialCard.description}
          </p>
          {!canSpecial && (
            <p style={{ fontSize: '0.7rem', color: 'var(--text-muted)', fontStyle: 'italic', marginBottom: '0.5rem' }}>
              Доступно під час вашого ходу або перед вигнанням
            </p>
          )}
          <button
            className="btn btn-sm"
            style={{ background: canSpecial ? '#4a148c' : '#2a1050', color: canSpecial ? '#fff' : 'var(--text-muted)' }}
            onClick={handlePlaySpecial}
            disabled={busy || !canSpecial}
          >
            Розіграти
          </button>
        </div>
      )}
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  panel: {
    background: 'var(--bg2)',
    border: '1px solid var(--border)',
    borderRadius: 'var(--radius-lg)',
    padding: '1rem',
  },
  header: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    marginBottom: '0.75rem',
  },
  cardsRow: {
    display: 'flex',
    gap: '6px',
    flexWrap: 'wrap',
  },
  actions: {
    marginTop: '0.75rem',
    display: 'flex',
    flexDirection: 'column',
    gap: '0.5rem',
  },
  hint: {
    fontSize: '0.78rem',
    color: 'var(--text-muted)',
  },
  specialCard: {
    marginTop: '0.75rem',
    background: 'rgba(74,20,140,0.15)',
    border: '1px solid rgba(179,136,255,0.3)',
    borderRadius: 'var(--radius)',
    padding: '0.75rem',
  },
  specialHeader: {
    display: 'flex',
    flexDirection: 'column',
    gap: '0.2rem',
    marginBottom: '0.4rem',
  },
};
