using System.Linq.Expressions;

namespace Oficina.Common.Domain.IRepository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[]? includes);
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includes);
        Task<IEnumerable<TEntity>> GetAllAsync(
           int page = 1,
           int pageSize = 10,
           Expression<Func<TEntity, bool>>? predicate = null,
           params Expression<Func<TEntity, object>>[] includes);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        Task SaveChangesAsync();
    }
}
