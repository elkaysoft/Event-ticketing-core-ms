namespace ETS.Infrastructure.Authentication
{
    public sealed class AuthenticationOptions
    {
        public string Audience { get; set; } = string.Empty;
        public string MetadataUrl { get; set; } = string.Empty;
        public bool RequireHttpsMetadata { get; set; }
        public string Issuer { get; set; } = string.Empty;
        public string IssuerKey { get; set; } = string.Empty;
        public int DurationInMinutes { get; set; }
    }
}
