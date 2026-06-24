using ETS.Application.Payment.Commands.Complete;
using System.Numerics;

namespace ETS.WebApi.DTO
{
    public class PaystackWebhookRequest
    {
        public string @event { get; set; }
        public PaystackWebhookRequestData data { get; set; }
    }

   

}
