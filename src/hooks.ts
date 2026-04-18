import { useCallback, useEffect, useRef, useState } from 'react';
import { api } from './api';
import type { GameState } from './types';

const POLL_INTERVAL = 2000;

export function useGameState(gameId: string | null) {
  const [state, setState] = useState<GameState | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const intervalRef = useRef<ReturnType<typeof setInterval> | null>(null);

  const fetchState = useCallback(async () => {
    if (!gameId) return;
    try {
      const data = await api.getState(gameId);
      setState(data);
      setError(null);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Помилка сервера');
    }
  }, [gameId]);

  useEffect(() => {
    if (!gameId) {
      setState(null);
      return;
    }
    setLoading(true);
    fetchState().finally(() => setLoading(false));
    intervalRef.current = setInterval(fetchState, POLL_INTERVAL);
    return () => {
      if (intervalRef.current) clearInterval(intervalRef.current);
    };
  }, [gameId, fetchState]);

  return { state, error, loading, refresh: fetchState };
}

export function useActionWithRefresh(gameId: string | null, refresh: () => Promise<void>) {
  const [busy, setBusy] = useState(false);
  const [actionError, setActionError] = useState<string | null>(null);

  async function doAction(fn: () => Promise<void>) {
    if (!gameId) return;
    setBusy(true);
    setActionError(null);
    try {
      await fn();
      await refresh();
    } catch (e) {
      setActionError(e instanceof Error ? e.message : 'Помилка');
    } finally {
      setBusy(false);
    }
  }

  return { busy, actionError, doAction };
}
