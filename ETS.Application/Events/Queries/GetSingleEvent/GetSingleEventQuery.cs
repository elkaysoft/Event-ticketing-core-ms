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
        private readonly IEventCategoryRepository _eventCategoryRepository;
        private readonly IOrderItemRepository _orderItemRepository;

        public GetSingleEventQueryHandler(IEventRepository eventRepository,
            IEventCategoryRepository eventCategoryRepository,
            IOrderItemRepository orderItemRepository)
        {
            _eventRepository = eventRepository;
            _eventCategoryRepository = eventCategoryRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<Result<EventItemsDto>> Handle(GetSingleEventQuery request, CancellationToken cancellationToken)
        {
            var eventResult = await _eventRepository.GetSingleAsync(x => x.Id == request.EventId, cancellationToken, p => p.EventCategories);
            if(eventResult is null)
            {
                return Result.Failure<EventItemsDto>(EventErrors.NotFound);
            }

            var eventCategories = await _eventCategoryRepository.GetAllAsync(x => x.EventId == request.EventId, cancellationToken);
            var ticketIds = eventCategories.Select(x => x.Id).ToList();
            var tickets = await _orderItemRepository.GetOrderItemsByIds(ticketIds, cancellationToken);

            int totalTickets = eventCategories.Sum(x => x.Qty);
            int totalSold = tickets.Sum(x => x.Unit);
            int ticketsAvailable = totalTickets - totalSold;
            decimal amountSold = tickets.Sum(x => (x.UnitPrice * x.Unit));            

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
                }).ToList(),
                Summary = new EventTicketStatistics
                {
                    TotalTickets = totalTickets,
                    TotalSold = totalSold,
                    AmountSold = amountSold,
                    TicketsAvailable = ticketsAvailable
                }
            };

            return eventDto;
        }


    }
}
