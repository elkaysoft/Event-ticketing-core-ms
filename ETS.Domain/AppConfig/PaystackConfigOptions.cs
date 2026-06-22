namespace ETS.Domain.AppConfig
{
    public class PaystackConfigOptions
    {
        public string BaseUrl { get; set; }
        public string ApiSecret { get; set; }
        public string FailedPaymentUrl { get; set; }
        public string SuccessfulPaymentUrl { get; set; }
        public string WhitelistedIP { get; set; }
        public decimal Fee { get; set; }
    }
}
