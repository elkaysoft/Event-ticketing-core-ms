using ETS.Domain.Enums;

namespace ETS.Application.Tickets.Queries.GetSingleTicket
{
    public class GetSingleTicketDto
    {
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime DateRegistered { get; set; }
        public DateTime EventDate { get; set; }
        public string EventName { get; set; }
        public OrderStatus PaymentStatus { get; set; }
        public TicketStatus RedemptionStatus { get; set; }
        public List<GetSingleTicketDetails> Details { get; set; }
    }

    public class GetSingleTicketDetails
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public int Qty { get; set; }
        public OrderStatus Status { get; set; }
    }
}
