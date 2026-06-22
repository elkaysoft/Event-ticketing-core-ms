using ETS.Application.Events.Queries.Events;
using ETS.Application.Events.Queries.GetActiveEvent;
using ETS.Application.Events.Queries.GetAllActiveEvents;
using ETS.Application.Events.Queries.GetCustomerEvent;
using ETS.Application.Events.Queries.GetSingleEvent;
using ETS.Application.Payment.Queries.ValidateCustomerTicket;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Extensions;
using ETS.WebApi.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETS.WebApi.Endpoints.v1
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CustomerController : AuthControllerBase<CustomerController>
    {
        public CustomerController(ILogger<CustomerController> logger,
            IConfiguration config,
            IUserContext userService,
            ISender mediator) : base(logger, config, userService, mediator)
        {
        }

        [HttpGet("top-active-events")]
        [ProducesResponseType(typeof(List<GetActiveEventDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetActiveEvents()
        {
            var query = new GetTopActiveEventQuery();
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        [HttpGet("list-active-events")]
        [ProducesResponseType(typeof(PaginatedList<GetActiveEventDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllActiveEvents([FromQuery] RequestsPagination request)
        {
            var query = new GetAllActiveEventsQuery { PageNumber = request.PageNumber, PageSize = request.PageSize};
            var result = await _mediator.Send(query);
            return result.ToActionResult();            
        }

        [HttpGet("active-events/{id:Guid}")]
        [ProducesResponseType(typeof(EventItemsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEventDetails(Guid id)
        {
            var query = new GetCustomeEventQuery(id); 
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        [HttpPost("ticket-validation")]
        [ProducesResponseType(typeof(ValidateCustomerTicketResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidateCustomerTicket(List<ValidateCustomerTicketRequest> request)
        {
            var command = new ValidateCustomerTicketQuery(
                request.Select(r => new CustomerTicketItemsDetails
                {
                    Id = r.Id,
                    Qty = r.Qty
                }).ToList());

            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }



    }
}
