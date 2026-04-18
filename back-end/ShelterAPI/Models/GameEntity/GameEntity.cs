using ShelterAPI.Enums;

namespace ShelterAPI.Models.GameEntity
{
    public class GameEntity
    {
        public Guid Id { get; set; }
        public string Code { get; private set; } = string.Empty;
        public GamePhase Phase { get; private set; }

        private int round;
        public int Round
        {
            get => round;
            private set
            {
                if (value < 1 || value > 5)
                    throw new ArgumentException(nameof(value));
                round = value;
            }
        }

        private int votingInRound;
        public int VotingInRound
        {
            get => votingInRound;
            private set
            {
                if (value < 1 || value > 2)
                    throw new ArgumentException(nameof(value));
                votingInRound = value;
            }
        }

        public int BunkerCapacity { get; private set; }
        public Guid HostId { get; private set; }
        public Guid? ActiveSpeakerId { get; private set; }
        public Guid? ExiledPlayerId { get; private set; }
        public Guid CatastropheId { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // Navigation properties
        public CatastropheEntity? Catastrophe { get; set; }
        public ICollection<PlayerEntity> Players { get; set; } = [];
        public ICollection<BunkerCardEntity> BunkerCards { get; set; } = [];
        public ICollection<VoteEntity> Votes { get; set; } = [];

        public static GameEntity Create(Guid hostId, Guid catastropheId, int bunkerCapacity)
        {
            return new GameEntity
            {
                Id = Guid.NewGuid(),
                HostId = hostId,
                CatastropheId = catastropheId,
                BunkerCapacity = bunkerCapacity,
                Round = 1,
                VotingInRound = 1,
                Phase = GamePhase.Waiting,
                Code = GenerateCode(),
                ActiveSpeakerId = null,
                ExiledPlayerId = null,
                CreatedAt = DateTime.UtcNow,
            };
        }

        public void SetPhase(GamePhase phase) => Phase = phase;

        public void SetActiveSpeaker(Guid? speakerId) => ActiveSpeakerId = speakerId;

        public void SetExiled(Guid playerId) => ExiledPlayerId = playerId;

        public void SetBunkerCapacity(int capacity) => BunkerCapacity = capacity;

        public void AdvanceRound()
        {
            if (Round < 5)
                Round = round + 1;

            VotingInRound = 1;
        }

        public void SetHost(Guid hostId) => HostId = hostId;

        public void AdvanceVotingInRound()
        {
            if (votingInRound < 2)
                VotingInRound = votingInRound + 1;
        }

        private static string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Range(0, 6)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }
    }
}
