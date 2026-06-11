namespace ETS.WebApi.DTO
{
    public class EventCheckoutRequest
    {
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public List<TicketDetail> TicketDetails { get; set; } = new List<TicketDetail>();

    }

    public class TicketDetail
    {
        public Guid TicketId { get; set; }
        public int Unit { get; set; }
    }
}
