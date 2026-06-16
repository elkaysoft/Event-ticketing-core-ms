using ETS.Application.Events.Commands.Add;
using ETS.Application.Events.Commands.Delete;
using ETS.Application.Events.Commands.Update;
using ETS.Application.Events.Commands.UpdateEventCategory;
using ETS.Application.Events.Queries.Events;
using ETS.Application.Events.Queries.GetEventSummary;
using ETS.Application.Events.Queries.GetPagedEvents;
using ETS.Application.Events.Queries.GetSingleEvent;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Extensions;
using ETS.WebApi.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
                request.PublishStatus,
                request.EventCategories.Select(x => new Application.Events.Commands.Add.EventCategoryRequest
                {
                    Price = x.Price,
                    Qty = x.Qty,
                    Title = x.Title
                }).ToList());

            var result = await _mediator.Send(command);
            return result.ToActionResult();
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
            return result.ToActionResult();
        }

        [HttpGet("{eventId:Guid}")]
        [ProducesResponseType(typeof(EventItemsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEvent(Guid eventId)
        {
            var query = new GetSingleEventQuery(eventId);
            var result = await _mediator.Send(query);
            return result.ToActionResult();
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
            return result.ToActionResult();
        }

        [HttpDelete("{eventId:Guid}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteEvent(Guid eventId)
        {
            var command = new DeleteEventCommand(eventId);

            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpGet("summary")]
        [ProducesResponseType(typeof(EventSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventSummary()
        {
            var query = new GetEventSummaryQuery();

            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }


        [HttpPut("{categoryId:Guid}/event-category")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEventCategory(Guid categoryId, [FromBody] UpdateEventCategoryRequest request)
        {
            var command = new UpdateEventCategoryCommand(categoryId,
                request.Title,
                request.Price,
                request.Qty);

            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

    }
}
