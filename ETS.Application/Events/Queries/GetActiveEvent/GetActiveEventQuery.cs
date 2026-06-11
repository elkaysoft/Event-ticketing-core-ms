using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;

namespace ETS.Application.Events.Queries.GetActiveEvent
{
    public record GetActiveEventQuery : IQuery<List<GetActiveEventDto>>;

    public class GetActiveEventQueryHandler : IQueryHandler<GetActiveEventQuery, List<GetActiveEventDto>>
    {
        private readonly IEventRepository _eventRepository;

        public GetActiveEventQueryHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<Result<List<GetActiveEventDto>>> Handle(GetActiveEventQuery request, CancellationToken cancellationToken)
        {
            var activeEvents = await _eventRepository.GetActiveEvents();
            if (activeEvents == null || !activeEvents.Any())
            {
                return Result.Failure<List<GetActiveEventDto>>(EventErrors.NoActiveEventFound);
            }

            var activeEventDtos = activeEvents.Select(activeEvent => new GetActiveEventDto
            {
                Description = activeEvent.Description,
                StartTime = activeEvent.KickoffTime,
                EndTime = activeEvent.EndTime,
                BannerUrl = activeEvent.BannerUrl,
                EventDate = activeEvent.EventDate,
                Id = activeEvent.Id,
                Location = activeEvent.Location,
                Title = activeEvent.Title,
                TicketCategories = activeEvent.EventCategories.Select(ec => new TicketCategoryDto
                {
                    TicketId = ec.Id,
                    Title = ec.Title,
                    Price = ec.Price,
                }).ToList()
            }).ToList();

            return Result<List<GetActiveEventDto>>.Success(activeEventDtos);
        }
    }                    
}
