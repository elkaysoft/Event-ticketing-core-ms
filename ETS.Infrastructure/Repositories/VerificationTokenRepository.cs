using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Repositories;

namespace ETS.Infrastructure.Repositories
{
    public class VerificationTokenRepository : Repository<VerificationToken, long>, IVerificationTokenRepository
    {
        public VerificationTokenRepository(IWriteApplicationDbContext writeDbContext, 
            IReadApplicationDbContext readDbContext) 
            : base(writeDbContext, readDbContext)
        {
        }
    }
}
