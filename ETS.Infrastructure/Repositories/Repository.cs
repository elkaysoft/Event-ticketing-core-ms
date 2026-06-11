using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ETS.Infrastructure.Repositories
{
    public abstract class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : Entity<TId>
    {
        protected readonly DbContext _writeDbContext;
        protected readonly DbContext _readDbContext;

        protected readonly DbSet<TEntity> _writeDbSet;
        protected readonly DbSet<TEntity> _readDbSet;

        protected Repository(IDbContext writeDbContext, IDbContext readDbContext)
        {
            _writeDbContext = writeDbContext.GetDbContext();
            _readDbContext = readDbContext.GetDbContext();

            _writeDbSet = _writeDbContext.Set<TEntity>();
            _readDbSet = _readDbContext.Set<TEntity>();
        }


        public void Add(TEntity entity)
        {
            _writeDbContext.Add(entity);
        }

        public void AddRange(List<TEntity> entities)
        {
            _writeDbContext.AddRange(entities);
        }


        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _readDbSet.AsNoTracking().AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, 
            CancellationToken cancellationToken = default, 
            params Expression<Func<TEntity, object>>[] includeExpressions)
        {
            var query = _writeDbSet.Where(predicate);
            foreach (var includeExpression in includeExpressions)
                query = query.Include(includeExpression);
            return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken)
        {
            return await
                _writeDbSet
                .FirstOrDefaultAsync(e => !e.IsDeleted && e.Id!.Equals(id), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<PaginatedList<TProjection>> GetPaginatedAsync<TProjection>(Expression<Func<TEntity, bool>>
            predicate, 
            Expression<Func<TEntity, TProjection>> selector, 
            int pageNumber = 1, 
            int pageSize = 10,
            Expression<Func<TEntity, object>> orderBy = null!,
            bool isAscending = true,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default, 
            params Expression<Func<TEntity, object>>[] includeExpressions)
        {
            var query = BuildBaseQuery(includeDeleted, includeExpressions);

            if (orderBy != null)
                query = isAscending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);

            // Apply projection before pagination
            var projectedQuery = query.Select(selector);

            return await PaginatedListedExtender<TProjection>.CreateAsync(projectedQuery, pageNumber, pageSize, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await
                _writeDbSet.FirstOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default, 
            params Expression<Func<TEntity, object>>[] includeExpressions)
        {
            var query = _writeDbSet.Where(predicate);
            foreach (var includeExpression in includeExpressions)
                query = query.Include(includeExpression);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }


        public async Task<TProjection?> GetSingleAsync<TProjection>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TProjection>> selector,
            CancellationToken cancellationToken = default, 
            params Expression<Func<TEntity, object>>[] includeExpressions)
        {
            var query = _writeDbSet.Where(predicate);

            foreach (var includeExpression in includeExpressions)
                query = query.Include(includeExpression);

            return await query.Select(selector).FirstOrDefaultAsync(cancellationToken);
        }

        public void Remove(TEntity entity)
        {
            entity.IsDeleted = true;
        }

        public void RemoveRange(List<TEntity> entities)
        {
            foreach (var entity in entities)
                entity.IsDeleted = true;
        }

        public void Update(TEntity entity)
        {
            _writeDbContext.Update(entity);
        }

        public void UpdateRange(List<TEntity> entities)
        {
            _writeDbContext.UpdateRange(entities);
        }

        private IQueryable<TEntity> BuildBaseQuery(bool includeDeleted = false, params Expression<Func<TEntity, object>>[] includeExpressions)
        {
            var query = _readDbSet.AsNoTracking().AsQueryable();

            foreach (var includeExpression in includeExpressions)
                query = query.Include(includeExpression);

            if (includeDeleted)
                query = query.IgnoreQueryFilters();

            return query;
        }


        private IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, string sortColumn, string sortOrder)
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
                return query;

            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, sortColumn);
            var lambda = Expression.Lambda<Func<TEntity, object>>(Expression.Convert(property, typeof(object)), parameter);

            if (sortOrder?.ToLower() == "desc")
                query = query.OrderByDescending(lambda);
            else
                query = query.OrderBy(lambda);

            return query;
        }

    }
}
