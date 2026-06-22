using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ETS.Infrastructure.Persistence.DbContexts
{
    /// <summary>
    /// Represents the base application database context
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseApplicationDbContext<T> : DbContext where T : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserContext _currentUser;

        protected BaseApplicationDbContext(
            DbContextOptions<T> options,
            IHttpContextAccessor httpContextAccessor,
            IUserContext currentUser)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _currentUser = currentUser;
        }

        protected abstract Assembly ConfigurationAssembly { get; }

        public override int SaveChanges()
        {
            try
            {
                TrackAuditChanges();
                var result = base.SaveChanges();
                return result;
            }
            catch(DbUpdateConcurrencyException ex)
            {
                throw new Exception("Concurrency exception occurred", ex);
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                TrackAuditChanges();
                var result = base.SaveChangesAsync(cancellationToken);
                return result;
            }
            catch(DbUpdateConcurrencyException ex)
            {
                throw new Exception("Concurrency exception occurred", ex);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.RegisterAllEntities(typeof(AuditLog).Assembly);

            // Explicitly register AuditLog since it doesn't inherit EntityBase
            modelBuilder.Entity<AuditLog>();

            // Apply entity configuration for shared assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseApplicationDbContext<>).Assembly);

            modelBuilder.ApplyConfigurationsFromAssembly(ConfigurationAssembly);

            modelBuilder.ApplySoftDeleteFilters();

            base.OnModelCreating(modelBuilder);
        }



        private void TrackAuditChanges()
        {
            var auditEntries = new List<AuditLog>();

            var now = DateTime.UtcNow;
            var user = _currentUser.UserEmail ?? $"UNIDENTIFIED-{DateTime.UtcNow}";

            foreach(var entry in ChangeTracker.Entries<EntityBase>().Where(e => e.State is EntityState.Modified or EntityState.Added or EntityState.Deleted))
            {
                var entityType = entry.Context.Model.FindEntityType(entry.Entity.GetType());
                var tableName = entityType?.ClrType.Name ?? entry.Entity.GetType().BaseType?.Name ?? entry.Entity.GetType().Name;

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.CreatedBy = user;                        
                        break;
                        case EntityState.Modified:
                        case EntityState.Deleted:
                        entry.Entity.LastModifiedAt = now;
                        entry.Entity.LastModifiedBy = user;
                        break;
                }

                auditEntries.AddRange(
                    from property in entry.Properties
                    where !property.IsTemporary && (property.IsModified || entry.State == EntityState.Added)
                    select new AuditLog(
                        tableName,
                        property.Metadata.Name,
                        entry.Properties.SingleOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString(),
                        entry.State == EntityState.Modified ? property.OriginalValue?.ToString() : null,
                        entry.State == EntityState.Deleted ? null : property.CurrentValue?.ToString(),
                        now,
                        user,
                        entry.State.ToString()
                    )
                );               
            }
              
            if (auditEntries.Any())
            {
                Set<AuditLog>().AddRange(auditEntries);
            }
        }




    }
}
