using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Errors;
using ETS.Domain.Extensions;
using ETS.Domain.Models;
using ETS.Domain.Repositories;
using ETS.Infrastructure.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ETS.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly AuthenticationOptions _authOptions;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public TokenService(ILogger<TokenService> logger,
            IOptions<AuthenticationOptions> authOptions,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _logger = logger;
            _authOptions = authOptions.Value;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<Result<TokenResponse>> GenerateAccessTokenAsync(long UserId,
             CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(UserId, cancellationToken);
                if (user is null)
                {
                    return Result.Failure<TokenResponse>(UserErrors.NotFound);
                }

                var tokenLifetime = TimeSpan.FromSeconds(_authOptions.TokenExpiryInSeconds);
                var expiresAt = DateTime.Now.Add(tokenLifetime);

                var claims = new List<Claim>()
                {
                    new(JwtRegisteredClaimNames.Sub, UserId.ToString()),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer),
                    new(JwtRegisteredClaimNames.Email, user.EmailAddress),
                    new("user_id", UserId.ToString()),
                    new("user_role", user.Role.ToString()),
                    new("fullname", user.FullName),
                    new("phoneNumber", user.PhoneNumber!)
                };

                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.IssuerKey));
                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    issuer: _authOptions.Issuer,
                    audience: _authOptions.Audience,
                    claims: claims,
                    expires: expiresAt,
                    signingCredentials: signingCredentials
                    );
                
                var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                var refreshToken = Cryptography.CharGenerator.genID(20, Domain.Enums.CharacterSet.ALPHA_NUMERIC_NON_CASE);
                var tokenHash = ComputeHash(refreshToken);
                var expiresAtRefresh = DateTime.UtcNow.AddSeconds(_authOptions.RefreshTokenExpiryInSeconds);

                var refreshTokenEntity = RefreshToken.Create(UserId, tokenHash, DateTime.UtcNow, expiresAtRefresh);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new TokenResponse
                {
                    AccessToken = accessToken,
                    ExpiresIn = expiresAt,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating access token for user {UserId}", UserId);
                return Result.Failure<TokenResponse>(new Error("TokenGenerationFailed", "Failed to generate access token."));
            }
        }

        public async Task<Result<TokenResponse>> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            try
            {
                var tokenHash = ComputeHash(refreshToken);
                var storedToken = await _refreshTokenRepository.GetSingleAsync(rt => 
                            rt.TokenHash == tokenHash, cancellationToken);
                if (storedToken is null || storedToken.ExpiresAt < DateTime.UtcNow)
                {
                    return Result.Failure<TokenResponse>(new Error("InvalidRefreshToken", "The provided refresh token is invalid or has expired."));
                }

                return await GenerateAccessTokenAsync(storedToken.UserId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing access token with refresh token");
                return Result.Failure<TokenResponse>(new Error("TokenRefreshFailed", "Failed to refresh access token."));
            }
        }

        private static string ComputeHash(string input)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
