# Бункер — фронтенд

React + TypeScript + Vite SPA для настільної гри "Бункер". Взаємодіє з back-end через REST API (проксі Vite → `http://localhost:3001`). Детальний контракт API — у [API_CONTRACT.md](./API_CONTRACT.md).

## Запуск

```bash
npm install
npm run dev      # dev-сервер + проксі на localhost:3001
npm run build    # tsc + vite build → dist/
```

## Структура

```
src/
  main.tsx          — точка входу, монтує <App />
  App.tsx           — кореневий компонент, маршрутизація між екранами
  api.ts            — HTTP-клієнт (всі fetch-виклики)
  hooks.ts          — useGameState (polling), useActionWithRefresh
  types.ts          — всі TypeScript-типи та enum-и
  utils.ts          — чисті утиліти, таблиця раундів ROUND_TABLE
  components/
    Lobby.tsx         — екран входу (створити / приєднатись)
    WaitingRoom.tsx   — зал очікування (phase=waiting)
    GameBoard.tsx     — основна ігрова дошка
    GameCard.tsx      — картка гравця (тип, зміст, стан)
    PlayerRow.tsx     — рядок гравця у списку
    PlayerHand.tsx    — рука гравця (його 6 карток)
    VotingPanel.tsx   — панель голосування
    ExileReveal.tsx   — оголошення вигнанця
    FinalScreen.tsx   — фінальний екран (phase=final)
```

## Ключові концепції

**Ідентифікація.** Після `createGame`/`joinGame` back-end повертає `playerId`. Він зберігається в `localStorage` (`bunker_player_id`) і передається в кожному запиті через заголовок `X-Player-Id`. `gameId` теж зберігається в `localStorage` (`bunker_game_id`).

**Polling.** `useGameState` опитує `/api/games/:gameId/state` кожні 2 с (константа `POLL_INTERVAL`). `refresh()` — примусовий запит після будь-якої дії.

**Видимість карток.** Back-end повертає `content: null` для карток, які ще не відкриті або належать іншим гравцям. `specialCard` є тільки у власного персонажа.

**Фази гри.** Порядок визначається back-end: `waiting → bunker_explore → card_reveal → voting → tie_break → exile_reveal → ... → final`. Компоненти рендеряться залежно від `state.phase`.

**Таблиця раундів.** `ROUND_TABLE` у `utils.ts` — кількість голосувань у кожному раунді залежно від числа гравців (4–16). Дублює таблицю з `API_CONTRACT.md`; back-end є авторитетним джерелом.

## Типи та утиліти

| Символ | Файл | Призначення |
|---|---|---|
| `GameState` | `types.ts` | повний стан гри (відповідь `/state`) |
| `Player`, `Card`, `BunkerCard` | `types.ts` | доменні моделі |
| `GamePhase` | `types.ts` | union-тип фаз |
| `CARD_TYPE_UA`, `CARD_TYPE_COLOR` | `types.ts` | локалізація та кольори карток |
| `PHASE_LABEL` | `types.ts` | локалізовані назви фаз |
| `api.*` | `api.ts` | типізовані виклики API |
| `getMyPlayer`, `canIVote`, … | `utils.ts` | похідний стан |

## Стек

- React 18, TypeScript 5, Vite 5
- Без зовнішніх UI-бібліотек; стилі — CSS-змінні у `index.css`
- Vite-проксі замість CORS (налаштовано у `vite.config.ts`)
