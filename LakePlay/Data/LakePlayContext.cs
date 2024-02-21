using Microsoft.EntityFrameworkCore;

namespace LakePlay.Data
{
    public class LakePlayContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public LakePlayContext(IConfiguration configuration)
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
