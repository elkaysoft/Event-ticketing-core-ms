using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ETS.Application.Events.Queries.GetCustomerEvent
{
    public record GetCustomeEventQuery(Guid Id) : IQuery<CustomerEventDto>;

    public class GetCustomerEventQueryHandler : IQueryHandler<GetCustomeEventQuery, CustomerEventDto>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<GetCustomerEventQueryHandler> _logger;

        public GetCustomerEventQueryHandler(IEventRepository eventRepository,
            ILogger<GetCustomerEventQueryHandler> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<Result<CustomerEventDto>> Handle(GetCustomeEventQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var theEvent = await _eventRepository.GetSingleAsync(x => x.Id == request.Id, 
                    cancellationToken,
                    includeExpressions: p => p.EventCategories);

                if (theEvent == null)
                {
                    return Result.Failure<CustomerEventDto>(EventErrors.NotFound);
                }

                return new CustomerEventDto
                {
                    Description = theEvent.Description,
                    StartTime = theEvent.KickoffTime,
                    EndTime = theEvent.EndTime,
                    BannerUrl = theEvent.BannerUrl,
                    EventDate = theEvent.EventDate,
                    Id = theEvent.Id,
                    Location = theEvent.Location,
                    Title = theEvent.Title,
                    Items = theEvent.EventCategories.Select(x => new CustomerEventItemsDto
                    {
                        AvailableUnit = x.Qty - x.UnitSold,
                        Id = x.Id,
                        Price = x.Price,
                        Title = x.Title
                    }).ToList()
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occured at {nameof(GetCustomerEventQueryHandler)}");
                return Result.Failure<CustomerEventDto>(EventErrors.SomethingWentWrong);
            }
        }

    }
}
