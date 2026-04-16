using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ETS.Infrastructure.Persistence.DbContexts
{
    public class WriteApplicationDbContext : ApplicationDbContext<WriteApplicationDbContext>,
        IUnitOfWork, 
        IWriteApplicationDbContext
    {
        /// <summary>
        /// WriteApplicationDbContext
        /// </summary>
        /// <param name="options"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="currentUser"></param>
        /// <param name="assembly"></param>
        public WriteApplicationDbContext(DbContextOptions<WriteApplicationDbContext> options,
            IHttpContextAccessor httpContextAccessor, 
            IUserContext currentUser) : base(options, httpContextAccessor, currentUser)
        {
        }

        protected override Assembly ConfigurationAssembly 
            => typeof(WriteApplicationDbContext).Assembly;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.RegisterAllEntities(typeof(AuditLog).Assembly);
            base.OnModelCreating(modelBuilder);
        }

    }
}
