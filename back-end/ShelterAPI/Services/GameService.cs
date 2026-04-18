using Microsoft.EntityFrameworkCore;
using ShelterAPI.Data;
using ShelterAPI.DTOs.Responses;
using ShelterAPI.Enums;
using ShelterAPI.Models;
using ShelterAPI.Models.GameEntity;

namespace ShelterAPI.Services
{
    public class GameService(
        BunkerDbContext db,
        IRoundTableService roundTable,
        ICardDealingService cardDealing,
        IVotingService voting) : IGameService
    {
        // ─── CREATE / JOIN ──────────────────────────────────────────────────

        public async Task<CreateGameResponse> CreateGameAsync(string hostName, CancellationToken ct = default)
        {
            var catastrophe = SeedData.Catastrophes[new Random().Next(SeedData.Catastrophes.Count)];

            // Генеруємо playerId наперед, щоб передати в GameEntity.Create як hostId
            var playerId = Guid.NewGuid();

            var game = GameEntity.Create(playerId, catastrophe.Id, 0);

            var host = PlayerEntity.Create(game.Id, hostName, 0);
            host.Id = playerId; // закріплюємо наш Id замість автозгенерованого

            // Game first (HostId is plain Guid — no FK), then Player (GameId → Games.Id)
            await db.Games.AddAsync(game, ct);
            await db.SaveChangesAsync(ct);

            await db.Players.AddAsync(host, ct);
            await db.SaveChangesAsync(ct);

            return new CreateGameResponse(game.Id, playerId, game.Code);
        }

        public async Task<JoinGameResponse> JoinGameAsync(string code, string playerName, CancellationToken ct = default)
        {
            var game = await db.Games
                .Include(g => g.Players)
                .FirstOrDefaultAsync(g => g.Code == code.ToUpper(), ct)
                ?? throw new KeyNotFoundException("Гру не знайдено.");

            if (game.Phase != GamePhase.Waiting)
                throw new InvalidOperationException("Гра вже розпочалась.");

            if (game.Players.Count >= 16)
                throw new InvalidOperationException("Гра переповнена.");

            var player = PlayerEntity.Create(game.Id, playerName, game.Players.Count);

            await db.Players.AddAsync(player, ct);
            await db.SaveChangesAsync(ct);

            return new JoinGameResponse(game.Id, player.Id);
        }

        // ─── GET STATE ──────────────────────────────────────────────────────

        public async Task<GameStateResponse> GetStateAsync(Guid gameId, Guid playerId, CancellationToken ct = default)
        {
            var game = await db.Games
                .Include(g => g.Catastrophe)
                .Include(g => g.Players)
                    .ThenInclude(p => p.Cards)
                .Include(g => g.Players)
                    .ThenInclude(p => p.SpecialCard)
                .Include(g => g.BunkerCards)
                .Include(g => g.Votes)
                    .ThenInclude(v => v.Voter)
                .FirstOrDefaultAsync(g => g.Id == gameId, ct)
                ?? throw new KeyNotFoundException("Гру не знайдено.");

            var maxVotings = roundTable.GetVotingsInRound(game.Players.Count, game.Round);

            var currentVotes = game.Votes
                .Where(v => v.Round == game.Round && v.VotingInRound == game.VotingInRound)
                .ToList();

            var myVote = currentVotes.FirstOrDefault(v => v.VoterId == playerId)?.TargetId;

            var revealAllForExiled = game.Phase is GamePhase.ExileReveal or GamePhase.Final;

            var playerDtos = game.Players
                .OrderBy(p => p.SeatOrder)
                .Select(p => new PlayerDto(
                    Id: p.Id,
                    Name: p.Name,
                    Cards: p.Cards.Select(c => new CardDto(
                        Id: c.Id,
                        Type: c.Type,
                        Content: (c.Revealed || p.Id == playerId || (revealAllForExiled && p.Id == game.ExiledPlayerId)) ? c.Content : null,
                        Revealed: c.Revealed
                    )).ToList(),
                    SpecialCard: p.Id == playerId && p.SpecialCard != null
                        ? new SpecialCardDto(p.SpecialCard.Id, p.SpecialCard.Title, p.SpecialCard.Description, p.SpecialCard.Played)
                        : null,
                    IsExiled: p.IsExiled,
                    IsSpeaking: p.Id == game.ActiveSpeakerId
                )).ToList();

            var bunkerDtos = game.BunkerCards
                .OrderBy(b => b.Slot)
                .Select(b => new BunkerCardDto(b.Id, b.Slot, b.Revealed ? b.Content : null, b.Revealed))
                .ToList();

            var voteDtos = currentVotes
                .Select(v => new VoteRecordDto(v.VoterId, v.Voter!.Name, v.TargetId))
                .ToList();

            return new GameStateResponse(
                Id: game.Id,
                Code: game.Code,
                Phase: PhaseToString(game.Phase),
                Round: game.Round,
                VotingInRound: game.VotingInRound,
                MaxVotingsInRound: maxVotings,
                Players: playerDtos,
                ActiveSpeakerId: game.ActiveSpeakerId,
                Catastrophe: new CatastropheDto(game.Catastrophe!.Id, game.Catastrophe.Title, game.Catastrophe.Description),
                BunkerCards: bunkerDtos,
                Votes: voteDtos,
                MyVote: myVote,
                ExiledPlayerId: game.ExiledPlayerId,
                BunkerCapacity: game.BunkerCapacity,
                IsHost: game.HostId == playerId,
                MyId: playerId
            );
        }

        // ─── ACTIONS ────────────────────────────────────────────────────────

        public async Task StartGameAsync(Guid gameId, Guid playerId, CancellationToken ct = default)
        {
            var game = await LoadWithPlayersAsync(gameId, ct);

            if (game.HostId != playerId)
                throw new UnauthorizedAccessException("Тільки ведучий може розпочати гру.");

            if (game.Phase != GamePhase.Waiting)
                throw new InvalidOperationException("Гра вже розпочалась.");

            if (game.Players.Count < 4)
                throw new InvalidOperationException("Мінімум 4 гравці для старту.");

            var capacity = roundTable.GetBunkerCapacity(game.Players.Count);
            game.SetBunkerCapacity(capacity);

            var players = game.Players.OrderBy(p => p.SeatOrder).ToList();
            await cardDealing.DealAsync(game.Id, players, ct);

            var firstPlayer = players.First();
            game.SetActiveSpeaker(firstPlayer.Id);
            game.SetPhase(GamePhase.BunkerExplore);

            await db.SaveChangesAsync(ct);
        }

        public async Task RevealBunkerCardAsync(Guid gameId, Guid playerId, CancellationToken ct = default)
        {
            var game = await db.Games
                .Include(g => g.Players)
                .Include(g => g.BunkerCards)
                .FirstOrDefaultAsync(g => g.Id == gameId, ct)
                ?? throw new KeyNotFoundException("Гру не знайдено.");

            if (game.Phase != GamePhase.BunkerExplore)
                throw new InvalidOperationException("Зараз не фаза дослідження бункера.");

            var isHost = game.HostId == playerId;
            var isActiveSpeaker = game.ActiveSpeakerId == playerId;
            if (!isHost && !isActiveSpeaker)
                throw new UnauthorizedAccessException("Тільки активний гравець або ведучий може відкрити картку бункера.");

            // Якщо ще є нерозкриті картки — відкриваємо наступну. Інакше просто переходимо далі.
            var nextCard = game.BunkerCards
                .Where(b => !b.Revealed)
                .OrderBy(b => b.Slot)
                .FirstOrDefault();

            nextCard?.Reveal();

            // Визначаємо першого гравця для фази card_reveal
            var firstActive = game.Players
                .Where(p => !p.IsExiled)
                .OrderBy(p => p.SeatOrder)
                .First();

            game.SetActiveSpeaker(firstActive.Id);
            game.SetPhase(GamePhase.CardReveal);

            await db.SaveChangesAsync(ct);
        }

        public async Task RevealCardAsync(Guid gameId, Guid playerId, Guid cardId, CancellationToken ct = default)
        {
            var game = await db.Games
                .Include(g => g.Players)
                    .ThenInclude(p => p.Cards)
                .FirstOrDefaultAsync(g => g.Id == gameId, ct)
                ?? throw new KeyNotFoundException("Гру не знайдено.");

            if (game.Phase != GamePhase.CardReveal)
                throw new InvalidOperationException("Зараз не фаза відкривання карток.");

            if (game.ActiveSpeakerId != playerId)
                throw new UnauthorizedAccessException("Зараз не ваша черга.");

            var player = game.Players.First(p => p.Id == playerId);
            var card = player.Cards.FirstOrDefault(c => c.Id == cardId)
                ?? throw new KeyNotFoundException("Картку не знайдено.");

            if (card.Revealed)
                throw new InvalidOperationException("Ця картка вже відкрита.");

            // У раунді 1 можна відкривати тільки profession
            if (game.Round == 1 && card.Type != "profession")
                throw new InvalidOperationException("У першому раунді потрібно відкрити картку Професії.");

            card.Reveal();

            var activePlayers = game.Players.Where(p => !p.IsExiled).OrderBy(p => p.SeatOrder).ToList();
            var allRevealed = activePlayers.All(p => p.Cards.Count(c => c.Revealed) >= game.Round);

            if (!allRevealed)
            {
                // Передаємо хід наступному гравцю, який ще не відкрив картку в цьому раунді
                var nextSpeaker = activePlayers
                    .FirstOrDefault(p => p.Id != playerId && p.Cards.Count(c => c.Revealed) < game.Round);
                game.SetActiveSpeaker(nextSpeaker?.Id);
            }
            else
            {
                TransitionAfterCardReveal(game, activePlayers);
            }

            await db.SaveChangesAsync(ct);
        }

        public async Task VoteAsync(Guid gameId, Guid playerId, Guid targetId, CancellationToken ct = default)
        {
            var game = await db.Games
                .Include(g => g.Players)
                .Include(g => g.Votes)
                .FirstOrDefaultAsync(g => g.Id == gameId, ct)
                ?? throw new KeyNotFoundException("Гру не знайдено.");

            if (game.Phase != GamePhase.Voting && game.Phase != GamePhase.TieBreak)
                throw new InvalidOperationException("Зараз не фаза голосування.");

            var alreadyVoted = game.Votes.Any(v =>
                v.Round == game.Round &&
                v.VotingInRound == game.VotingInRound &&
                v.VoterId == playerId);

            if (alreadyVoted)
                throw new InvalidOperationException("Ви вже проголосували.");

            var target = game.Players.FirstOrDefault(p => p.Id == targetId && !p.IsExiled)
                ?? throw new KeyNotFoundException("Кандидата не знайдено серед активних гравців.");

            var vote = VoteEntity.Create(gameId, game.Round, game.VotingInRound, playerId, targetId);
            await db.Votes.AddAsync(vote, ct);
            await db.SaveChangesAsync(ct);

            // Перевіряємо — всі гравці (включно з вигнаними) вже проголосували
            if (!await voting.AllVotedAsync(gameId, game.Round, game.VotingInRound, ct))
                return;

            // Всі проголосували — підраховуємо
            var result = await voting.CountVotesAsync(gameId, game.Round, game.VotingInRound, ct);

            if (!result.IsTie)
            {
                var exiledPlayer = game.Players.First(p => p.Id == result.WinnerId);
                exiledPlayer.Exile();
                game.SetExiled(exiledPlayer.Id);
                game.SetPhase(GamePhase.ExileReveal);
            }
            else
            {
                var maxVotings = roundTable.GetVotingsInRound(game.Players.Count, game.Round);

                if (game.VotingInRound < maxVotings)
                {
                    game.AdvanceVotingInRound();
                    game.SetPhase(GamePhase.TieBreak);
                }
                else
                {
                    // Нічия після всіх переголосувань — рандомний вигнанець серед лідерів
                    var tally = game.Votes
                        .Where(v => v.Round == game.Round && v.VotingInRound == game.VotingInRound && v.TargetId != null)
                        .GroupBy(v => v.TargetId!.Value)
                        .Select(g => new { Id = g.Key, Count = g.Count() })
                        .ToList();

                    var maxCount = tally.Max(x => x.Count);
                    var candidates = tally.Where(x => x.Count == maxCount).Select(x => x.Id).ToList();
                    var randomId = candidates[new Random().Next(candidates.Count)];

                    var exiledPlayer = game.Players.First(p => p.Id == randomId);
                    exiledPlayer.Exile();
                    game.SetExiled(exiledPlayer.Id);
                    game.SetPhase(GamePhase.ExileReveal);
                }
            }

            await db.SaveChangesAsync(ct);
        }

        public async Task NextPhaseAsync(Guid gameId, Guid playerId, CancellationToken ct = default)
        {
            var game = await LoadWithPlayersAsync(gameId, ct);

            if (game.HostId != playerId)
                throw new UnauthorizedAccessException("Тільки ведучий може просунути фазу.");

            switch (game.Phase)
            {
                case GamePhase.ExileReveal:
                    if (game.Round < 5)
                    {
                        game.AdvanceRound();
                        var firstActive = game.Players
                            .Where(p => !p.IsExiled)
                            .OrderBy(p => p.SeatOrder)
                            .First();
                        game.SetActiveSpeaker(firstActive.Id);
                        game.SetPhase(GamePhase.BunkerExplore);
                    }
                    else
                    {
                        game.SetPhase(GamePhase.Final);
                        game.SetActiveSpeaker(null);
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Не можна вручну просунути фазу з '{PhaseToString(game.Phase)}'.");
            }

            await db.SaveChangesAsync(ct);
        }

        public async Task PlaySpecialCardAsync(Guid gameId, Guid playerId, Guid? targetId, CancellationToken ct = default)
        {
            var game = await db.Games
                .Include(g => g.Players)
                    .ThenInclude(p => p.Cards)
                .Include(g => g.Players)
                    .ThenInclude(p => p.SpecialCard)
                .FirstOrDefaultAsync(g => g.Id == gameId, ct)
                ?? throw new KeyNotFoundException("Гру не знайдено.");

            var player = game.Players.FirstOrDefault(p => p.Id == playerId)
                ?? throw new KeyNotFoundException("Гравця не знайдено.");

            var special = player.SpecialCard
                ?? throw new InvalidOperationException("У вас немає спеціальної картки.");

            if (special.Played)
                throw new InvalidOperationException("Спеціальну картку вже зіграно.");

            var isMyTurn = game.Phase == GamePhase.CardReveal && game.ActiveSpeakerId == playerId;
            var isAboutToBeExiled = game.Phase == GamePhase.ExileReveal && game.ExiledPlayerId == playerId;
            if (!isMyTurn && !isAboutToBeExiled)
                throw new InvalidOperationException("Спеціальну картку можна використати лише під час вашого ходу або перед вигнанням.");

            await ApplySpecialEffectAsync(game, player, special, targetId, ct);

            special.Play();
            await db.SaveChangesAsync(ct);
        }

        // ─── HELPERS ────────────────────────────────────────────────────────

        private async Task<GameEntity> LoadWithPlayersAsync(Guid gameId, CancellationToken ct)
        {
            return await db.Games
                .Include(g => g.Players)
                .FirstOrDefaultAsync(g => g.Id == gameId, ct)
                ?? throw new KeyNotFoundException("Гру не знайдено.");
        }

        private void TransitionAfterCardReveal(GameEntity game, List<PlayerEntity> activePlayers)
        {
            var maxVotings = roundTable.GetVotingsInRound(game.Players.Count, game.Round);

            if (maxVotings > 0)
            {
                game.SetPhase(GamePhase.Voting);
                game.SetActiveSpeaker(null);
            }
            else if (game.Round < 5)
            {
                game.AdvanceRound();
                game.SetPhase(GamePhase.BunkerExplore);
                game.SetActiveSpeaker(activePlayers.First().Id);
            }
            else
            {
                game.SetPhase(GamePhase.Final);
                game.SetActiveSpeaker(null);
            }
        }

        private async Task ApplySpecialEffectAsync(
            GameEntity game, PlayerEntity player, SpecialCardEntity special,
            Guid? targetId, CancellationToken ct)
        {
            switch (special.Title)
            {
                case "Амністія":
                {
                    // Повертає одного вигнаного гравця
                    var exiled = game.Players.FirstOrDefault(p => p.IsExiled && p.Id == targetId)
                        ?? throw new InvalidOperationException("Вкажіть вигнаного гравця для Амністії.");
                    exiled.Restore();
                    break;
                }

                case "Викривач":
                {
                    // Примусово відкриває одну закриту картку цільового гравця
                    if (targetId == null)
                        throw new InvalidOperationException("Вкажіть гравця для Викривача.");

                    var target = game.Players.FirstOrDefault(p => p.Id == targetId)
                        ?? throw new KeyNotFoundException("Гравця не знайдено.");

                    var hiddenCard = target.Cards.FirstOrDefault(c => !c.Revealed)
                        ?? throw new InvalidOperationException("У цього гравця немає закритих карток.");

                    hiddenCard.Reveal();
                    break;
                }

                case "Маніпулятор":
                {
                    // Скасовує поточне голосування — видаляємо голоси цього раунду
                    var votesToRemove = await db.Votes
                        .Where(v => v.GameId == game.Id
                                 && v.Round == game.Round
                                 && v.VotingInRound == game.VotingInRound)
                        .ToListAsync(ct);
                    db.Votes.RemoveRange(votesToRemove);
                    game.SetPhase(GamePhase.Voting);
                    break;
                }

                case "Переворот":
                {
                    // Новим ведучим стає гравець з найбільшою кількістю відкритих карток
                    var newHost = game.Players
                        .Where(p => !p.IsExiled)
                        .OrderByDescending(p => p.Cards.Count(c => c.Revealed))
                        .First();
                    game.SetHost(newHost.Id);
                    break;
                }

                // Картки що не змінюють стан гри — ефект UI-сторони або вже відображений через State
                case "Ясновидець":
                case "Захисник":
                case "Подвійний голос":
                case "Імунітет":
                case "Саботажник":
                case "Детектор брехні":
                case "Перетасування":
                case "Альянс":
                case "Компромат":
                case "Жертва":
                case "Обмін долями":
                    break;

                default:
                    break;
            }
        }

        private static string PhaseToString(GamePhase phase) => phase switch
        {
            GamePhase.Waiting       => "waiting",
            GamePhase.BunkerExplore => "bunker_explore",
            GamePhase.CardReveal    => "card_reveal",
            GamePhase.Voting        => "voting",
            GamePhase.TieBreak      => "tie_break",
            GamePhase.ExileReveal   => "exile_reveal",
            GamePhase.Final         => "final",
            _ => phase.ToString().ToLower()
        };

        private static string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rng = new Random();
            return new string(Enumerable.Range(0, 6).Select(_ => chars[rng.Next(chars.Length)]).ToArray());
        }
    }
}
