using ETS.Application.Abstraction.Mediation;
using ETS.Domain.AppConfig;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Enums;
using ETS.Domain.Extensions;
using ETS.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ETS.Application.Payment.Commands.Complete
{
    public class PaystackWebhookRequestData
    {
        public long id { get; set; }
        public string domain { get; set; }
        public string status { get; set; }
        public string reference { get; set; }
        public long amount { get; set; }
        public string channel { get; set; } // Will be "card" or "bank_transfer"
        public object message { get; set; }
        public string gateway_response { get; set; }
        public string gateway_response_code { get; set; }
        public string response_code { get; set; }
        public DateTime paid_at { get; set; }
        public DateTime created_at { get; set; }
        public string currency { get; set; }
        public string ip_address { get; set; }
        public Metadata metadata { get; set; }
        public object fees_breakdown { get; set; }
        public object log { get; set; }
        public int fees { get; set; }
        public object fees_split { get; set; }
        public Authorization authorization { get; set; }
        public CustomerInfo customer { get; set; }
        public Plan plan { get; set; }
        public Subaccount subaccount { get; set; }
        public Split split { get; set; }
        public object order_id { get; set; }
        public DateTime paidAt { get; set; }
        public int requested_amount { get; set; }
        public object pos_transaction_data { get; set; }
        public Source source { get; set; }
    }
    public class Source
    {
        public string type { get; set; }
        public string source { get; set; }
        public string entry_point { get; set; }
        public object identifier { get; set; }
    }
    public class Split
    {
    }
    public class Subaccount
    {
    }
    public class Metadata
    {
        [JsonPropertyName("cancel_action")]
        public string CancelAction { get; set; }
        public string referrer { get; set; }
    }
    public class Plan
    {
    }
    public class Authorization
    {
        [JsonPropertyName("authorization_code")]
        public string AuthorizationCode { get; set; }
        public string bin { get; set; }
        public string last4 { get; set; }
        public string exp_month { get; set; }
        public string exp_year { get; set; }
        public string channel { get; set; }
        [JsonPropertyName("card_type")]
        public string CardType { get; set; }
        public string bank { get; set; }
        public string country_code { get; set; }
        public string brand { get; set; }
        public bool reusable { get; set; }
        public string signature { get; set; }
        public object account_name { get; set; }
        [JsonPropertyName("receiver_bank_account_number")]
        public string ReceiverBankAccountNumber { get; set; }
        [JsonPropertyName("receiver_bank")]
        public string ReceiverBank { get; set; }
    }

    
    public class CustomerInfo
    {
        public int id { get; set; }
        public object first_name { get; set; }
        public object last_name { get; set; }
        public string email { get; set; }
        public string customer_code { get; set; }
        public object phone { get; set; }
        public object metadata { get; set; }
        public string risk_action { get; set; }
        public object international_format_phone { get; set; }
    }
    
    public class PaystackChargeRequest
    {
        [JsonPropertyName("event")]
        public string Event { get; set; } = string.Empty;
        [JsonPropertyName("data")]
        public PaystackWebhookRequestData Data { get; set; }
    }

    public record CompletePaymentCommand(string requestBody) : ICommand<bool>;
    


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

            // verify x-paystack-signature header
            if (httpRequest?.Headers.TryGetValue("x-paystack-signature", out StringValues signature) != true)
            {
                return Result.Failure<bool>(new Error("Payment.SignatureNotFound", "Signature could not be found in the header"));
            }

            _logger.LogInformation($"Paystack SignatureKey {signature.ToString()}");                                    

            if(!IsSignatureValid(request.requestBody, signature.ToString()))
            {
                return Result.Failure<bool>(new Error("Payment.SignatureError", "Signature validation failed"));
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var webhookWrapper = JsonSerializer.Deserialize<PaystackChargeRequest>(request.requestBody, options);

            if(webhookWrapper == null)
            {
                return Result.Failure<bool>(new Error("Payment.MalformePayload", "The payload is malformed"));
            }
            
            var orderResult = await _orderRepository.GetSingleAsync(x => x.OrderNumber == webhookWrapper.Data.reference);
            if (orderResult == null)
            {
                _logger.LogWarning($"Order not found for {webhookWrapper.Data.reference}");
                return Result.Failure<bool>(new Error("Order.NotFound", "The order reference is not founds malformed"));
            }

            var orderStatus = webhookWrapper.Data.status.ToLower() == "success" ? OrderStatus.Completed : OrderStatus.Cancelled;
            
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

            _logger.LogWarning($"Payment successfully processed for {webhookWrapper.Data.reference} with Payment status: [{orderResult.ToString()}]");

            return true;
        }


        private bool IsSignatureValid(string rawBody, string sentSignature)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(_config.ApiSecret);
            byte[] messageBytes = Encoding.UTF8.GetBytes(rawBody);

            // Compute the expected hash bytes
            using var hmacsha512 = new HMACSHA512(keyByte);
            byte[] computedHashBytes = hmacsha512.ComputeHash(messageBytes);

            // Attempt to convert the computed hash directly to a lowercase hex string
            string computedSignature = Convert.ToHexString(computedHashBytes).ToLower();

            // Now convert both signature strings to byte arrays for comparison
            byte[] computedSigBytes = Encoding.UTF8.GetBytes(computedSignature);
            byte[] sentSigBytes = Encoding.UTF8.GetBytes(sentSignature);

            // Finally, securely compare bytes using a fixed execution time loop
            // Reason why I used 'FixedTimeEquals' is because it always checks every single byte in the array,
            // taking identical processing time regardless of where a mismatch occurs or if the tokens are perfectly valid.
            // In comparison with using 'Equals' which exit early on the first non-matching character which allows 
            // an attacker to deduce the correct signature character-by-character based on how long the server
            // takes to reject the request
            return CryptographicOperations.FixedTimeEquals(computedSigBytes, sentSigBytes);
        }

    }
}
