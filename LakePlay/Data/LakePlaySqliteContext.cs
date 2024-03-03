using Microsoft.EntityFrameworkCore;

namespace LakePlay.Data
{
    public class LakePlaySqliteContext : DbContext
    {
        public LakePlaySqliteContext(DbContextOptions<LakePlaySqliteContext> options) : base(options)
        {
            Database.Migrate();
        }
        public DbSet<TriviaQuestion>? Questions { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = @"TimeOff.sqlite";
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
