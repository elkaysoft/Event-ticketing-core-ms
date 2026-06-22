using ETS.Domain.Entities;
using ETS.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ETS.Infrastructure.Persistence.DbContexts
{
    public class ReadApplicationDbContext : ReadOnlyApplicationDbContext<ReadApplicationDbContext>
    {
        public ReadApplicationDbContext(DbContextOptions<ReadApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.RegisterAllEntities(typeof(AuditLog).Assembly);
            modelBuilder.ApplySoftDeleteFilters();
            modelBuilder.Entity<Order>().Property(x => x.OrderStatus).HasConversion<string>();
            modelBuilder.Entity<Order>().Property(x => x.RedemptionStatus).HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
       

    }
}
