export type CardType = 'profession' | 'biology' | 'health' | 'hobby' | 'baggage' | 'facts';

export const CARD_TYPE_UA: Record<CardType, string> = {
  profession: 'ПРОФЕСІЯ',
  biology: 'БІОЛОГІЯ',
  health: "ЗДОРОВʼЯ",
  hobby: 'ХОБІ',
  baggage: 'БАГАЖ',
  facts: 'ФАКТИ',
};

export const CARD_TYPE_COLOR: Record<CardType, string> = {
  profession: '#1565c0',
  biology:   '#c8a200',
  health:    '#b71c1c',
  hobby:     '#1b5e20',
  baggage:   '#bf360c',
  facts:     '#4a148c',
};

export interface Card {
  id: string;
  type: CardType;
  content: string | null; // null = закрита (не ваша або ще не розкрита)
  revealed: boolean;
}

export interface SpecialCard {
  id: string;
  title: string;
  description: string;
  played: boolean;
}

export interface Player {
  id: string;
  name: string;
  cards: Card[];
  specialCard: SpecialCard | null; // null = не ваша картка (прихована)
  isExiled: boolean;
  isSpeaking: boolean; // чия зараз черга відкривати картку
}

export interface BunkerCard {
  id: string;
  slot: number; // 1–5
  content: string | null; // null = ще не відкрита
  revealed: boolean;
}

export interface Catastrophe {
  id: string;
  title: string;
  description: string;
}

export type GamePhase =
  | 'waiting'
  | 'bunker_explore'
  | 'card_reveal'
  | 'voting'
  | 'tie_break'
  | 'exile_reveal'
  | 'final';

export const PHASE_LABEL: Record<GamePhase, string> = {
  waiting:      'Очікування гравців',
  bunker_explore: 'Дослідження бункера',
  card_reveal:  'Відкривання карток',
  voting:       'Голосування',
  tie_break:    'Повторне голосування',
  exile_reveal: 'Оголошення вигнанця',
  final:        'Фінал',
};

export interface VoteRecord {
  voterId: string;
  voterName: string;
  targetId: string | null; // null = ще не проголосував
}

export interface GameState {
  id: string;
  code: string;              // короткий код для підключення (напр. "AB12")
  phase: GamePhase;
  round: number;             // 1–5
  votingInRound: number;     // яке це голосування у раунді (1 або 2)
  maxVotingsInRound: number; // з таблиці раундів
  players: Player[];
  activeSpeakerId: string | null; // хто зараз говорить
  catastrophe: Catastrophe;
  bunkerCards: BunkerCard[];
  votes: VoteRecord[];       // голоси поточного голосування
  myVote: string | null;     // ID кандидата, за якого проголосував поточний гравець
  exiledPlayerId: string | null;
  bunkerCapacity: number;    // скільки потрапить у бункер
  isHost: boolean;
  myId: string;              // ID поточного гравця
}

// ---- API result shapes ----

export interface CreateGameResult {
  gameId: string;
  playerId: string;
  code: string;
}

export interface JoinGameResult {
  gameId: string;
  playerId: string;
}
