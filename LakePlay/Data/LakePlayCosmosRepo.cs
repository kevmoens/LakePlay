using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using System.Threading;

namespace LakePlay.Data
{
    public class LakePlayCosmosRepo : ILakePlayRepo<TriviaQuestion>
    {
        private readonly LakePlayCosmosContext _context;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        public LakePlayCosmosRepo(LakePlayCosmosContext context)
        {
            _context = context;
        }
        public void Add(TriviaQuestion entity)
        {
            _semaphore.Wait();
            entity.Id = Guid.NewGuid().ToString();
            _context.Questions!.Add(entity);
            _semaphore.Release();
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<List<TriviaQuestion>> LoadAllAsync()
        {
            await _semaphore.WaitAsync();
            var resp = await _context.Questions!.ToListAsync();
            _semaphore.Release();
            return resp;
        }

        public void Remove(TriviaQuestion entity)
        {
            _semaphore.Wait();
            _context.Questions!.Remove(entity);
            _semaphore.Release();
        }

        public async Task<int> SaveAsync()
        {
            await _semaphore.WaitAsync();
            var resp = await _context.SaveChangesAsync();
            _semaphore.Release();
            return resp;
        }

        public void Update(TriviaQuestion entity)
        {
            _semaphore.Wait();
            _context.Questions!.Update(entity);
            _semaphore.Release();
        }
        
        public async Task<List<TriviaQuestion>> Where(Expression<Func<TriviaQuestion, bool>> clause)
        {
            await _semaphore.WaitAsync();
            var query = _context.Questions!.Where(clause);
            var resp = await query.ToListAsync();
            _semaphore.Release();
            return resp;

        }
       
    }
}
