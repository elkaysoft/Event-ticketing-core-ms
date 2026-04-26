namespace ETS.Domain.Models
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public DateTime ExpiresIn { get; set; }
        public string RefreshToken { get; set; }

    }
}
