using ETS.Application.Abstraction.Common;
using ETS.Application.Abstraction.Mediation;
using ETS.Application.Events.Queries.GetActiveEvent;
using ETS.Domain.Common;
using ETS.Domain.Repositories;
using System.Linq.Expressions;

namespace ETS.Application.Events.Queries.GetAllActiveEvents
{
    public record GetAllActiveEventsQuery() : PaginationQuery, IQuery<PaginatedList<GetActiveEventDto>>;

    public class GetAllActiveEventsQueryHandler : IQueryHandler<GetAllActiveEventsQuery, PaginatedList<GetActiveEventDto>>
    {
        private readonly IEventRepository _eventRepository;

        public GetAllActiveEventsQueryHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        private Expression<Func<Domain.Entities.Events, GetActiveEventDto>> Selector()
        {
            return v => new GetActiveEventDto
            {
                Id = v.Id,
                Description = v.Description,
                EventDate = v.EventDate,
                EndTime = v.EndTime,
                BannerUrl = v.BannerUrl,
                Title = v.Title,
                StartTime = v.KickoffTime,
                Location = v.Location                
            };
        }

        private static Expression<Func<Domain.Entities.Events, bool>> GetQueryExpression(GetAllActiveEventsQuery request) => u =>
           u.PublishStatus == Domain.Enums.PublishStatus.Published
                        && u.EventDate >= DateTime.UtcNow;



        public async Task<Result<PaginatedList<GetActiveEventDto>>> Handle(GetAllActiveEventsQuery request, CancellationToken cancellationToken)
        {
            var filter = GetQueryExpression(request);

            var events = await _eventRepository.GetPaginatedAsync<GetActiveEventDto>(
                filter,
                Selector(),
                request.PageNumber,
                request.PageSize,
                null!,
                false,
                cancellationToken: cancellationToken);

            return events;
        }
    }

}
