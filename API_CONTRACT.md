# API Contract — Гра "Бункер"

Фронтенд робить запити на `/api/*` (Vite проксіює на `http://localhost:3001`).
Кожен запит від гравця містить заголовок `X-Player-Id`.

---

## Ідентифікація гравця

Після `createGame` або `joinGame` back-end повертає `playerId` (UUID).
Клієнт зберігає його в `localStorage` і передає у всіх наступних запитах:

```
X-Player-Id: <playerId>
```

---

## Ендпоінти

### POST /api/games
Створити нову гру. Ведучий автоматично стає першим гравцем.

```json
// Request
{ "hostName": "Андрій" }

// Response 201
{ "gameId": "abc123", "playerId": "uuid-host", "code": "AB12" }
```

---

### POST /api/games/join
Приєднатися до гри за кодом.

```json
// Request
{ "code": "AB12", "playerName": "Марія" }

// Response 200
{ "gameId": "abc123", "playerId": "uuid-player" }
```

Помилки: `404` якщо гра не знайдена, `400` якщо гра вже стартувала або переповнена.

---

### GET /api/games/:gameId/state
Отримати поточний стан гри. **Відповідь залежить від `X-Player-Id`**: картки інших гравців
повертаються з `content: null` якщо вони ще не розкриті.

```typescript
// Response 200 — GameState
{
  id: string,
  code: string,
  phase: "waiting" | "bunker_explore" | "card_reveal" | "voting" | "tie_break" | "exile_reveal" | "final",
  round: number,           // 1–5
  votingInRound: number,   // поточне голосування у раунді (1 або 2)
  maxVotingsInRound: number,
  players: Player[],
  activeSpeakerId: string | null,  // хто зараз говорить/ходить
  catastrophe: { id, title, description },
  bunkerCards: BunkerCard[],  // slot 1–5
  votes: VoteRecord[],         // поточні голоси
  myVote: string | null,       // ID кандидата, за якого проголосував цей гравець
  exiledPlayerId: string | null,
  bunkerCapacity: number,
  isHost: boolean,
  myId: string
}

// Player shape
{
  id: string,
  name: string,
  cards: Card[],           // 6 карток; content=null якщо закрита і не ваша
  specialCard: SpecialCard | null,  // null якщо не ваш персонаж
  isExiled: boolean,
  isSpeaking: boolean
}

// Card shape
{ id, type: "profession"|"biology"|"health"|"hobby"|"baggage"|"facts", content: string|null, revealed: boolean }

// BunkerCard shape
{ id, slot: 1..5, content: string|null, revealed: boolean }

// VoteRecord shape
{ voterId, voterName, targetId: string|null }
```

---

### POST /api/games/:gameId/actions/start
Розпочати гру (тільки ведучий). Роздає картки і переводить у фазу `bunker_explore` раунду 1.

---

### POST /api/games/:gameId/actions/reveal-bunker
Відкрити наступну закриту картку бункера. Виконує активний гравець або ведучий.
Після відкриття back-end переводить у фазу `card_reveal`.

---

### POST /api/games/:gameId/actions/reveal-card
Гравець відкриває одну зі своїх карток.

```json
// Request
{ "cardId": "uuid" }
```

У раунді 1 back-end перевіряє що розкривається саме `profession`.
Після того як всі активні гравці відкрили картку → перейти до `voting` (якщо є) або до `bunker_explore` наступного раунду.

---

### POST /api/games/:gameId/actions/vote
Гравець голосує за кандидата на вигнання. Вигнанці теж голосують.

```json
// Request
{ "targetId": "uuid" }
```

Коли всі проголосували:
- Якщо є явний лідер → `exile_reveal`
- Якщо нічия → фаза `tie_break`, ті самі ендпоінти

---

### POST /api/games/:gameId/actions/next-phase
Ведучий вручну просуває гру (напр. після `exile_reveal`, після `card_reveal` якщо всі відкрили).
Back-end визначає наступну фазу з таблиці раундів.

---

### POST /api/games/:gameId/actions/play-special
Розіграти картку Особливих Умов.

```json
// Request (опційно)
{ "targetId": "uuid" }
```

---

## Таблиця раундів (кількість голосувань)

| Гравців | Р1 | Р2 | Р3 | Р4 | Р5 | Вигнано | Бункер |
|---------|----|----|----|----|----|---------| -------|
| 4       | 0  | 0  | 0  | 1  | 1  | 2       | 2      |
| 5       | 0  | 0  | 1  | 1  | 1  | 3       | 2      |
| 6       | 0  | 0  | 1  | 1  | 1  | 3       | 3      |
| 7       | 0  | 1  | 1  | 1  | 1  | 4       | 3      |
| 8       | 0  | 1  | 1  | 1  | 1  | 4       | 4      |
| 9       | 0  | 1  | 1  | 1  | 2  | 5       | 4      |
| 10      | 0  | 1  | 1  | 1  | 2  | 5       | 5      |
| 11      | 0  | 1  | 1  | 2  | 2  | 6       | 5      |
| 12      | 0  | 1  | 1  | 2  | 2  | 6       | 6      |
| 13      | 0  | 1  | 2  | 2  | 2  | 7       | 6      |
| 14      | 0  | 1  | 2  | 2  | 2  | 7       | 7      |
| 15      | 0  | 2  | 2  | 2  | 2  | 8       | 7      |
| 16      | 0  | 2  | 2  | 2  | 2  | 8       | 8      |

---

## Потік фаз (BaseMode)

```
waiting
  → (start) → bunker_explore [round 1]
  → (reveal bunker card) → card_reveal [round 1, кожен відкриває Професію]
  → (всі відкрили) → bunker_explore [round 2]
  → (reveal) → card_reveal [round 2]
  → (всі відкрили) → voting [якщо є за таблицею]
  → (всі проголосували) → exile_reveal
  → (next) → bunker_explore [round 3]
  ...
  → після 5 раундів → final
```
