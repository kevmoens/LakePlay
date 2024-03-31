using Microsoft.EntityFrameworkCore;

namespace TriviaForCheeseHeads.Data
{
    public class TriviaForCheeseHeadsSqliteContext : DbContext
    {
        public TriviaForCheeseHeadsSqliteContext(DbContextOptions<TriviaForCheeseHeadsSqliteContext> options) : base(options)
        {
            Database.Migrate();
        }
        public DbSet<TriviaQuestion>? Questions { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = @"TriviaForCheeseHeads.sqlite";
            optionsBuilder.UseSqlite(
                $"data source={path}");
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
