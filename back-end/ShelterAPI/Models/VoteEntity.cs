using ShelterAPI.Models.GameEntity;

namespace ShelterAPI.Models
{
    public class VoteEntity
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public int Round { get; set; }
        public int VotingInRound { get; set; }
        public Guid VoterId { get; set; }
        public Guid? TargetId { get; set; }

        // Navigation properties
        public GameEntity.GameEntity? Game { get; set; }
        public PlayerEntity? Voter { get; set; }
        public PlayerEntity? Target { get; set; }

        public static VoteEntity Create(Guid gameId, int round, int votingInRound, Guid voterId, Guid targetId)
        {
            return new VoteEntity
            {
                Id = Guid.NewGuid(),
                GameId = gameId,
                Round = round,
                VotingInRound = votingInRound,
                VoterId = voterId,
                TargetId = targetId,
            };
        }
    }
}
