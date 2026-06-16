using ETS.Domain.Entities;

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


    public static class EventMappingExtensions
    {
        public static GetActiveEventDto ToDto(this Domain.Entities.Events activeEvent) => new()
        {
            Id = activeEvent.Id,
            Title = activeEvent.Title,
            Description = activeEvent.Description,
            Location = activeEvent.Location,
            BannerUrl = activeEvent.BannerUrl,
            EventDate = activeEvent.EventDate,
            StartTime = activeEvent.KickoffTime,
            EndTime = activeEvent.EndTime,
            TicketCategories = activeEvent.EventCategories
            .Select(ec => new TicketCategoryDto
            {
                TicketId = ec.Id,
                Title = ec.Title,
                Price = ec.Price,
            }).ToList()
        };
    }
    
}
