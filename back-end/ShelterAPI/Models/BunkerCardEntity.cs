using ShelterAPI.Models.GameEntity;

namespace ShelterAPI.Models
{
    public class BunkerCardEntity
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public int Slot { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool Revealed { get; private set; }

        // Navigation property
        public GameEntity.GameEntity? Game { get; set; }

        public static BunkerCardEntity Create(Guid gameId, int slot, string content)
        {
            return new BunkerCardEntity
            {
                Id = Guid.NewGuid(),
                GameId = gameId,
                Slot = slot,
                Content = content,
                Revealed = false,
            };
        }

        public void Reveal() => Revealed = true;
    }
}
