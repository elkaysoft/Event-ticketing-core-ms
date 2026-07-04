using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ETS.Application.Tickets.Queries.VerifyTicket
{
    public record VerifyTicketQuery(string QRCodeReference) : IQuery<VerifyTicketResponse>;

    public class VerifyTicketQueryHandler(IOrderItemRepository _orderItemRepository,
        IUnitOfWork _unitOfWork,
        ILogger<VerifyTicketQueryHandler> _logger) 
        : IQueryHandler<VerifyTicketQuery, VerifyTicketResponse>
    {
        public async Task<Result<VerifyTicketResponse>> Handle(VerifyTicketQuery request, 
            CancellationToken cancellationToken)
        {
            try
            {
                var orderItem = await _orderItemRepository.GetSingleAsync(x => x.QRCodeReference == request.QRCodeReference,
                    cancellationToken,
                    [o => o.Order, ev => ev.Order.Event]);

                if(orderItem is null)
                {
                    return Result.Failure<VerifyTicketResponse>(TicketError.NotFound);
                }

                // check for payment completion
                if(orderItem.PaymentStatus != Domain.Enums.OrderStatus.Completed)
                {
                    return new VerifyTicketResponse { IsValid = false, Message = "This ticket has not been paid for." };
                }

                // check for ticket redemption
                if (orderItem.RedemptionStatus == Domain.Enums.TicketStatus.Redeemed)
                {
                    return new VerifyTicketResponse 
                    {
                        IsValid = false, 
                        Message = "This ticket has already been used.",
                        RedemptionStatus = Domain.Enums.TicketStatus.Redeemed,
                        EventDate = orderItem.Order.Event.EventDate,
                        EventTitle = orderItem.Order.EventName,
                        TicketCategory = orderItem.Title,
                        TicketHolder = orderItem.Order.FullName
                    };
                }

                orderItem.ChangeRedemptionStatus(Domain.Enums.TicketStatus.Redeemed);


                try
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Another request redeemed this ticket between our read and our write
                    // Reload the current state from DB and return already-used response
                    await _orderItemRepository.ReloadAsync(orderItem, cancellationToken);

                    return new VerifyTicketResponse
                    {
                        IsValid = false,
                        Message = "This ticket has already been used.",
                        RedemptionStatus = Domain.Enums.TicketStatus.Redeemed,
                        EventDate = orderItem.Order.Event.EventDate,
                        EventTitle = orderItem.Order.EventName,
                        TicketCategory = orderItem.Title,
                        TicketHolder = orderItem.Order.FullName
                    };
                }


                return new VerifyTicketResponse
                {
                    IsValid = true,
                    RedemptionStatus = Domain.Enums.TicketStatus.Redeemed,
                    EventDate = orderItem.Order.Event.EventDate,
                    EventTitle = orderItem.Order.EventName,
                    Message = "Ticket verified successfully. Welcome!",
                    TicketCategory = orderItem.Title,
                    TicketHolder = orderItem.Order.FullName
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in {Handler} for reference {Reference}",
                nameof(VerifyTicketQuery), request.QRCodeReference);
                return Result.Failure<VerifyTicketResponse>(TicketError.SomethingWentWrong);
            }        
        }

    } 
}
