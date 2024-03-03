using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace LakePlay.Data
{
    public class LakePlayCosmosRepo : ILakePlayRepo<TriviaQuestion>
    {
        private readonly LakePlayCosmosContext _context;

        public LakePlayCosmosRepo(LakePlayCosmosContext context)
        {
            _context = context;
        }
        public void Add(TriviaQuestion entity)
        {
            _context.Questions!.Add(entity);
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

        public void Update(TriviaQuestion entity)
        {
            _context.Questions!.Update(entity);
        }
        
        public async Task<List<TriviaQuestion>> Where(Expression<Func<TriviaQuestion, bool>> clause)
        {
            var query = _context.Questions!.Where(clause);
            return await query.ToListAsync();
        }
       
    }
}
