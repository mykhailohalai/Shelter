import { useState } from 'react';
import type { GameState } from '../types';
import { PHASE_LABEL } from '../types';
import { api } from '../api';
import { getActivePlayers, getExiledPlayers, isMyTurnForBunker } from '../utils';
import { BunkerCardSlot } from './GameCard';
import { PlayerRow } from './PlayerRow';
import { PlayerHand } from './PlayerHand';
import { VotingPanel } from './VotingPanel';
import { ExileReveal } from './ExileReveal';

interface Props {
  state: GameState;
  gameId: string;
  refresh: () => Promise<void>;
  onLeave: () => void;
}

export function GameBoard({ state, gameId, refresh, onLeave }: Props) {
  const [busy, setBusy] = useState(false);
  const [actionError, setActionError] = useState('');
  const [showLeaveConfirm, setShowLeaveConfirm] = useState(false);

  const activePlayers = getActivePlayers(state);
  const exiledPlayers = getExiledPlayers(state);
  const myTurnBunker  = isMyTurnForBunker(state);
  const isVoting      = state.phase === 'voting' || state.phase === 'tie_break';
  const isCardReveal  = state.phase === 'card_reveal';
  const isBunkerPhase = state.phase === 'bunker_explore';
  const isExileReveal = state.phase === 'exile_reveal';

  async function doAction(fn: () => Promise<void>) {
    setBusy(true);
    setActionError('');
    try {
      await fn();
      await refresh();
    } catch (e) {
      setActionError(e instanceof Error ? e.message : 'Помилка');
    } finally {
      setBusy(false);
    }
  }

  const speakingPlayer = state.players.find(p => p.isSpeaking);

  return (
    <div style={styles.page}>
      {/* ── Top bar ───────────────────────────────────────── */}
      <header style={styles.topBar}>
        <div style={styles.topLeft}>
          <span style={styles.logoText}>☢ БУНКЕР</span>
          <div style={styles.roundBadge}>
            <span style={{ color: 'var(--text-muted)', fontSize: '0.65rem' }}>РАУНД</span>
            <span style={{ color: 'var(--gold)', fontSize: '1.2rem', fontWeight: 700 }}>{state.round}</span>
            <span style={{ color: 'var(--text-muted)', fontSize: '0.65rem' }}>/ 5</span>
          </div>
          <span className="badge badge-muted" style={{ fontSize: '0.65rem' }}>
            {PHASE_LABEL[state.phase]}
          </span>
        </div>
        <div style={styles.topRight}>
          <span style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>
            Бункер: <strong style={{ color: 'var(--gold)' }}>{state.bunkerCapacity}</strong> місць
          </span>
          <button className="btn btn-ghost btn-sm" onClick={() => setShowLeaveConfirm(true)}>
            Вийти
          </button>
        </div>
      </header>

      {/* ── Main layout ───────────────────────────────────── */}
      <div style={styles.main}>
        {/* Left column */}
        <aside style={styles.aside}>
          {/* Catastrophe card */}
          <div style={styles.catCard}>
            <p style={styles.catLabel}>☢ КАТАСТРОФА</p>
            <h3 style={styles.catTitle}>{state.catastrophe.title}</h3>
            <p style={styles.catDesc}>{state.catastrophe.description}</p>
          </div>

          {/* Bunker cards */}
          <div style={styles.bunkerSection}>
            <p style={styles.sectionLabel}>🏚 БУНКЕР</p>
            <div style={styles.bunkerGrid}>
              {state.bunkerCards.map(card => (
                <BunkerCardSlot
                  key={card.id}
                  card={card}
                  canReveal={isBunkerPhase && myTurnBunker && !card.revealed}
                  onClick={() =>
                    doAction(() => api.revealBunkerCard(gameId))
                  }
                />
              ))}
            </div>
          </div>

          {/* Bunker explore prompt */}
          {isBunkerPhase && (
            <div style={styles.phaseBox}>
              {myTurnBunker ? (
                <>
                  {state.bunkerCards.some(c => !c.revealed) ? (
                    <p style={styles.phaseHint}>Натисніть на закриту картку бункера щоб відкрити її</p>
                  ) : (
                    <>
                      <p style={styles.phaseHint}>Всі картки бункера вже відкриті</p>
                      <button
                        className="btn btn-outline btn-sm"
                        style={{ marginTop: '0.5rem' }}
                        onClick={() => doAction(() => api.revealBunkerCard(gameId))}
                        disabled={busy}
                      >
                        Продовжити →
                      </button>
                    </>
                  )}
                  {actionError && <div className="alert alert-error">{actionError}</div>}
                </>
              ) : (
                <p style={styles.phaseHint}>
                  {speakingPlayer
                    ? `${speakingPlayer.name} досліджує бункер…`
                    : 'Очікування дослідження бункера…'}
                </p>
              )}
            </div>
          )}
        </aside>

        {/* Right column */}
        <main style={styles.content}>
          {/* Phase banner */}
          {isCardReveal && speakingPlayer && (
            <div style={styles.phaseBanner}>
              <span style={{ fontSize: '0.7rem', textTransform: 'uppercase', letterSpacing: 2, color: 'var(--gold)' }}>
                Говорить:
              </span>
              <strong style={{ fontSize: '1.1rem' }}>{speakingPlayer.name}</strong>
              <span style={{ fontSize: '0.8rem', color: 'var(--text-muted)' }}>
                — обґрунтовує корисність для бункера
              </span>
            </div>
          )}

          {/* Players list */}
          <div style={styles.playersSection}>
            <div style={styles.playersHeader}>
              <p style={styles.sectionLabel}>
                ГРАВЦІ ({activePlayers.length} активних)
              </p>
              {exiledPlayers.length > 0 && (
                <span style={{ fontSize: '0.72rem', color: 'var(--text-muted)' }}>
                  {exiledPlayers.length} вигнано
                </span>
              )}
            </div>

            <div style={styles.playersList}>
              {activePlayers.map(player => (
                <PlayerRow
                  key={player.id}
                  player={player}
                  state={state}
                  isMe={player.id === state.myId}
                  voteMode={isVoting}
                  onVote={targetId => doAction(() => api.castVote(gameId, targetId))}
                />
              ))}
            </div>

            {exiledPlayers.length > 0 && (
              <>
                <p style={{ ...styles.sectionLabel, marginTop: '1rem', marginBottom: '0.5rem', color: 'var(--danger)' }}>
                  ВИГНАНЦІ
                </p>
                <div style={styles.playersList}>
                  {exiledPlayers.map(player => (
                    <PlayerRow
                      key={player.id}
                      player={player}
                      state={state}
                      isMe={player.id === state.myId}
                      voteMode={isVoting}
                      onVote={targetId => doAction(() => api.castVote(gameId, targetId))}
                    />
                  ))}
                </div>
              </>
            )}
          </div>

          {/* Voting panel */}
          {isVoting && (
            <VotingPanel state={state} gameId={gameId} refresh={refresh} />
          )}

          {/* My hand — always visible once the game starts */}
          <PlayerHand state={state} gameId={gameId} refresh={refresh} />

        </main>
      </div>

      {/* ── Exile reveal overlay ───────────────────────── */}
      {isExileReveal && state.exiledPlayerId && (
        <ExileReveal state={state} gameId={gameId} refresh={refresh} />
      )}

      {/* ── Leave confirm ─────────────────────────────── */}
      {showLeaveConfirm && (
        <div className="overlay">
          <div className="modal">
            <h3 className="modal-title">Вийти з гри?</h3>
            <p style={{ color: 'var(--text-muted)', marginBottom: '1.5rem', fontSize: '0.9rem' }}>
              Ви покинете поточну гру. Повернутися буде неможливо.
            </p>
            <div style={{ display: 'flex', gap: '0.75rem' }}>
              <button className="btn btn-danger" onClick={onLeave}>Вийти</button>
              <button className="btn btn-outline" onClick={() => setShowLeaveConfirm(false)}>
                Скасувати
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  page: {
    minHeight: '100vh',
    background: 'var(--bg)',
    display: 'flex',
    flexDirection: 'column',
  },
  topBar: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: '0.6rem 1rem',
    background: 'var(--bg2)',
    borderBottom: '1px solid var(--border)',
    gap: '0.75rem',
    flexWrap: 'wrap',
    position: 'sticky',
    top: 0,
    zIndex: 10,
  },
  topLeft: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.75rem',
    flexWrap: 'wrap',
  },
  topRight: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.75rem',
  },
  logoText: {
    fontFamily: "'Oswald', sans-serif",
    fontSize: '1.1rem',
    fontWeight: 700,
    color: 'var(--gold)',
    letterSpacing: '3px',
  },
  roundBadge: {
    display: 'flex',
    alignItems: 'baseline',
    gap: '0.2rem',
    background: 'var(--bg3)',
    borderRadius: 'var(--radius)',
    padding: '0.2rem 0.6rem',
  },
  main: {
    display: 'grid',
    gridTemplateColumns: '240px 1fr',
    gap: '1rem',
    padding: '1rem',
    flex: 1,
    alignItems: 'start',
  },
  aside: {
    display: 'flex',
    flexDirection: 'column',
    gap: '0.75rem',
    position: 'sticky',
    top: '52px',
  },
  catCard: {
    background: 'rgba(140,30,0,0.2)',
    border: '1px solid rgba(255,100,50,0.35)',
    borderRadius: 'var(--radius-lg)',
    padding: '1rem',
  },
  catLabel: {
    fontSize: '0.6rem',
    letterSpacing: '2px',
    textTransform: 'uppercase',
    color: '#ff8a65',
    marginBottom: '0.4rem',
  },
  catTitle: {
    fontSize: '1rem',
    fontWeight: 700,
    color: '#ffccbc',
    marginBottom: '0.4rem',
    fontFamily: "'Oswald', sans-serif",
    textTransform: 'uppercase',
    letterSpacing: '1px',
  },
  catDesc: {
    fontSize: '0.75rem',
    color: 'var(--text-dim)',
    lineHeight: 1.5,
  },
  bunkerSection: {
    background: 'var(--bg2)',
    border: '1px solid var(--border)',
    borderRadius: 'var(--radius-lg)',
    padding: '0.85rem',
  },
  bunkerGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(3, 1fr)',
    gap: '6px',
    marginTop: '0.5rem',
  },
  sectionLabel: {
    fontSize: '0.65rem',
    letterSpacing: '2px',
    textTransform: 'uppercase',
    color: 'var(--text-muted)',
    marginBottom: '0',
  },
  phaseBox: {
    background: 'var(--bg2)',
    border: '1px solid var(--border)',
    borderRadius: 'var(--radius)',
    padding: '0.75rem',
  },
  phaseHint: {
    fontSize: '0.78rem',
    color: 'var(--text-muted)',
    fontStyle: 'italic',
  },
  content: {
    display: 'flex',
    flexDirection: 'column',
    gap: '0.75rem',
    minWidth: 0,
  },
  phaseBanner: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.5rem',
    background: 'var(--gold-bg)',
    border: '1px solid rgba(245,197,24,0.3)',
    borderRadius: 'var(--radius)',
    padding: '0.6rem 0.9rem',
    flexWrap: 'wrap',
  },
  playersSection: {
    background: 'var(--bg2)',
    border: '1px solid var(--border)',
    borderRadius: 'var(--radius-lg)',
    padding: '1rem',
  },
  playersHeader: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    marginBottom: '0.75rem',
  },
  playersList: {
    display: 'flex',
    flexDirection: 'column',
    gap: '0.4rem',
  },
};
