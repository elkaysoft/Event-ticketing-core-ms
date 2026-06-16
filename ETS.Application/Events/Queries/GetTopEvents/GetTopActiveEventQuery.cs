using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ETS.Application.Events.Queries.GetActiveEvent
{
    public record GetTopActiveEventQuery : IQuery<List<GetActiveEventDto>>;

    public class GetTopActiveEventQueryHandler : IQueryHandler<GetTopActiveEventQuery, List<GetActiveEventDto>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<GetTopActiveEventQueryHandler> _logger;

        public GetTopActiveEventQueryHandler(IEventRepository eventRepository, ILogger<GetTopActiveEventQueryHandler> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<Result<List<GetActiveEventDto>>> Handle(GetTopActiveEventQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var activeEvents = await _eventRepository.GetActiveEvents(cancellationToken);
                if (activeEvents == null || !activeEvents.Any())
                {
                    return Result.Failure<List<GetActiveEventDto>>(EventErrors.NoActiveEventFound);
                }

                return activeEvents.Select(e => e.ToDto()).ToList();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in {Handler}", nameof(GetTopActiveEventQuery));
                return Result.Failure<List<GetActiveEventDto>>(EventErrors.SomethingWentWrong);                
            }
        }
    }                    
}
