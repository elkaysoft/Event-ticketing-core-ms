using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Repositories;

namespace ETS.Infrastructure.Repositories
{
    public sealed class RefreshTokenRepository : Repository<RefreshToken, Guid>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(IWriteApplicationDbContext writeDbContext, 
            IReadApplicationDbContext readDbContext) : base(writeDbContext, readDbContext)
        {
        }
    }
}
