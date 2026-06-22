using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ETS.Application.Tickets.Queries.GetSingleTicket
{
    public record GetSingleTicketsQuery(Guid TicketId) : IQuery<GetSingleTicketDto>;

    public class GetSingleTicketQueryHandler : IQueryHandler<GetSingleTicketsQuery, GetSingleTicketDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetSingleTicketQueryHandler> _logger;

        public GetSingleTicketQueryHandler(IOrderRepository orderRepository, ILogger<GetSingleTicketQueryHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Result<GetSingleTicketDto>> Handle(GetSingleTicketsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var ticket = await _orderRepository.GetSingleAsync(x => x.Id == request.TicketId, 
                    cancellationToken, 
                    includeExpressions: [oi => oi.OrderItems]);

                if(ticket is null)
                {
                    return Result.Failure<GetSingleTicketDto>(TicketError.NotFound);
                }

                return new GetSingleTicketDto
                {
                    FullName = ticket.FullName,
                    DateRegistered = ticket.CreatedAt,
                    EmailAddress= ticket.EmailAddress,
                    PhoneNumber = ticket.PhoneNumber,
                    Address = ticket.Event.Location,
                    RedemptionStatus = ticket.RedemptionStatus,
                    PaymentStatus = ticket.OrderStatus,
                    Details = ticket.OrderItems.Select(x => new GetSingleTicketDetails
                    {
                        Category = x.Title,
                        Id = x.Id.ToString(),
                        Qty = x.Unit,
                        Status = ticket.OrderStatus
                    }).ToList()
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occured at {nameof(GetSingleTicketQueryHandler)}");
                return Result.Failure<GetSingleTicketDto>(TicketError.SomethingWentWrong);
            }
        }
    }

}
