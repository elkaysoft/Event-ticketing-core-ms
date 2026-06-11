namespace ETS.Domain.AppConfig
{
    public class PaystackConfigOptions
    {
        public string BaseUrl { get; set; }
        public string ApiSecret { get; set; }
        public string CancellationUrl { get; set; }
        public string CallbackUrl { get; set; }
        public string WhitelistedIP { get; set; }
    }
}
