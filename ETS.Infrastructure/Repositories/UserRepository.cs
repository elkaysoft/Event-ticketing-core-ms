using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Repositories;

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
