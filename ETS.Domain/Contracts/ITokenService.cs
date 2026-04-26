using ETS.Domain.Common;
using ETS.Domain.Models;
using System.Security.Claims;

namespace ETS.Domain.Contracts
{
    public interface ITokenService
    {
        Task<Result<TokenResponse>> GenerateAccessTokenAsync(long UserId,            
            CancellationToken cancellationToken);

        Task<Result<TokenResponse>> RefreshAccessTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken);
    }
}
