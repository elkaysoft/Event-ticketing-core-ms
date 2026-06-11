namespace ETS.Application.Events.Queries.GetActiveEvent
{
    public class GetActiveEventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Location { get; set; }
        public string BannerUrl { get; set; }
        public List<TicketCategoryDto> TicketCategories { get; set; } = [];
    }

    public class TicketCategoryDto
    {
        public Guid TicketId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
    }
}
