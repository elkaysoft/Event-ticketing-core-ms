using ETS.Application.Events.Commands.Add;
using ETS.Application.Users.Commands.RegisterUser;
using ETS.Domain.Contracts;
using ETS.WebApi.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETS.WebApi.Endpoints.v1
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : AuthControllerBase<EventController>
    {
        public EventController(ILogger<EventController> logger,
            IConfiguration config,
            IUserContext userContext,
            ISender mediator) : base(logger, config, userContext, mediator)
        {
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser(EventRequest request)
        {
            var command = new AddEventCommand(request.Thumbnail,
                request.Title, 
                request.Description, 
                request.Location, 
                request.EventDate, 
                request.StartTime,
                "11",
                request.EventCategories.Select(x => new Application.Events.Commands.Add.EventCategoryRequest 
                { 
                    Price = x.Price, 
                    Qty = x.Qty, 
                    Title = x.Title
                }).ToList());

            var result = await _mediator.Send(command);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
