using ETS.Application.Events.Queries.GetActiveEvent;
using ETS.Domain.Contracts;
using ETS.WebApi.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ETS.WebApi.Endpoints.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : AuthControllerBase<CustomerController>
    {
        public CustomerController(ILogger<CustomerController> logger,
            IConfiguration config,
            IUserContext userService,
            ISender mediator) : base(logger, config, userService, mediator)
        {
        }

        [HttpGet("active-events")]
        public async Task<IActionResult> GetActiveEvents()
        {
            var query = new GetActiveEventQuery();
            var result = await _mediator.Send(query);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

       

    }
}
