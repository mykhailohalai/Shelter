namespace ShelterAPI.Services
{
    public record VoteResult(bool IsTie, Guid? WinnerId);

    public interface IVotingService
    {
        /// <summary>
        /// Підраховує голоси поточного голосування.
        /// Повертає переможця або IsTie=true при нічиїй.
        /// </summary>
        Task<VoteResult> CountVotesAsync(Guid gameId, int round, int votingInRound, CancellationToken ct = default);

        /// <summary>
        /// Перевіряє чи всі активні гравці вже проголосували.
        /// </summary>
        Task<bool> AllVotedAsync(Guid gameId, int round, int votingInRound, CancellationToken ct = default);
    }
}
