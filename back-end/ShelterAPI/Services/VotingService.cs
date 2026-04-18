using Microsoft.EntityFrameworkCore;
using ShelterAPI.Data;

namespace ShelterAPI.Services
{
    public class VotingService(BunkerDbContext db) : IVotingService
    {
        public async Task<VoteResult> CountVotesAsync(
            Guid gameId, int round, int votingInRound, CancellationToken ct = default)
        {
            var votes = await db.Votes
                .Where(v => v.GameId == gameId
                         && v.Round == round
                         && v.VotingInRound == votingInRound
                         && v.TargetId != null)
                .ToListAsync(ct);

            if (votes.Count == 0)
                return new VoteResult(IsTie: true, WinnerId: null);

            // Підраховуємо кількість голосів за кожного кандидата
            var tally = votes
                .GroupBy(v => v.TargetId!.Value)
                .Select(g => new { PlayerId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var maxVotes = tally[0].Count;
            var leaders = tally.Where(x => x.Count == maxVotes).ToList();

            // Нічия — більше одного гравця з максимумом голосів
            if (leaders.Count > 1)
                return new VoteResult(IsTie: true, WinnerId: null);

            return new VoteResult(IsTie: false, WinnerId: leaders[0].PlayerId);
        }

        public async Task<bool> AllVotedAsync(
            Guid gameId, int round, int votingInRound, CancellationToken ct = default)
        {
            // Всі гравці голосують — включно з вигнаними у попередніх раундах
            var activePlayers = await db.Players
                .Where(p => p.GameId == gameId)
                .Select(p => p.Id)
                .ToListAsync(ct);

            // Гравці, що вже проголосували у цьому голосуванні
            var votedPlayerIds = await db.Votes
                .Where(v => v.GameId == gameId
                         && v.Round == round
                         && v.VotingInRound == votingInRound)
                .Select(v => v.VoterId)
                .ToListAsync(ct);

            return activePlayers.All(id => votedPlayerIds.Contains(id));
        }
    }
}
