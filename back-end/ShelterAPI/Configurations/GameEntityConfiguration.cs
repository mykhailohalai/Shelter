using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShelterAPI.Models.GameEntity;

namespace ShelterAPI.Configurations
{
    public class GameEntityConfiguration : IEntityTypeConfiguration<GameEntity>
    {
        public void Configure(EntityTypeBuilder<GameEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Games");

            builder.Property(x => x.Code).IsRequired().HasMaxLength(6);
            builder.HasIndex(x => x.Code).IsUnique();

            builder.Property(x => x.Phase)
                .IsRequired()
                .HasConversion<string>();

            // Games → Catastrophes (many-to-one, no cascade — seed data must stay)
            builder.HasOne(x => x.Catastrophe)
                .WithMany()
                .HasForeignKey(x => x.CatastropheId)
                .OnDelete(DeleteBehavior.Restrict);

            // Games → Players (one-to-many)
            builder.HasMany(x => x.Players)
                .WithOne(p => p.Game)
                .HasForeignKey(p => p.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // Games → BunkerCards (one-to-many)
            builder.HasMany(x => x.BunkerCards)
                .WithOne(b => b.Game)
                .HasForeignKey(b => b.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // Games → Votes (one-to-many)
            builder.HasMany(x => x.Votes)
                .WithOne(v => v.Game)
                .HasForeignKey(v => v.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // HostId, ActiveSpeakerId, ExiledPlayerId — plain Guid columns (no FK constraint).
            // Enforcing circular FK Games↔Players breaks SQLite insert order.
            builder.Property(x => x.HostId).IsRequired();
            builder.Property(x => x.ActiveSpeakerId);
            builder.Property(x => x.ExiledPlayerId);
        }
    }
}
