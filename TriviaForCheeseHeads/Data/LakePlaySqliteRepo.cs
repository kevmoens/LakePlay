using TriviaForCheeseHeads.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace TriviaForCheeseHeads.Data
{
    public class TriviaForCheeseHeadsSqliteRepo : ITriviaForCheeseHeadsRepo<TriviaQuestion>
    {
        private readonly TriviaForCheeseHeadsSqliteContext _context;
        public TriviaForCheeseHeadsSqliteRepo(TriviaForCheeseHeadsSqliteContext context)
        {
            _context = context;
        }
        public void Add(TriviaQuestion question)
        {

            if (string.IsNullOrEmpty(question.Id))
            {
                question.Id = Guid.NewGuid().ToString();
            }
            foreach (var option in question.ListOptions)
            {
                if (string.IsNullOrEmpty(option.Id))
                {
                    option.Id = Guid.NewGuid().ToString();
                }
            }


            _context.Questions!.Add(question);
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<List<TriviaQuestion>> LoadAllAsync()
        {
            return await _context.Questions!.ToListAsync();
        }

        public void Remove(TriviaQuestion entity)
        {
            _context.Questions!.Remove(entity);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Update(TriviaQuestion question)
        {
            foreach (var option in question.ListOptions)
            {
                if (string.IsNullOrEmpty(option.Id))
                {
                    option.Id = Guid.NewGuid().ToString();
                }
            }
            _context.Questions!.Update(question);
        }

        public async Task<List<TriviaQuestion>> Where(Expression<Func<TriviaQuestion, bool>> clause)
        {
            var query = _context.Questions!.Where(clause);
            return await query.ToListAsync();
        }
    }
}
