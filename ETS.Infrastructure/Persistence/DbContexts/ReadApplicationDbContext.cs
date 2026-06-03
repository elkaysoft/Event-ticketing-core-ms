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

            base.OnModelCreating(modelBuilder);
        }
       

    }
}
