using ETS.Application.Events.Queries.GetActiveEvent;
using ETS.Application.Events.Queries.GetAllActiveEvents;
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
        public async Task<IActionResult> GetActiveEvents()
        {
            var query = new GetTopActiveEventQuery();
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        [HttpGet("list-active-events")]
        public async Task<IActionResult> GetAllActiveEvents([FromQuery] RequestsPagination request)
        {
            var query = new GetAllActiveEventsQuery { PageNumber = request.PageNumber, PageSize = request.PageSize};
            var result = await _mediator.Send(query);
            return result.ToActionResult();            
        }


    }
}
