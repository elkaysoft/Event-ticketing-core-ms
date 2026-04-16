using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS.Infrastructure.Repositories
{
    public class UserRepository : Repository<User, long>, IUserRepository
    {
        public UserRepository(IWriteApplicationDbContext writeDbContext, 
            IReadApplicationDbContext readDbContext) 
            : base(writeDbContext, readDbContext)
        {
        }
    }
}
