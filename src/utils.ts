import type { GameState, Player, Card } from './types';

// Таблиця голосувань: ROUND_TABLE[кількість гравців][раунд 0-4] = кількість голосувань
export const ROUND_TABLE: Record<number, readonly number[]> = {
  4:  [0, 0, 0, 1, 1],
  5:  [0, 0, 1, 1, 1],
  6:  [0, 0, 1, 1, 1],
  7:  [0, 1, 1, 1, 1],
  8:  [0, 1, 1, 1, 1],
  9:  [0, 1, 1, 1, 2],
  10: [0, 1, 1, 1, 2],
  11: [0, 1, 1, 2, 2],
  12: [0, 1, 1, 2, 2],
  13: [0, 1, 2, 2, 2],
  14: [0, 1, 2, 2, 2],
  15: [0, 2, 2, 2, 2],
  16: [0, 2, 2, 2, 2],
};

// Кількість місць у бункері
export const BUNKER_CAPACITY: Record<number, number> = {
  4: 2, 5: 2, 6: 3, 7: 3, 8: 4,
  9: 4, 10: 5, 11: 5, 12: 6, 13: 6,
  14: 7, 15: 7, 16: 8,
};

export function getMyPlayer(state: GameState): Player | undefined {
  return state.players.find(p => p.id === state.myId);
}

export function getActivePlayers(state: GameState): Player[] {
  return state.players.filter(p => !p.isExiled);
}

export function getExiledPlayers(state: GameState): Player[] {
  return state.players.filter(p => p.isExiled);
}

export function getUnrevealedCards(player: Player): Card[] {
  return player.cards.filter(c => !c.revealed && c.content !== null);
}

export function canIRevealCard(state: GameState): boolean {
  const me = getMyPlayer(state);
  return (
    state.phase === 'card_reveal' &&
    !!me &&
    !me.isExiled &&
    me.isSpeaking
  );
}

export function canIVote(state: GameState): boolean {
  return (
    (state.phase === 'voting' || state.phase === 'tie_break') &&
    state.myVote === null
  );
}

export function canPlaySpecial(state: GameState): boolean {
  const me = getMyPlayer(state);
  if (!me || !me.specialCard || me.specialCard.played) return false;
  const myTurnToSpeak = state.phase === 'card_reveal' && state.activeSpeakerId === me.id;
  const aboutToBeExiled = state.phase === 'exile_reveal' && state.exiledPlayerId === me.id;
  return myTurnToSpeak || aboutToBeExiled;
}

export function isMyTurnForBunker(state: GameState): boolean {
  return (
    state.phase === 'bunker_explore' &&
    (state.isHost || state.activeSpeakerId === state.myId)
  );
}

export function voteCountFor(state: GameState, playerId: string): number {
  return state.votes.filter(v => v.targetId === playerId).length;
}

export function allVotesCast(state: GameState): boolean {
  return state.votes.length > 0 &&
    state.votes.every(v => v.targetId !== null);
}

export function copyToClipboard(text: string): void {
  navigator.clipboard.writeText(text).catch(() => {
    const ta = document.createElement('textarea');
    ta.value = text;
    document.body.appendChild(ta);
    ta.select();
    document.execCommand('copy');
    document.body.removeChild(ta);
  });
}

export function joinUrl(code: string): string {
  return `${window.location.origin}/?join=${code}`;
}
