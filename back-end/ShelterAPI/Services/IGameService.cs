using ShelterAPI.DTOs.Responses;

namespace ShelterAPI.Services
{
    public interface IGameService
    {
        Task<CreateGameResponse> CreateGameAsync(string hostName, CancellationToken ct = default);
        Task<JoinGameResponse> JoinGameAsync(string code, string playerName, CancellationToken ct = default);
        Task<GameStateResponse> GetStateAsync(Guid gameId, Guid playerId, CancellationToken ct = default);
        Task StartGameAsync(Guid gameId, Guid playerId, CancellationToken ct = default);
        Task RevealBunkerCardAsync(Guid gameId, Guid playerId, CancellationToken ct = default);
        Task RevealCardAsync(Guid gameId, Guid playerId, Guid cardId, CancellationToken ct = default);
        Task VoteAsync(Guid gameId, Guid playerId, Guid targetId, CancellationToken ct = default);
        Task NextPhaseAsync(Guid gameId, Guid playerId, CancellationToken ct = default);
        Task PlaySpecialCardAsync(Guid gameId, Guid playerId, Guid? targetId, CancellationToken ct = default);
    }
}
