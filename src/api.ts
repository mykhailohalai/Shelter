/**
 * API-шар для гри "Бункер".
 *
 * Всі запити йдуть на /api/* (проксі → http://localhost:3001).
 * Ідентифікатор гравця передається у заголовку X-Player-Id.
 *
 * Очікуваний контракт back-end:
 *
 * POST /api/games
 *   Body:  { hostName: string }
 *   Result: { gameId, playerId, code }
 *
 * POST /api/games/join
 *   Body:  { code: string, playerName: string }
 *   Result: { gameId, playerId }
 *
 * GET  /api/games/:gameId/state
 *   Header: X-Player-Id
 *   Result: GameState  (картки повертаються відповідно до ролі гравця)
 *
 * POST /api/games/:gameId/actions/start
 *   Header: X-Player-Id (тільки хост)
 *
 * POST /api/games/:gameId/actions/reveal-bunker
 *   Header: X-Player-Id (активний гравець або хост)
 *
 * POST /api/games/:gameId/actions/reveal-card
 *   Header: X-Player-Id
 *   Body:  { cardId: string }  (у 1-му раунді back-end сам визначає що це Професія)
 *
 * POST /api/games/:gameId/actions/vote
 *   Header: X-Player-Id
 *   Body:  { targetId: string }
 *
 * POST /api/games/:gameId/actions/next-phase
 *   Header: X-Player-Id (хост або авто)
 *
 * POST /api/games/:gameId/actions/play-special
 *   Header: X-Player-Id
 *   Body:  { targetId?: string }  — залежить від картки
 */

import type { GameState, CreateGameResult, JoinGameResult } from './types';

const BASE = (import.meta.env.VITE_API_URL ?? '') + '/api';

function playerId(): string | null {
  return localStorage.getItem('bunker_player_id');
}

function headers(): HeadersInit {
  const pid = playerId();
  return {
    'Content-Type': 'application/json',
    ...(pid ? { 'X-Player-Id': pid } : {}),
  };
}

async function req<T>(method: string, path: string, body?: unknown): Promise<T> {
  const res = await fetch(`${BASE}${path}`, {
    method,
    headers: headers(),
    body: body !== undefined ? JSON.stringify(body) : undefined,
  });
  if (!res.ok) {
    const payload = await res.json().catch(() => ({}));
    throw new Error((payload as { message?: string }).message ?? `HTTP ${res.status}`);
  }
  return res.json() as Promise<T>;
}

export const api = {
  createGame:       (hostName: string) =>
    req<CreateGameResult>('POST', '/games', { hostName }),

  joinGame:         (code: string, playerName: string) =>
    req<JoinGameResult>('POST', '/games/join', { code, playerName }),

  getState:         (gameId: string) =>
    req<GameState>('GET', `/games/${gameId}/state`),

  startGame:        (gameId: string) =>
    req<void>('POST', `/games/${gameId}/actions/start`),

  revealBunkerCard: (gameId: string) =>
    req<void>('POST', `/games/${gameId}/actions/reveal-bunker`),

  revealCard:       (gameId: string, cardId: string) =>
    req<void>('POST', `/games/${gameId}/actions/reveal-card`, { cardId }),

  castVote:         (gameId: string, targetId: string) =>
    req<void>('POST', `/games/${gameId}/actions/vote`, { targetId }),

  nextPhase:        (gameId: string) =>
    req<void>('POST', `/games/${gameId}/actions/next-phase`),

  playSpecialCard:  (gameId: string, targetId?: string) =>
    req<void>('POST', `/games/${gameId}/actions/play-special`, { targetId }),
};
