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
			modelBuilder.Entity<TriviaQuestion>(question =>
			{
				question.HasKey(q => q.Id);

				question.Property(q => q.Id).IsRequired();
				question.Property(q => q.Text).IsRequired();
				question.Property(q => q.Difficulty).IsRequired();
				question.Property(q => q.Used).IsRequired();
				question.Property(q => q.AskedThisRound).IsRequired();

				question.OwnsMany(q => q.ListOptions, option =>
				{
					option.WithOwner().HasForeignKey(o => o.QuestionId);

					option.Property(o => o.Id).IsRequired();
					option.Property(o => o.Text).IsRequired();
					option.Property(o => o.IsAnswer).IsRequired();

					// Define composite key
					option.HasKey(o => new { o.Id, o.QuestionId });

					// Optional: rename table and columns if needed
					option.ToTable("TriviaQuestionOptions");
				});
			});
		}
    }
}
