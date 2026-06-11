namespace ETS.Domain.Models.Paystack.Response
{
    public class PaystackBase<T> where T : class
    {
        
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class InitializeTransactionResponse
    {
        public string authorization_url { get; set; }
        public string access_code { get; set; }
        public string reference { get; set; }
    }
}
