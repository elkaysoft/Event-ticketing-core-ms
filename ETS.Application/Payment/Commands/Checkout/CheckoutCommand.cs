using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Errors;
using ETS.Domain.Models.Paystack.Request;
using ETS.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace ETS.Application.Payment.Commands.Checkout
{
    public record CheckoutListDto(Guid TicketId, int Unit);    

    public record CheckoutCommand(string FullName, 
        string PhoneNumber,
        string EmailAddress,
        List<CheckoutListDto> TicketDetail) : ICommand<CheckoutCommandResponse>;

    public class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
    {
        public CheckoutCommandValidator()
        {
            RuleFor(x => x.FullName).NotNull().NotEmpty().WithMessage("FullName is required");
            RuleFor(x => x.PhoneNumber).NotNull().WithMessage("Phone Number is required");
            RuleFor(x => x.EmailAddress).NotNull().WithMessage("Email Address is required");

            RuleFor(x => x.TicketDetail)
                .NotEmpty().WithMessage("Checkout list cannot be empty.")
                .Must(list => list.All(item => item.Unit > 0)).WithMessage("Unit must be greater than zero for all items.");
        }
    }

    public class CheckoutCommandHandler : ICommandHandler<CheckoutCommand, CheckoutCommandResponse>
    {
        private readonly IEventCategoryRepository _eventCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IPaystackService _paystackService;
        private readonly IConfiguration _configuration;

        public CheckoutCommandHandler(IEventCategoryRepository eventCategoryRepository,
            IUnitOfWork unitOfWork,
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IPaystackService paystackService,
            IConfiguration configuration)
        {
            _eventCategoryRepository = eventCategoryRepository;
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _paystackService = paystackService;
            _configuration = configuration;
        }

        public async Task<Result<CheckoutCommandResponse>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            var ticketCategoryIds = request.TicketDetail.Select(x => x.TicketId).ToList();
            var eventCategories = await _eventCategoryRepository.GetEventCategoriesByIds(ticketCategoryIds, cancellationToken);
            if (!eventCategories.Any())
            {
                return Result.Failure<CheckoutCommandResponse>(CustomerErrors.InvalidEventId);
            }

            string orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            decimal subTotal = 0;
            
            var orderDetails = new List<OrderItem>();
            foreach (var item in request.TicketDetail)
            {
                var eventCategory = eventCategories.FirstOrDefault(x => x.Id == item.TicketId);
                if (eventCategory is null) continue;

                subTotal += eventCategory.Price * item.Unit;

                var orderDetail = OrderItem.Create(Guid.NewGuid(), eventCategory.Id, item.Unit, eventCategory.Price);
                orderDetails.Add(orderDetail);
            }
            decimal taxAmount = 5.5m;
            decimal totalAmount = subTotal + taxAmount;

            // proceed to initialize transaction on paystack
            decimal paymentAmount = totalAmount * 100;            
            var intializePaymentResult = await _paystackService.InitializeTransactionAsync(new InitiateTransactionRequest
            {
                Amount = paymentAmount.ToString(),
                EmailAddress = request.EmailAddress,
                Reference = orderNumber
            }, cancellationToken);

            if (intializePaymentResult.IsFailure)
            {
                return Result.Failure<CheckoutCommandResponse>(TransactionErrors.UnableToProcessPayment);
            }

            int totalTickets = orderDetails.Sum(x => x.Unit);
            var order = Order.Create(request.FullName,
                request.EmailAddress,
                request.PhoneNumber,
                orderNumber,
                taxAmount,
                subTotal,
                totalAmount,
                intializePaymentResult.Value.access_code,
                totalTickets);
            _orderRepository.Add(order);

            foreach (var item in orderDetails)
            {
                var orderItem = OrderItem.Create(order.Id, item.EventCategoryId, item.Unit, item.UnitPrice);
                _orderItemRepository.Add(orderItem);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CheckoutCommandResponse 
            { 
                access_code = intializePaymentResult.Value.access_code,
                reference = intializePaymentResult.Value.reference 
            };
        }


    }
}
