using Microsoft.EntityFrameworkCore;

namespace TriviaForCheeseHeads.Data
{
    public class TriviaForCheeseHeadsCosmosContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public TriviaForCheeseHeadsCosmosContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public DbSet<TriviaQuestion>? Questions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            string? accountEndpoint = _configuration["CosmosAccountEndpoint"];
            string? accountKey = _configuration["CosmosAccountKey"];

            optionsBuilder.UseCosmos(
                accountEndpoint,
                accountKey,
                "LakePlay"
                );
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TriviaQuestion>()
                   .ToContainer("Questions")
                   .HasPartitionKey(e => e.Id);

            modelBuilder.Entity<TriviaQuestion>()
                .OwnsMany(p => p.ListOptions);
        }
    }
}
