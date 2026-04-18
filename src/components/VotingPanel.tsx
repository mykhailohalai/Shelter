import { useState } from 'react';
import type { GameState } from '../types';
import { api } from '../api';
import { canIVote, getActivePlayers } from '../utils';

interface Props {
  state: GameState;
  gameId: string;
  refresh: () => Promise<void>;
}

export function VotingPanel({ state, gameId, refresh }: Props) {
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');

  const isTieBreak = state.phase === 'tie_break';
  const myVote = state.myVote;
  const iVoted = myVote !== null;
  const candidates = isTieBreak
    ? state.players.filter(p => state.votes.find(v => v.voterId === 'TIE' && v.targetId === p.id) ||
        // fallback: all tied players from previous vote — back-end should mark them
        !p.isExiled)
    : getActivePlayers(state);

  const totalVoters = state.players.length;
  const castCount   = state.votes.filter(v => v.targetId !== null).length;

  async function handleVote(targetId: string) {
    if (iVoted || busy) return;
    setBusy(true);
    setError('');
    try {
      await api.castVote(gameId, targetId);
      await refresh();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Помилка');
    } finally {
      setBusy(false);
    }
  }

  return (
    <div style={styles.panel}>
      {/* Header */}
      <div style={styles.header}>
        <div>
          <h3 style={{ color: 'var(--danger)', marginBottom: '0.2rem' }}>
            {isTieBreak ? '⚖ Повторне голосування' : '🗳 Голосування'}
          </h3>
          <p style={styles.subline}>
            {isTieBreak
              ? 'Нічия! Обирайте ще раз серед позначених кандидатів'
              : 'Хто має покинути бункер?'}
          </p>
        </div>
        <div style={styles.counter}>
          <span style={{ fontSize: '1.6rem', fontWeight: 700, color: castCount === totalVoters ? 'var(--gold)' : 'var(--text)' }}>
            {castCount}
          </span>
          <span style={{ color: 'var(--text-muted)', fontSize: '0.7rem' }}>/ {totalVoters}</span>
          <span style={{ color: 'var(--text-muted)', fontSize: '0.6rem', letterSpacing: 1 }}>голосів</span>
        </div>
      </div>

      {/* Vote progress */}
      <div style={styles.progressBar}>
        <div
          style={{
            ...styles.progressFill,
            width: totalVoters ? `${(castCount / totalVoters) * 100}%` : '0%',
          }}
        />
      </div>

      {/* Candidates */}
      <div style={styles.candidates}>
        {candidates.map(player => {
          const isMe       = player.id === state.myId;
          const votes      = state.votes.filter(v => v.targetId === player.id).length;
          const iVotedThis = myVote === player.id;
          const canVote    = canIVote(state) && !isMe;

          return (
            <button
              key={player.id}
              style={{
                ...styles.candidate,
                ...(iVotedThis ? styles.candidateVoted : {}),
                ...(canVote ? styles.candidateHover : {}),
                cursor: canVote ? 'pointer' : 'default',
                opacity: isMe && !iVotedThis ? 0.5 : 1,
              }}
              onClick={() => canVote && handleVote(player.id)}
              disabled={!canVote || busy}
              title={isMe ? 'За себе голосувати не можна' : ''}
            >
              <span style={styles.candidateName}>{player.name}</span>
              {isMe && (
                <span className="badge badge-muted" style={{ fontSize: '0.55rem' }}>Ви</span>
              )}
              {iVotedThis && (
                <span className="badge badge-danger" style={{ fontSize: '0.55rem' }}>← ваш голос</span>
              )}
              <span style={styles.voteBar}>
                {Array.from({ length: votes }).map((_, i) => (
                  <span key={i} style={styles.voteDot} />
                ))}
                {votes > 0 && (
                  <span style={styles.voteNum}>{votes}</span>
                )}
              </span>
            </button>
          );
        })}
      </div>

      {/* Who voted for whom (shown after you vote) */}
      {iVoted && state.votes.some(v => v.targetId !== null) && (
        <div style={styles.voteList}>
          <p style={{ fontSize: '0.7rem', color: 'var(--text-muted)', marginBottom: '0.4rem', textTransform: 'uppercase', letterSpacing: 1 }}>
            Голоси:
          </p>
          {state.votes.map(v => (
            <div key={v.voterId} style={styles.voteListRow}>
              <span style={{ color: 'var(--text-dim)', fontSize: '0.8rem' }}>{v.voterName}</span>
              <span style={{ color: 'var(--text-muted)', fontSize: '0.7rem' }}>→</span>
              <span style={{ fontSize: '0.8rem', color: v.targetId ? 'var(--text)' : 'var(--text-muted)' }}>
                {v.targetId
                  ? state.players.find(p => p.id === v.targetId)?.name ?? '?'
                  : 'ще не проголосував'}
              </span>
            </div>
          ))}
        </div>
      )}

      {error && <div className="alert alert-error" style={{ marginTop: '0.75rem' }}>{error}</div>}

      {!canIVote(state) && !iVoted && (
        <p style={{ color: 'var(--text-muted)', fontSize: '0.8rem', textAlign: 'center', marginTop: '0.5rem' }}>
          Очікування голосів…
        </p>
      )}
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  panel: {
    background: 'var(--bg2)',
    border: '1px solid var(--danger)',
    borderRadius: 'var(--radius-lg)',
    padding: '1.25rem',
  },
  header: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: '0.75rem',
    gap: '0.5rem',
  },
  subline: {
    color: 'var(--text-muted)',
    fontSize: '0.8rem',
  },
  counter: {
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    background: 'var(--bg3)',
    borderRadius: 'var(--radius)',
    padding: '0.4rem 0.75rem',
    minWidth: 60,
    textAlign: 'center',
  },
  progressBar: {
    height: 4,
    background: 'var(--border)',
    borderRadius: 2,
    marginBottom: '1rem',
    overflow: 'hidden',
  },
  progressFill: {
    height: '100%',
    background: 'var(--danger)',
    borderRadius: 2,
    transition: 'width 0.3s',
  },
  candidates: {
    display: 'flex',
    flexDirection: 'column',
    gap: '0.4rem',
  },
  candidate: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.5rem',
    padding: '0.55rem 0.75rem',
    background: 'var(--bg3)',
    border: '1px solid var(--border)',
    borderRadius: 'var(--radius)',
    color: 'var(--text)',
    fontFamily: "'Roboto Condensed', sans-serif",
    fontSize: '0.9rem',
    textAlign: 'left',
    transition: 'background 0.15s, border-color 0.15s',
  },
  candidateVoted: {
    background: 'rgba(229,57,53,0.15)',
    borderColor: 'var(--danger)',
  },
  candidateHover: {},
  candidateName: {
    flex: 1,
    fontWeight: 700,
  },
  voteBar: {
    display: 'flex',
    alignItems: 'center',
    gap: '3px',
  },
  voteDot: {
    display: 'inline-block',
    width: 8,
    height: 8,
    borderRadius: '50%',
    background: 'var(--danger)',
  },
  voteNum: {
    fontSize: '0.75rem',
    fontWeight: 700,
    color: 'var(--danger)',
    marginLeft: '2px',
  },
  voteList: {
    marginTop: '0.75rem',
    padding: '0.75rem',
    background: 'var(--bg3)',
    borderRadius: 'var(--radius)',
  },
  voteListRow: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.5rem',
    padding: '0.2rem 0',
  },
};
