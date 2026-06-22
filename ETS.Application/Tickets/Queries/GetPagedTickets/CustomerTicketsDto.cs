using ETS.Domain.Enums;

namespace ETS.Application.Tickets.Queries.GetPagedTickets
{
    public class CustomerTicketsDto
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; }
        public string Title { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public int Qty { get; set; }
        public OrderStatus PaymentStatus { get; set; }
        public TicketStatus RedemptionStatus  { get; set; }
    }
}
