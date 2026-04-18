using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShelterAPI.Models;

namespace ShelterAPI.Configurations
{
    public class VoteEntityConfiguration : IEntityTypeConfiguration<VoteEntity>
    {
        public void Configure(EntityTypeBuilder<VoteEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Votes");

            // VoterId → Players (no cascade — handled by Games cascade)
            builder.HasOne(x => x.Voter)
                .WithMany()
                .HasForeignKey(x => x.VoterId)
                .OnDelete(DeleteBehavior.Restrict);

            // TargetId → Players (nullable, no cascade)
            builder.HasOne(x => x.Target)
                .WithMany()
                .HasForeignKey(x => x.TargetId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
