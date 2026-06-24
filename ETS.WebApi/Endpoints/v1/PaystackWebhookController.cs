using ETS.Application.Payment.Commands.Complete;
using ETS.Domain.Contracts;
using ETS.Domain.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETS.WebApi.Endpoints.v1
{    
    [ApiController]
    [Route("api/paystack-webhook")]
    public class PaystackWebhookController : AuthControllerBase<PaystackWebhookController>
    {
        private readonly ILogger<PaystackWebhookController> _logger;
        public PaystackWebhookController(ILogger<PaystackWebhookController> logger, 
            IConfiguration config, 
            IUserContext userService, 
            ISender mediator) : base(logger, config, userService, mediator)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            // proceed to read the raw body content for exact signature verification
            using var reader = new StreamReader(Request.Body);
            string jsonBody = await reader.ReadToEndAsync();
            _logger.LogInformation("Webhook Request at {0} - {1}", DateTime.UtcNow, jsonBody);

            var result = await _mediator.Send(new CompletePaymentCommand(jsonBody));
            return result.ToActionResult();
        }
    }
}
