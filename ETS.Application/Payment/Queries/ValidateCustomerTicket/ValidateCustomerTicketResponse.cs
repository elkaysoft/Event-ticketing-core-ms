namespace ETS.Application.Payment.Queries.ValidateCustomerTicket
{
    public class ValidateCustomerTicketResponse
    {
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public List<ValidateCustomerTicketResponseDetails> details { get; set; } = [];
        public ChargeDetails Charge { get; set; }
    }

    public class ChargeDetails
    {
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public decimal Tax { get; set; }
    }

    public class ValidateCustomerTicketResponseDetails
    {
        public Guid Id { get; set; }
        public int RequestedQty { get; set; }
        public int AvailableQty { get; set; }
        public bool HasError { get; set; }
        //public string ErrorMessage { get; set; } = string.Empty;
    }

}
