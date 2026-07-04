using ETS.Domain.Enums;

namespace ETS.Application.Tickets.Queries.VerifyTicket
{
    public class VerifyTicketResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? TicketHolder { get; set; }
        public string? EventTitle { get; set; }
        public string? TicketCategory { get; set; }
        public DateTime? EventDate { get; set; }
        public TicketStatus RedemptionStatus { get; set; }
    }
}
