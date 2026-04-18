using ShelterAPI.Models;

namespace ShelterAPI.Services
{
    public interface ICardDealingService
    {
        /// <summary>
        /// Роздає 6 карток + 1 спеціальну кожному гравцю та генерує 5 карток бункера.
        /// Викликається один раз при actions/start.
        /// </summary>
        Task DealAsync(Guid gameId, IList<PlayerEntity> players, CancellationToken ct = default);
    }
}
