using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace LakePlay.Data
{
    public interface ILakePlayRepo<TEntity> : IDisposable where TEntity : class
    {

        void Add(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        Task<List<TEntity>> Where(Expression<Func<TEntity,bool>> clause);
        Task<int> SaveAsync();
        Task<List<TEntity>> LoadAllAsync();

    }
}
