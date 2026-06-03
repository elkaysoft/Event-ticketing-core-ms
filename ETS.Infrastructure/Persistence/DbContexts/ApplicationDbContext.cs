using ETS.Domain.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ETS.Infrastructure.Persistence.DbContexts
{
    public abstract class ApplicationDbContext<T> : BaseApplicationDbContext<T>,
        IWriteApplicationDbContext,
        IUnitOfWork where T : DbContext
    {
        protected ApplicationDbContext(DbContextOptions<T> options, 
            IHttpContextAccessor httpContextAccessor, 
            IUserContext currentUser) 
            : base(options, httpContextAccessor, currentUser)
        {
        }

        public DbContext GetDbContext() => this;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
