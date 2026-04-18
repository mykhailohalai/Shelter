import type { Player, GameState } from '../types';
import { PlayerCard } from './GameCard';
import { voteCountFor } from '../utils';

interface Props {
  player: Player;
  state: GameState;
  isMe: boolean;
  onVote?: (playerId: string) => void;
  voteMode?: boolean;
  tiedIds?: string[];
}

export function PlayerRow({ player, state, isMe, onVote, voteMode, tiedIds }: Props) {
  const isExiled    = player.isExiled;
  const isSpeaking  = player.isSpeaking;
  const voteCount   = voteCountFor(state, player.id);
  const myVotedThis = state.myVote === player.id;
  const isTied      = tiedIds?.includes(player.id);
  const exiledThis  = state.exiledPlayerId === player.id;
  const canVoteFor  = voteMode && !isMe && !isExiled;

  return (
    <div
      style={{
        ...styles.row,
        ...(isExiled  ? styles.exiledRow  : {}),
        ...(isSpeaking && !isExiled ? styles.speakingRow : {}),
        ...(exiledThis ? styles.exiledNowRow : {}),
        cursor: canVoteFor ? 'pointer' : 'default',
        outline: myVotedThis ? '2px solid var(--danger)' : (isTied ? '2px solid var(--gold)' : 'none'),
      }}
      onClick={canVoteFor ? () => onVote?.(player.id) : undefined}
    >
      {/* Name + badges */}
      <div style={styles.nameSection}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.4rem', flexWrap: 'wrap' }}>
          <span style={styles.name}>{player.name}</span>
          {isMe       && <span className="badge badge-info"  style={{ fontSize: '0.55rem' }}>Ви</span>}
          {isSpeaking && !isExiled && <span className="badge badge-gold" style={{ fontSize: '0.55rem' }}>Говорить</span>}
          {isExiled   && <span className="badge badge-danger" style={{ fontSize: '0.55rem' }}>Вигнанець</span>}
          {exiledThis && <span className="badge badge-danger" style={{ fontSize: '0.55rem' }}>☠ Вигнано</span>}
        </div>

        {/* Vote count badge */}
        {voteMode && voteCount > 0 && (
          <span style={styles.voteCount}>{voteCount} голос{voteCount === 1 ? '' : 'ів'}</span>
        )}
        {myVotedThis && (
          <span style={styles.myVoteLabel}>← ваш голос</span>
        )}
      </div>

      {/* Cards — show face-down until revealed to everyone */}
      <div style={styles.cards}>
        {player.cards.map(card => (
          <PlayerCard
            key={card.id}
            card={card.revealed ? card : { ...card, content: null }}
            size="sm"
          />
        ))}
      </div>
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  row: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.75rem',
    padding: '0.6rem 0.75rem',
    background: 'var(--bg3)',
    borderRadius: 'var(--radius)',
    border: '1px solid transparent',
    transition: 'background 0.15s, outline 0.15s',
  },
  speakingRow: {
    background: 'rgba(245,197,24,0.07)',
    border: '1px solid rgba(245,197,24,0.3)',
  },
  exiledRow: {
    background: 'rgba(120,0,0,0.18)',
    border: '1px solid rgba(120,0,0,0.35)',
    opacity: 0.75,
  },
  exiledNowRow: {
    background: 'rgba(180,0,0,0.25)',
    border: '1px solid var(--danger)',
    opacity: 1,
  },
  nameSection: {
    flex: 1,
    minWidth: 0,
  },
  name: {
    fontWeight: 700,
    fontSize: '0.95rem',
    overflow: 'hidden',
    textOverflow: 'ellipsis',
    whiteSpace: 'nowrap',
  },
  voteCount: {
    display: 'inline-block',
    marginTop: '0.2rem',
    fontSize: '0.72rem',
    color: 'var(--danger)',
    fontWeight: 700,
  },
  myVoteLabel: {
    display: 'inline-block',
    marginTop: '0.2rem',
    fontSize: '0.68rem',
    color: 'var(--danger)',
    fontStyle: 'italic',
  },
  cards: {
    display: 'flex',
    gap: '3px',
    flexShrink: 0,
  },
};
