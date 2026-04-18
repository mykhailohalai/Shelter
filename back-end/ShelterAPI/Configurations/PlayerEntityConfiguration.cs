using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShelterAPI.Models;

namespace ShelterAPI.Configurations
{
    public class PlayerEntityConfiguration : IEntityTypeConfiguration<PlayerEntity>
    {
        public void Configure(EntityTypeBuilder<PlayerEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Players");

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

            // Players → Cards (one-to-many)
            builder.HasMany(x => x.Cards)
                .WithOne(c => c.Player)
                .HasForeignKey(c => c.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Players → SpecialCard (one-to-one)
            builder.HasOne(x => x.SpecialCard)
                .WithOne(s => s.Player)
                .HasForeignKey<SpecialCardEntity>(s => s.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
