using ETS.Domain.Common;

namespace ETS.Domain.Errors
{
    public class TransactionErrors
    {
        public static readonly Error TransactionProcessingFailed = new("Transaction.Failed", "We are unable to process transaction transaction, pls try again later.");
        public static readonly Error UnableToProcessPayment = new("Transaction.PaymentProcessFailed", "We are unable to process your payment at the moment");
    }
}
