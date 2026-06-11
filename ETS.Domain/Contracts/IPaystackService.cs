using ETS.Domain.Common;
using ETS.Domain.Models.Paystack.Request;
using ETS.Domain.Models.Paystack.Response;

namespace ETS.Domain.Contracts
{
    public interface IPaystackService
    {
        Task<Result<InitializeTransactionResponse>> InitializeTransactionAsync(InitiateTransactionRequest model,
            CancellationToken cancellationToken);
    }
}
