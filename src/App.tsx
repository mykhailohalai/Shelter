import { useEffect, useState } from 'react';
import { Lobby } from './components/Lobby';
import { WaitingRoom } from './components/WaitingRoom';
import { GameBoard } from './components/GameBoard';
import { FinalScreen } from './components/FinalScreen';
import { useGameState } from './hooks';

type Screen = 'lobby' | 'game';

export default function App() {
  const [screen, setScreen] = useState<Screen>(() =>
    localStorage.getItem('bunker_game_id') ? 'game' : 'lobby',
  );
  const [gameId, setGameId] = useState<string | null>(() =>
    localStorage.getItem('bunker_game_id'),
  );

  // check URL for ?join=CODE to auto-populate join form
  const [autoJoinCode] = useState<string | null>(() => {
    const params = new URLSearchParams(window.location.search);
    return params.get('join');
  });

  const { state, error, loading, refresh } = useGameState(gameId);

  useEffect(() => {
    if (gameId) setScreen('game');
  }, [gameId]);

  function handleGameJoined(gId: string, pId: string) {
    localStorage.setItem('bunker_game_id', gId);
    localStorage.setItem('bunker_player_id', pId);
    setGameId(gId);
    setScreen('game');
    // clean URL
    window.history.replaceState({}, '', '/');
  }

  function handleLeaveGame() {
    localStorage.removeItem('bunker_game_id');
    localStorage.removeItem('bunker_player_id');
    setGameId(null);
    setScreen('lobby');
  }

  /* ── Lobby ─────────────────────────────────────────────── */
  if (screen === 'lobby') {
    return (
      <Lobby
        onGameJoined={handleGameJoined}
        defaultJoinCode={autoJoinCode ?? undefined}
      />
    );
  }

  /* ── Loading first fetch ───────────────────────────────── */
  if (loading && !state) {
    return (
      <div className="fullscreen-center">
        <div className="spinner" />
        <p>Підключення до гри…</p>
      </div>
    );
  }

  /* ── Connection error ──────────────────────────────────── */
  if (error && !state) {
    return (
      <div className="fullscreen-center">
        <h2 style={{ color: 'var(--danger)' }}>Помилка підключення</h2>
        <p>{error}</p>
        <p className="hint">Перевірте, що сервер запущений на порті 3001</p>
        <div style={{ display: 'flex', gap: '0.75rem', marginTop: '1rem' }}>
          <button className="btn btn-outline" onClick={() => refresh()}>
            Спробувати ще
          </button>
          <button className="btn btn-ghost" onClick={handleLeaveGame}>
            На головну
          </button>
        </div>
      </div>
    );
  }

  if (!state) return null;

  /* ── Final screen ──────────────────────────────────────── */
  if (state.phase === 'final') {
    return <FinalScreen state={state} onNewGame={handleLeaveGame} />;
  }

  /* ── Waiting room ──────────────────────────────────────── */
  if (state.phase === 'waiting') {
    return (
      <WaitingRoom
        state={state}
        gameId={gameId!}
        refresh={refresh}
        onLeave={handleLeaveGame}
      />
    );
  }

  /* ── Main game board ───────────────────────────────────── */
  return (
    <GameBoard
      state={state}
      gameId={gameId!}
      refresh={refresh}
      onLeave={handleLeaveGame}
    />
  );
}
