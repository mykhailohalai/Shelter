using Microsoft.EntityFrameworkCore;
using ShelterAPI.Models;
using ShelterAPI.Models.GameEntity;

namespace ShelterAPI.Data
{
    public class BunkerDbContext(DbContextOptions<BunkerDbContext> options) : DbContext(options)
    {
        public DbSet<GameEntity> Games { get; set; }
        public DbSet<PlayerEntity> Players { get; set; }
        public DbSet<CardEntity> Cards { get; set; }
        public DbSet<SpecialCardEntity> SpecialCards { get; set; }
        public DbSet<BunkerCardEntity> BunkerCards { get; set; }
        public DbSet<VoteEntity> Votes { get; set; }
        public DbSet<CatastropheEntity> Catastrophes { get; set; }
        public DbSet<CardTemplateEntity> CardTemplates { get; set; }
        public DbSet<SpecialCardTemplateEntity> SpecialCardTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BunkerDbContext).Assembly);

            modelBuilder.Entity<CatastropheEntity>().HasData(SeedData.Catastrophes);
            modelBuilder.Entity<CardTemplateEntity>().HasData(SeedData.CardTemplates);
            modelBuilder.Entity<SpecialCardTemplateEntity>().HasData(SeedData.SpecialCardTemplates);
        }
    }
}
