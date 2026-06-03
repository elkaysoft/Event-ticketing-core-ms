using ETS.Application.Events.Commands.Add;
using ETS.Application.Events.Commands.Delete;
using ETS.Application.Events.Commands.Update;
using ETS.Application.Events.Queries.Events;
using ETS.Application.Events.Queries.GetPagedEvents;
using ETS.Application.Events.Queries.GetSingleEvent;
using ETS.Domain.Common;
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
        [ProducesResponseType(typeof(AddEventsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEvent(EventRequest request)
        {
            var command = new AddEventCommand(request.Thumbnail,
                request.Title,
                request.Description,
                request.Location,
                request.EventDate,
                request.StartTime,
                request.EndTime,
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

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<EventItemsDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEvents([FromQuery] GetPaginatedEventFilter filter)
        {
            var query = new GetPagedEventsQuery(
                filter.SearchText,
                filter.StartDate,
                filter.EndDate,
                filter.PublishStatus,
                filter.SortField,
                filter.IsAscending);

            var result = await _mediator.Send(query);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("{eventId:Guid}")]
        [ProducesResponseType(typeof(EventItemsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEvent(Guid eventId)
        {
            var query = new GetSingleEventQuery(eventId);
            var result = await _mediator.Send(query);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("{eventId:Guid}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEvent(Guid eventId, [FromForm]UpdateEventRequest request)
        {
            var command = new UpdateEventCommand(request.Thumbnail,
                eventId,
                request.Title,
                request.Description,
                request.Location,
                request.EventDate,
                request.StartTime,
                request.EndTime,
                request.PublishStatus,
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

        [HttpDelete("{eventId:Guid}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteEvent(Guid eventId)
        {
            var command = new DeleteEventCommand(eventId);

            var result = await _mediator.Send(command);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }


    }
}
