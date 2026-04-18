namespace ShelterAPI.Models
{
    public class CardEntity
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool Revealed { get; private set; }

        // Navigation property
        public PlayerEntity? Player { get; set; }

        public static CardEntity Create(Guid playerId, string type, string content)
        {
            return new CardEntity
            {
                Id = Guid.NewGuid(),
                PlayerId = playerId,
                Type = type,
                Content = content,
                Revealed = false,
            };
        }

        public void Reveal() => Revealed = true;
    }
}
