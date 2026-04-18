using ShelterAPI.Models.GameEntity;

namespace ShelterAPI.Models
{
    public class PlayerEntity
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsExiled { get; private set; }
        public int SeatOrder { get; set; }

        // Navigation properties
        public GameEntity.GameEntity? Game { get; set; }
        public ICollection<CardEntity> Cards { get; set; } = [];
        public SpecialCardEntity? SpecialCard { get; set; }

        public static PlayerEntity Create(Guid gameId, string name, int seatOrder)
        {
            return new PlayerEntity
            {
                Id = Guid.NewGuid(),
                GameId = gameId,
                Name = name,
                SeatOrder = seatOrder,
                IsExiled = false,
            };
        }

        public void Exile() => IsExiled = true;

        public void Restore() => IsExiled = false;
    }
}
