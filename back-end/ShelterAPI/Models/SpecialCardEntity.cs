namespace ShelterAPI.Models
{
    public class SpecialCardEntity
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Played { get; private set; }

        // Navigation property
        public PlayerEntity? Player { get; set; }

        public static SpecialCardEntity Create(Guid playerId, string title, string description)
        {
            return new SpecialCardEntity
            {
                Id = Guid.NewGuid(),
                PlayerId = playerId,
                Title = title,
                Description = description,
                Played = false,
            };
        }

        public void Play() => Played = true;
    }
}
