namespace ShelterAPI.Services
{
    public class RoundTableService : IRoundTableService
    {
        // [playerCount][round-1] → кількість голосувань
        // Таблиця з API_CONTRACT.md, гравців 4–16
        private static readonly Dictionary<int, int[]> VotingsTable = new()
        {
            [4]  = [0, 0, 0, 1, 1],
            [5]  = [0, 0, 1, 1, 1],
            [6]  = [0, 0, 1, 1, 1],
            [7]  = [0, 1, 1, 1, 1],
            [8]  = [0, 1, 1, 1, 1],
            [9]  = [0, 1, 1, 1, 2],
            [10] = [0, 1, 1, 1, 2],
            [11] = [0, 1, 1, 2, 2],
            [12] = [0, 1, 1, 2, 2],
            [13] = [0, 1, 2, 2, 2],
            [14] = [0, 1, 2, 2, 2],
            [15] = [0, 2, 2, 2, 2],
            [16] = [0, 2, 2, 2, 2],
        };

        // Скільки гравців потрапляє в бункер
        private static readonly Dictionary<int, int> BunkerCapacityTable = new()
        {
            [4]  = 2,
            [5]  = 2,
            [6]  = 3,
            [7]  = 3,
            [8]  = 4,
            [9]  = 4,
            [10] = 5,
            [11] = 5,
            [12] = 6,
            [13] = 6,
            [14] = 7,
            [15] = 7,
            [16] = 8,
        };

        public int GetVotingsInRound(int playerCount, int round)
        {
            if (round < 1 || round > 5)
                throw new ArgumentOutOfRangeException(nameof(round), "Раунд має бути від 1 до 5.");

            var count = Math.Clamp(playerCount, 4, 16);

            return VotingsTable[count][round - 1];
        }

        public int GetBunkerCapacity(int playerCount)
        {
            var count = Math.Clamp(playerCount, 4, 16);
            return BunkerCapacityTable[count];
        }
    }
}
