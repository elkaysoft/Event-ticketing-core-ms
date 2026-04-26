using ETS.Domain.Entities;

namespace ETS.Domain.Repositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken, Guid>
    {
    }
}
