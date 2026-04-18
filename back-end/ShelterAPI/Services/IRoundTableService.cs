namespace ShelterAPI.Services
{
    public interface IRoundTableService
    {
        /// <summary>
        /// Кількість голосувань у даному раунді для даної кількості гравців.
        /// Значення 0 означає голосування немає.
        /// </summary>
        int GetVotingsInRound(int playerCount, int round);

        /// <summary>
        /// Скільки гравців потрапляє в бункер.
        /// </summary>
        int GetBunkerCapacity(int playerCount);
    }
}
