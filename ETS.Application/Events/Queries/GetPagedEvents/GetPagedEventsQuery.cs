using ETS.Application.Abstraction.Common;
using ETS.Application.Abstraction.Mediation;
using ETS.Application.Events.Queries.Events;
 using ETS.Domain.Common;
using ETS.Domain.Enums;
using ETS.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ETS.Application.Events.Queries.GetPagedEvents
{
    public record GetPagedEventsQuery(string? SearchText,
        DateTime? StartDate,
        DateTime? EndDate,
        PublishStatus? PublishStatus,
        string? SortField = "CreatedAt",
        bool IsAscending = false): PaginationQuery, IQuery<PaginatedList<EventItemsDto>>;

    public class GetPagedEventsQueryHandler : IQueryHandler<GetPagedEventsQuery, PaginatedList<EventItemsDto>>
    {
        private readonly IEventRepository _eventRepository;

        public GetPagedEventsQueryHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        private static Expression<Func<Domain.Entities.Events, object>> GetSortProperty(GetPagedEventsQuery request) =>
           request.SortField?.ToLower() switch
           {
               "id" => p => p.Id,
               "title" => p => p.Title,
               "location" => p => p.Location,
               "startdate" => p => p.EventDate,
               _ => p => p.CreatedAt
           };


        private static Expression<Func<Domain.Entities.Events, bool>> GetQueryExpression(GetPagedEventsQuery request) => u =>
           (request.StartDate == null || u.EventDate >= request.StartDate.Value) &&
           (request.EndDate == null || u.EventDate <= request.EndDate.Value.AddDays(1).AddMinutes(-1) &&
           (string.IsNullOrWhiteSpace(request.SearchText) ||
               EF.Functions.Like(u.Title, $"%{request.SearchText}%") ||
               EF.Functions.Like(u.Location, $"%{request.SearchText}%")));

        private Expression<Func<Domain.Entities.Events, EventItemsDto>> Selector()
        {
            return v => new EventItemsDto
            {
                Id = v.Id,
                DateCreated = v.CreatedAt,
                Description = v.Description,
                EventDate = v.EventDate,
                EndTime = v.EndTime,
                BannerUrl = v.BannerUrl,    
                Title = v.Title,
                StartTime = v.KickoffTime,
                Location = v.Location
            };
        }


        public async Task<Result<PaginatedList<EventItemsDto>>> Handle(GetPagedEventsQuery request, CancellationToken cancellationToken)
        {
            var filter = GetQueryExpression(request);
            var sort = GetSortProperty(request);

            var events = await _eventRepository.GetPaginatedAsync<EventItemsDto>(
                filter,
                Selector(),
                request.PageNumber,
                request.PageSize,
                sort,
                request.IsAscending,
                cancellationToken: cancellationToken);

            return events;
        }


        //private async Task<int> GetTicketSoldByEvent()
        //{

        //}
    }

}
