using ETS.Domain.Enums;

namespace ETS.Application.Events.Queries.Events
{
    public class EventItemsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string BannerUrl { get; set; }
        public DateTime EventDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int TotalSold { get; set; }
        public int TotalTickets { get; set; }
        public DateTime DateCreated { get; set; }
        public PublishStatus PublishStatus { get; set; }
        public List<EventCategoryItemsDto> EventCategories { get; set; } = [];
        public EventTicketStatistics Summary { get; set; }
    }

    public class EventTicketStatistics
    {
        public int TotalTickets { get; set; }
        public int TotalSold { get; set; }
        public int TicketsAvailable { get; set; }
        public decimal AmountSold { get; set; }
    }

    public class EventCategoryItemsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
    }


}
