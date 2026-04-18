import type { GameState, Player } from '../types';
import { CARD_TYPE_UA, CARD_TYPE_COLOR } from '../types';

interface Props {
  state: GameState;
  onNewGame: () => void;
}

export function FinalScreen({ state, onNewGame }: Props) {
  const inBunker  = state.players.filter(p => !p.isExiled);
  const exiled    = state.players.filter(p => p.isExiled);

  return (
    <div style={styles.page}>
      {/* Hero */}
      <div style={styles.hero}>
        <div style={styles.heroIcon}>🏚</div>
        <h1 style={styles.heroTitle}>ФІНАЛ</h1>
        <p style={styles.heroSub}>
          У бункер потрапили{' '}
          <strong style={{ color: 'var(--gold)' }}>{inBunker.length}</strong>{' '}
          з {state.players.length} гравців
        </p>
      </div>

      {/* Bunker group */}
      <section style={styles.section}>
        <div style={styles.sectionTitle}>
          <span style={{ fontSize: '1.3rem' }}>✅</span>
          <h2 style={{ color: 'var(--success)', fontSize: '1.1rem' }}>В БУНКЕРІ</h2>
          <span className="badge badge-success">{inBunker.length} гравців</span>
        </div>
        {inBunker.map(player => (
          <PlayerSummary key={player.id} player={player} highlight />
        ))}
      </section>

      {/* Catastrophe info */}
      <section style={styles.catCard}>
        <p style={{ fontSize: '0.65rem', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: 2, marginBottom: '0.4rem' }}>
          Катастрофа
        </p>
        <h3 style={{ color: '#ff8a65', marginBottom: '0.5rem', fontSize: '1.1rem' }}>
          {state.catastrophe.title}
        </h3>
        <p style={{ color: 'var(--text-dim)', fontSize: '0.85rem', lineHeight: 1.5 }}>
          {state.catastrophe.description}
        </p>
      </section>

      {/* Revealed bunker cards */}
      {state.bunkerCards.some(c => c.revealed) && (
        <section style={styles.section}>
          <div style={styles.sectionTitle}>
            <span style={{ fontSize: '1.3rem' }}>🏚</span>
            <h2 style={{ color: '#a0d8a0', fontSize: '1.1rem' }}>БУНКЕР</h2>
          </div>
          <div style={styles.bunkerGrid}>
            {state.bunkerCards.filter(c => c.revealed && c.content).map(c => (
              <div key={c.id} style={styles.bunkerSlot}>
                {c.content}
              </div>
            ))}
          </div>
        </section>
      )}

      {/* Exiled group */}
      {exiled.length > 0 && (
        <section style={styles.section}>
          <div style={styles.sectionTitle}>
            <span style={{ fontSize: '1.3rem' }}>☠</span>
            <h2 style={{ color: 'var(--danger)', fontSize: '1.1rem' }}>ВИГНАНЦІ</h2>
            <span className="badge badge-danger">{exiled.length} гравців</span>
          </div>
          {exiled.map(player => (
            <PlayerSummary key={player.id} player={player} highlight={false} />
          ))}
        </section>
      )}

      <div style={styles.footer}>
        <button className="btn btn-outline btn-lg" onClick={onNewGame}>
          Нова гра
        </button>
      </div>
    </div>
  );
}

function PlayerSummary({ player, highlight }: { player: Player; highlight: boolean }) {
  return (
    <div style={{ ...summaryStyles.row, ...(highlight ? summaryStyles.rowHighlight : summaryStyles.rowExiled) }}>
      <div style={summaryStyles.name}>
        {highlight ? '✅ ' : '☠ '}{player.name}
      </div>
      <div style={summaryStyles.cards}>
        {player.cards.map(card => {
          if (!card.revealed || !card.content) return null;
          const color = CARD_TYPE_COLOR[card.type];
          return (
            <span key={card.id} style={{ ...summaryStyles.cardChip, background: color + '33', borderColor: color }}>
              <span style={{ fontSize: '0.55rem', color, fontWeight: 700, letterSpacing: 1 }}>
                {CARD_TYPE_UA[card.type]}
              </span>
              <span style={{ fontSize: '0.75rem', fontWeight: 700, color: 'var(--text)' }}>
                {card.content}
              </span>
            </span>
          );
        })}
      </div>
    </div>
  );
}

const styles: Record<string, React.CSSProperties> = {
  page: {
    minHeight: '100vh',
    background: 'var(--bg)',
    padding: '1rem',
    maxWidth: 680,
    margin: '0 auto',
  },
  hero: {
    textAlign: 'center',
    padding: '2rem 0 1.5rem',
  },
  heroIcon: {
    fontSize: '3.5rem',
    marginBottom: '0.5rem',
  },
  heroTitle: {
    fontSize: '3rem',
    fontWeight: 700,
    color: 'var(--gold)',
    letterSpacing: '6px',
    marginBottom: '0.5rem',
  },
  heroSub: {
    color: 'var(--text-dim)',
    fontSize: '1rem',
  },
  section: {
    background: 'var(--bg2)',
    border: '1px solid var(--border)',
    borderRadius: 'var(--radius-lg)',
    padding: '1.25rem',
    marginBottom: '1rem',
  },
  sectionTitle: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.5rem',
    marginBottom: '1rem',
  },
  catCard: {
    background: 'rgba(180,50,0,0.12)',
    border: '1px solid rgba(255,138,101,0.3)',
    borderRadius: 'var(--radius-lg)',
    padding: '1.25rem',
    marginBottom: '1rem',
  },
  bunkerGrid: {
    display: 'flex',
    flexWrap: 'wrap',
    gap: '0.5rem',
  },
  bunkerSlot: {
    background: '#1e2e1e',
    border: '1px solid #2e5e2e',
    borderRadius: 'var(--radius)',
    color: '#a0d8a0',
    fontSize: '0.75rem',
    fontWeight: 700,
    padding: '0.4rem 0.7rem',
    textTransform: 'uppercase',
  },
  footer: {
    textAlign: 'center',
    padding: '2rem 0',
  },
};

const summaryStyles: Record<string, React.CSSProperties> = {
  row: {
    borderRadius: 'var(--radius)',
    marginBottom: '0.6rem',
    padding: '0.6rem 0.75rem',
  },
  rowHighlight: {
    background: 'rgba(67,160,71,0.1)',
    border: '1px solid rgba(67,160,71,0.3)',
  },
  rowExiled: {
    background: 'rgba(120,0,0,0.15)',
    border: '1px solid rgba(120,0,0,0.3)',
    opacity: 0.8,
  },
  name: {
    fontWeight: 700,
    fontSize: '0.95rem',
    marginBottom: '0.5rem',
  },
  cards: {
    display: 'flex',
    flexWrap: 'wrap',
    gap: '0.35rem',
  },
  cardChip: {
    display: 'inline-flex',
    flexDirection: 'column',
    gap: '0.1rem',
    border: '1px solid',
    borderRadius: 4,
    padding: '0.25rem 0.4rem',
    minWidth: 70,
  },
};
