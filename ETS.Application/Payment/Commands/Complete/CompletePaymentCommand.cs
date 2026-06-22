using Azure.Core;
using ETS.Application.Abstraction.Mediation;
using ETS.Domain.AppConfig;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Enums;
using ETS.Domain.Extensions;
using ETS.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ETS.Application.Payment.Commands.Complete
{
    public class Plan
    {
    }
    public class History
    {
        public string type { get; set; }
        public string message { get; set; }
        public int time { get; set; }
    }
    public class Authorization
    {
        public string authorization_code { get; set; }
        public string bin { get; set; }
        public string last4 { get; set; }
        public string exp_month { get; set; }
        public string exp_year { get; set; }
        public string card_type { get; set; }
        public string bank { get; set; }
        public string country_code { get; set; }
        public string brand { get; set; }
        public string account_name { get; set; }
    }
    public class Log
    {
        public int time_spent { get; set; }
        public int attempts { get; set; }
        public string authentication { get; set; }
        public int errors { get; set; }
        public bool success { get; set; }
        public bool mobile { get; set; }
        public List<object> input { get; set; }
        public object channel { get; set; }
        public List<History> history { get; set; }
    }
    public class Customer
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string customer_code { get; set; }
        public object phone { get; set; }
        public object metadata { get; set; }
        public string risk_action { get; set; }
    }
    public class Data
    {
        public int id { get; set; }
        public string domain { get; set; }
        public string status { get; set; }
        public string reference { get; set; }
        public int amount { get; set; }
        public object message { get; set; }
        public string gateway_response { get; set; }
        public DateTime paid_at { get; set; }
        public DateTime created_at { get; set; }
        public string channel { get; set; }
        public string currency { get; set; }
        public string ip_address { get; set; }
        public int metadata { get; set; }
        public Log log { get; set; }
        public object fees { get; set; }
        public Customer customer { get; set; }
        public Authorization authorization { get; set; }
        public Plan plan { get; set; }
    }


    public class CompletePaymentCommand : ICommand<bool>
    {
        public string @event { get; set; }
        public Data data { get; set; }
    }


    public class CompletePaymentCommandHandler : ICommandHandler<CompletePaymentCommand, bool>
    {
        private readonly ILogger<CompletePaymentCommandHandler> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly PaystackConfigOptions _config;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventRepository _eventRepository;
        private readonly IEventCategoryRepository _eventCategoryRepository;

        public CompletePaymentCommandHandler(ILogger<CompletePaymentCommandHandler> logger,
            IOrderRepository orderRepository,
            IOptions<PaystackConfigOptions> config,
            IHttpContextAccessor contextAccessor,
            IUnitOfWork unitOfWork,
            IEventCategoryRepository eventCategoryRepository,
            IEventRepository eventRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _config = config.Value;
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
            _eventCategoryRepository = eventCategoryRepository;
            _eventRepository = eventRepository;
        }

        public async Task<Result<bool>> Handle(CompletePaymentCommand request, CancellationToken cancellationToken)
        {
            var httpRequest = _contextAccessor?.HttpContext?.Request;

            if (httpRequest?.Headers.TryGetValue("x-paystack-signature", out StringValues pstkSignatureHeader) != true)
            {
                _logger.LogWarning($"Paystack signature was not found for {request.data.reference}");
                return false;
            }

            // rewind the stream to the beginning and read the exact raw string received
            httpRequest.Body.Position = 0;
            using var reader = new StreamReader(httpRequest.Body, Encoding.UTF8, leaveOpen: true);
            string rawJsonBody = await reader.ReadToEndAsync();

            string computedHash = Cryptography.ComputeHmacSha512(rawJsonBody, _config.ApiSecret);


            // Convert the two strings (computed hash and paystack header) to byte arrays for a secure comparison
            byte[] computedBytes = Encoding.UTF8.GetBytes(computedHash);
            byte[] receivedBytes = Encoding.UTF8.GetBytes(pstkSignatureHeader.ToString());

            // Use FixedTimeEquals to protect against timing attacks
            if(!CryptographicOperations.FixedTimeEquals(computedBytes, receivedBytes))
            {
                _logger.LogWarning($"Signature mismatch found for {request.data.reference}");
                return false;
            }

            var orderResult = await _orderRepository.GetSingleAsync(x => x.OrderNumber == request.data.reference);
            if (orderResult == null)
            {
                _logger.LogWarning($"Order not found for {request.data.reference}");
                return false;
            }

            var orderStatus = request.data.status.ToLower() == "success" ? OrderStatus.Completed : OrderStatus.Cancelled;
            
            // reverse/return the unit sold, since the payment failed or cancelled
            if(orderStatus == OrderStatus.Cancelled)
            {
                var eventCategories = await _eventCategoryRepository.GetAllAsync(x => x.EventId == orderResult.EventId, 
                    cancellationToken);
                foreach(var category in eventCategories)
                {
                    category.RemoveFromUnitSold(category.Qty);
                }
            }            
            
            orderResult.UpdateStatus(orderStatus);                        
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogWarning($"Payment successfully processed for {request.data.reference} with Payment status: [{orderResult.ToString()}]");

            return true;
        }

    }
}
