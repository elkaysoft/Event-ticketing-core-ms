using ETS.Domain.Common;
using System.Linq.Expressions;

namespace ETS.Domain.Repositories
{
    public interface IRepository<TEntity, TId>
        where TEntity : Entity<TId>
    {
        Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken);
        Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken);

        Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includeExpressions);
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, 
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includeExpressions);
        
        Task<PaginatedList<TProjection>> GetPaginatedAsync<TProjection>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TProjection>> selector,
            int pageNumber = 1,
            int pageSize = 10,
            Expression<Func<TEntity, object>> orderBy = null!,
            bool isAscending = true,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includeExpressions);

        Task<TProjection?> GetSingleAsync<TProjection>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TProjection>> selctor,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includeExpressions
            );

        void Add(TEntity entity);
        void AddRange(List<TEntity> entities);
        void Remove(TEntity entity);
        void Update(TEntity entity);
        void UpdateRange(List<TEntity> entities);
        void RemoveRange(List<TEntity> entities);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);
    }
}
