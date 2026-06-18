using ETS.Domain.Common;

namespace ETS.Domain.Entities
{
    public class RefreshToken: Entity<Guid>
    {
        protected RefreshToken() { }

        public long UserId { get; private set; }
        public string TokenHash { get; private set; }
        public DateTime IssuedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool Revoked { get; private set; }
        public string? RevokedReason { get; private set; }

        public static RefreshToken Create(long userId, string tokenHash, DateTime issuedAt, DateTime expiresAt)
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenHash = tokenHash,
                IssuedAt = issuedAt,
                ExpiresAt = expiresAt,
                Revoked = false
            };
        }

    }
}
