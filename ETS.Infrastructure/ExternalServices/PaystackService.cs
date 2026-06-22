using ETS.Domain.AppConfig;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Errors;
using ETS.Domain.Models.Paystack.Request;
using ETS.Domain.Models.Paystack.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ETS.Infrastructure.ExternalServices
{
    public class PaystackService : ExternalServiceBase, IPaystackService
    {
        private readonly ILogger<PaystackService> _logger;
        private string secretKey = "";
        private readonly PaystackConfigOptions _paystackConfigOption;

        public PaystackService(IServiceProvider serviceProvider,
            ILogger<PaystackService> logger,
            IOptions<PaystackConfigOptions> pstConfigOption) : base(serviceProvider, logger)
        {
            BaseUriString = pstConfigOption.Value.BaseUrl;
            _logger = logger;            
            _paystackConfigOption = pstConfigOption.Value;

            secretKey = _paystackConfigOption.ApiSecret;
        }

        public async Task<Result<InitializeTransactionResponse>> InitializeTransactionAsync(InitiateTransactionRequest model, 
            CancellationToken cancellationToken = default)
        {            
            try
            {
                model.CallbackUrl = _paystackConfigOption.SuccessfulPaymentUrl;
                var metadata = new { cancel_action = _paystackConfigOption.FailedPaymentUrl };
                model.MetaData = JsonSerializer.Serialize(metadata);

                var headers = BuildHeaders();
                var response = await Send<PaystackBase<InitializeTransactionResponse>>(HttpMethod.Post,
                    "/transaction/initialize",
                    model,
                    headers,
                    cancellation: cancellationToken);
                                
                return response.Status ?
                    Result.Success(response.Data) :
                    Result.Failure<InitializeTransactionResponse>(new Error("TransactionInitialization.Failed", 
                    response.Message ?? "Transaction initialization failed!"));
            }
            catch (Exception ex)
            {
                return Result.Failure<InitializeTransactionResponse>(TransactionErrors.TransactionProcessingFailed);
            }
        }

        private Dictionary<string, string> BuildHeaders() =>
            new()
            {
                ["Authorization"] = $"Bearer {secretKey}"
            };


       

    }
}
