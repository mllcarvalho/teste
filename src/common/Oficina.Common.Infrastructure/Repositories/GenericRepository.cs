using Microsoft.EntityFrameworkCore;
using Oficina.Common.Domain.IRepository;
using System.Linq.Expressions;

namespace Oficina.Common.Infrastructure.Repositories
{
    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        protected readonly TContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(TContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<TEntity?> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[]? includes)
        {
            if (includes == null || includes.Length == 0)
            {
                return await _dbSet.FindAsync(id);
            }
            else
            {
                IQueryable<TEntity> query = _dbSet;
                foreach (var include in includes)
                    query = query.Include(include);

                return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
            }
        }
       
        public virtual async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            if (predicate != null)
                query = query.Where(predicate);

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
            int page = 1,
            int pageSize = 10,
            Expression<Func<TEntity, bool>>? predicate = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            if (predicate != null)
                query = query.Where(predicate);

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task SaveChangesAsync()
        {
            var entries = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);

            foreach (var entry in entries)
            {
                var property = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "DataAtualizacao");
                if (property != null)
                    property.CurrentValue = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}
