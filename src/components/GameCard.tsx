import type { Card, BunkerCard, CardType } from '../types';
import { CARD_TYPE_UA, CARD_TYPE_COLOR } from '../types';

/* ── Player character card ─────────────────────────────────── */
interface PlayerCardProps {
  card: Card;
  selectable?: boolean;
  selected?: boolean;
  onClick?: () => void;
  size?: 'sm' | 'md';
}

export function PlayerCard({ card, selectable, selected, onClick, size = 'md' }: PlayerCardProps) {
  const isSmall = size === 'sm';

  // content === null means backend didn't send it (other player's hidden card)
  // content !== null means you can see it (your own card or already revealed to everyone)
  if (card.content === null) {
    return (
      <div
        className="game-card game-card-face-down"
        style={{
          width: isSmall ? 60 : 90,
          cursor: selectable ? 'pointer' : 'default',
          border: selected ? '2px solid var(--gold)' : undefined,
          opacity: selectable ? 1 : 0.6,
        }}
        onClick={selectable ? onClick : undefined}
        title={selectable ? 'Натисніть щоб відкрити' : CARD_TYPE_UA[card.type]}
      >
        <span style={{ fontSize: isSmall ? '1rem' : '1.3rem' }}>🃏</span>
        <span style={{ fontSize: isSmall ? '0.5rem' : '0.55rem', textAlign: 'center' }}>
          {CARD_TYPE_UA[card.type]}
        </span>
      </div>
    );
  }

  const bgColor = CARD_TYPE_COLOR[card.type as CardType];

  return (
    <div
      className="game-card"
      style={{
        width: isSmall ? 60 : 90,
        cursor: selectable ? 'pointer' : 'default',
        boxShadow: selected ? `0 0 0 2px var(--gold), var(--shadow-card)` : undefined,
        transform: selected ? 'translateY(-3px)' : undefined,
        transition: 'transform 0.15s, box-shadow 0.15s',
      }}
      onClick={selectable ? onClick : undefined}
    >
      <div
        className="game-card-header"
        style={{ background: bgColor, fontSize: isSmall ? '0.5rem' : '0.6rem' }}
      >
        {CARD_TYPE_UA[card.type as CardType]}
      </div>
      <div
        className="game-card-body"
        style={{ fontSize: isSmall ? '0.6rem' : '0.75rem', minHeight: isSmall ? 36 : 52 }}
      >
        {card.content}
      </div>
    </div>
  );
}

/* ── Bunker card slot ──────────────────────────────────────── */
interface BunkerCardProps {
  card: BunkerCard;
  onClick?: () => void;
  canReveal?: boolean;
}

export function BunkerCardSlot({ card, onClick, canReveal }: BunkerCardProps) {
  if (!card.revealed || card.content === null) {
    return (
      <div
        style={{
          ...slotStyles.base,
          cursor: canReveal ? 'pointer' : 'default',
          border: canReveal ? '2px dashed var(--gold)' : '2px dashed var(--border)',
          background: canReveal ? 'rgba(245,197,24,0.04)' : 'var(--bg3)',
          transition: 'all 0.15s',
        }}
        onClick={canReveal ? onClick : undefined}
        title={canReveal ? 'Натисніть щоб відкрити картку бункера' : undefined}
      >
        <span style={{ fontSize: '1.4rem' }}>🔒</span>
        <span style={{ fontSize: '0.55rem', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: 1 }}>
          {canReveal ? 'Відкрити' : `Бункер ${card.slot}`}
        </span>
      </div>
    );
  }

  return (
    <div style={{ ...slotStyles.base, ...slotStyles.revealed }}>
      <span style={{ fontSize: '1.1rem' }}>🏚</span>
      <p style={slotStyles.content}>{card.content}</p>
    </div>
  );
}

const slotStyles: Record<string, React.CSSProperties> = {
  base: {
    borderRadius: 'var(--radius)',
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    justifyContent: 'center',
    gap: '0.3rem',
    padding: '0.6rem 0.4rem',
    minHeight: 90,
    textAlign: 'center',
  },
  revealed: {
    background: '#1e2e1e',
    border: '2px solid #2e5e2e',
  },
  content: {
    color: '#a0d8a0',
    fontSize: '0.65rem',
    fontWeight: 700,
    textTransform: 'uppercase',
    letterSpacing: '0.5px',
    lineHeight: 1.3,
    marginTop: '0.15rem',
  },
};
