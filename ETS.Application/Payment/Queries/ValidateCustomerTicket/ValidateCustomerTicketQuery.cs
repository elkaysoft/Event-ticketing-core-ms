using ETS.Application.Abstraction.Mediation;
using ETS.Domain.AppConfig;
using ETS.Domain.Common;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETS.Application.Payment.Queries.ValidateCustomerTicket
{
    public class CustomerTicketItemsDetails
    {
        public Guid Id { get; set; }
        public int Qty { get; set; }

    }

    public record ValidateCustomerTicketQuery(List<CustomerTicketItemsDetails> items) 
        : IQuery<ValidateCustomerTicketResponse>;


    public class ValidateCustomerTicketValidator : AbstractValidator<ValidateCustomerTicketQuery>
    {
        public ValidateCustomerTicketValidator()
        {
            RuleFor(x => x.items)
               .NotNull().WithMessage("Ticket detail is required")
               .NotEmpty().WithMessage("Ticket detail must contain at least one item")
               .Must(items => items.Select(i => i.Id).Distinct().Count() == items.Count)
               .WithMessage("Ticket details must not contain duplicate event Id");

            RuleForEach(x => x.items)
                .SetValidator(new ValidateCustomerTicketDetailsValidator());
        }
    }

    public class ValidateCustomerTicketDetailsValidator : AbstractValidator<CustomerTicketItemsDetails>
    {
        public ValidateCustomerTicketDetailsValidator()
        {
            RuleFor(x => x.Qty).GreaterThan(0).WithMessage("Qty must be greater than zero");
        }
    }

    public class ValidateCustomerTicketQueryHandler :
        IQueryHandler<ValidateCustomerTicketQuery, ValidateCustomerTicketResponse>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventCategoryRepository _eventCategoryRepository;
        private readonly ILogger<ValidateCustomerTicketQueryHandler> _logger;
        private readonly PaystackConfigOptions _paystackConfigOption;
        const int CapFeeAmount = 2000;
        const int MinTransactionAmount = 2500;
        const int PstkFee = 100;

        public ValidateCustomerTicketQueryHandler(IEventRepository eventRepository,
            ILogger<ValidateCustomerTicketQueryHandler> logger,
            IEventCategoryRepository eventCategoryRepository,
            IOptions<PaystackConfigOptions> paystackConfigOptions)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _eventCategoryRepository = eventCategoryRepository;
            _paystackConfigOption = paystackConfigOptions.Value;
        }

        public async Task<Result<ValidateCustomerTicketResponse>> Handle(ValidateCustomerTicketQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var ticketIds = request.items.Select(x => x.Id).ToList();
                var tickets = await _eventCategoryRepository.GetEventCategoriesByIds(ticketIds, cancellationToken);
                if (!tickets.Any()) 
                {
                    return Result.Failure<ValidateCustomerTicketResponse>(EventErrors.NoActiveEventFound);
                }

                var result = new ValidateCustomerTicketResponse();
                int errorCount = 0;
                var charge = new ChargeDetails();

                foreach (var item in request.items)
                {
                    var ticket = tickets.FirstOrDefault(x => x.Id == item.Id)!;
                    int availableTicket = ticket.Qty - ticket.UnitSold;
                    bool hasError = item.Qty > availableTicket;

                    var response = new ValidateCustomerTicketResponseDetails
                    {
                        RequestedQty = item.Qty,
                        Id = item.Id,
                        HasError = hasError,
                        AvailableQty = availableTicket                        
                    };                    
                    result.details.Add(response);
                    if(hasError)
                       errorCount++;
                }

                charge.SubTotal = tickets.Where(t => request.items.Any(i => i.Id == t.Id))
                    .Sum(t => t.Price * request.items.First(i => i.Id == t.Id).Qty);

                decimal taxAmount = (_paystackConfigOption.Fee / 100) * charge.SubTotal;
                charge.Tax = GetCharge(charge.SubTotal);
                charge.Total = charge.SubTotal + charge.Tax;

                result.Charge = charge;
                result.StatusCode = errorCount > 0 ? "02" : "00";
                result.StatusMessage = errorCount > 0 ? "The requested has some validation error" 
                    : "No validation error";

                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occured at {nameof(ValidateCustomerTicketQueryHandler)}");
                return Result.Failure<ValidateCustomerTicketResponse>(EventErrors.SomethingWentWrong);
            }
        }

        private decimal GetCharge(decimal subTotal)
        {
            decimal fee = (_paystackConfigOption.Fee / 100) * subTotal;
            decimal charge = Math.Min(fee, CapFeeAmount);
            if (subTotal > MinTransactionAmount)
                charge += PstkFee;
            return charge;
        }


    }
}
