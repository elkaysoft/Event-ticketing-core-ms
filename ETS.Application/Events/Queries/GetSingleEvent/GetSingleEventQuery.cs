using ETS.Application.Abstraction.Mediation;
using ETS.Application.Events.Queries.Events;
using ETS.Domain.Common;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;

namespace ETS.Application.Events.Queries.GetSingleEvent
{
    public record GetSingleEventQuery(Guid EventId) : IQuery<EventItemsDto>;

    public class GetSingleEventQueryHandler : IQueryHandler<GetSingleEventQuery, EventItemsDto>
    {
        private readonly IEventRepository _eventRepository;

        public GetSingleEventQueryHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<Result<EventItemsDto>> Handle(GetSingleEventQuery request, CancellationToken cancellationToken)
        {
            var eventResult = await _eventRepository.GetSingleAsync(x => x.Id == request.EventId, cancellationToken, p => p.EventCategories);
            if(eventResult is null)
            {
                return Result.Failure<EventItemsDto>(EventErrors.NotFound);
            }

            var eventDto = new EventItemsDto
            {
                Id = eventResult.Id,
                Title = eventResult.Title,
                Description = eventResult.Description,
                Location = eventResult.Location,
                EventDate = eventResult.EventDate,
                StartTime = eventResult.KickoffTime,
                EndTime = eventResult.EndTime,
                BannerUrl = eventResult.BannerUrl,
                PublishStatus = eventResult.PublishStatus,
                EventCategories = eventResult.EventCategories.Select(x => new EventCategoryItemsDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Price = x.Price,
                    Qty = x.Qty
                }).ToList()
            };

            return eventDto;
        }


    }
}
