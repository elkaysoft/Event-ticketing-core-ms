using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Repositories;

namespace ETS.Application.Events.Queries.GetEventSummary
{
    public record GetEventSummaryQuery() : IQuery<EventSummaryDto>;

    public class GetEventSummaryQueryHandler : IQueryHandler<GetEventSummaryQuery, EventSummaryDto>
    {
        private readonly IEventRepository _eventRepository;

        public GetEventSummaryQueryHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<Result<EventSummaryDto>> Handle(GetEventSummaryQuery request, CancellationToken cancellationToken)
        {
            var eventSummary = await _eventRepository.GetAllAsync(x => x.IsDeleted == false);
            var publishedEvent = eventSummary.Count(x => x.PublishStatus == Domain.Enums.PublishStatus.Published);
            var draftedEvent = eventSummary.Count(x => x.PublishStatus == Domain.Enums.PublishStatus.Draft);
            var completedEvent = eventSummary.Count (x => x.PublishStatus == Domain.Enums.PublishStatus.Completed);
            var cancelledEvent = eventSummary.Count(x => x.PublishStatus == Domain.Enums.PublishStatus.Cancelled);

            return new EventSummaryDto 
            {
                CancelledCount = cancelledEvent,
                CompletedCount = completedEvent,
                DraftedCount = draftedEvent,
                PublishedCount = publishedEvent,
            };
        }
    }
}
