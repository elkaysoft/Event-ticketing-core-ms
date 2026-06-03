using ETS.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ETS.Infrastructure.Persistence.DbContexts
{
    public abstract class ReadOnlyApplicationDbContext<T> : DbContext, IReadApplicationDbContext 
        where T : DbContext
    {
        private readonly Assembly _assembly;

        protected ReadOnlyApplicationDbContext(DbContextOptions<T> options, Assembly? assembly = null)
            :base(options)
        {
            _assembly = assembly ?? typeof(T).Assembly;

            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbContext GetDbContext() => this;
      

    }
}
