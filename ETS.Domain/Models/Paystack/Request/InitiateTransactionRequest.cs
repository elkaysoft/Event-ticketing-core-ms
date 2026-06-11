using Newtonsoft.Json;

namespace ETS.Domain.Models.Paystack.Request
{
    public class InitiateTransactionRequest
    {
        [JsonProperty("email")]
        public required string EmailAddress { get; set; }
        [JsonProperty("amount")]
        public required string Amount { get; set; }
        [JsonProperty("reference")]
        public required string Reference { get; set; }
        [JsonProperty("callback_url")]
        public string CallbackUrl { get; set; }
        [JsonProperty("metadata")]
        public string? MetaData { get; set; }
                
    }
}
