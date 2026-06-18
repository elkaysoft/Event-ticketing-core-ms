using ETS.Application.Tickets.Queries.GetPagedTickets;
using ETS.Application.Tickets.Queries.GetSingleTicket;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Extensions;
using ETS.WebApi.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETS.WebApi.Endpoints.v1
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : AuthControllerBase<TicketsController>
    {
        public TicketsController(ILogger<TicketsController> logger, 
            IConfiguration config, 
            IUserContext userService, 
            ISender mediator) : base(logger, config, userService, mediator)
        {
        }


        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<CustomerTicketsDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTickets([FromQuery] GetPagedTicketsFilter filter)
        {
            var query = new GetPagedTicketsQuery(filter.SearchText,
                filter.StartDate,
                filter.EndDate,
                filter.SortField)
            {
                PageSize = filter.PageSize,
                PageNumber = filter.PageNumber
            };

            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        [HttpGet("{ticketId:Guid}")]
        [ProducesResponseType(typeof(GetSingleTicketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTicketById(Guid ticketId)
        {
            var query = new GetSingleTicketsQuery(ticketId);
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }


    }
}
