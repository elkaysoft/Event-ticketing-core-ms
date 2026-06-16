using ETS.Application.Payment.Commands.Checkout;
using ETS.Application.Payment.Commands.Complete;
using ETS.Domain.Contracts;
using ETS.Domain.Extensions;
using ETS.WebApi.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ETS.WebApi.Endpoints.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : AuthControllerBase<PaymentController>
    {
        public PaymentController(ILogger<PaymentController> logger, 
            IConfiguration config, 
            IUserContext userService,
            ISender mediator) : base(logger, config, userService, mediator)
        {
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(EventCheckoutRequest request)
        {
            var command = new CheckoutCommand(request.FullName,
                request.PhoneNumber,
                request.EmailAddress,
                request.TicketDetails.Select(x => new CheckoutListDto(x.TicketId, x.Unit)
                ).ToList());

            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPost("paystack/notification")]
        public async Task<IActionResult> HandlePaystackWebhook(CompletePaymentCommand model)
        {
            var result = await _mediator.Send(model);
            return result.ToActionResult();
        }

    }
}
