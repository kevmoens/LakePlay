using Microsoft.EntityFrameworkCore;

namespace LakePlay.Data
{
    public class LakePlayContext : DbContext
    {
        public DbSet<TriviaQuestion>? Questions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var accountEndpoint = Environment.GetEnvironmentVariable("CosmosAccountEndpoint", EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(accountEndpoint))
            {
                accountEndpoint = Environment.GetEnvironmentVariable("CosmosAccountEndpoint", EnvironmentVariableTarget.Machine);
            }
            if (string.IsNullOrEmpty(accountEndpoint))
            {
                throw new ArgumentNullException("CosmosAccountEndpoint");
            }
            var accountKey = Environment.GetEnvironmentVariable("CosmosAccountKey", EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(accountKey))
            {
                accountKey = Environment.GetEnvironmentVariable("CosmosAccountKey", EnvironmentVariableTarget.Machine);
            }
            if (string.IsNullOrEmpty(accountKey))
            {
                throw new ArgumentNullException("CosmosAccountKey");
            }
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
