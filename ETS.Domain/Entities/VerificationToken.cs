using ETS.Domain.Common;
using ETS.Domain.Enums;

namespace ETS.Domain.Entities
{
    public class VerificationToken : Entity<long>
    {
        public long UserId { get; set; }
        public string Token { get; set; }
        public string? RequestId { get; set; }
        public DateTime GeneratedAt { get; set; }
        public DateTime? ExpirationAt { get; set; }
        public VerificationPurpose VerificationPurpose { get; set; }
        public bool IsValid { get; set; }


        public static VerificationToken Create(long userId, string token, DateTime expirationTime,
            VerificationPurpose verificationPurpose, string? requestId = "")
        {
            return new VerificationToken
            {
                UserId = userId,
                ExpirationAt = expirationTime,
                VerificationPurpose = verificationPurpose,
                GeneratedAt = DateTime.UtcNow,  
                RequestId = requestId,
                Token = token,
                IsValid = false
            };
        }

    }
}
