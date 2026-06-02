namespace ETS.WebApi.DTO
{
    public class EventRequest
    {
        public required IFormFile Thumbnail { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Location { get; set; }
        public DateTime EventDate { get; set; }
        public string StartTime { get; set; }
        public required List<EventCategoryRequest> EventCategories { get; set; }
    }

    public class EventCategoryRequest
    {
        public string Title { get; set; }
        public int Qty { get; set; }
        public int Price { get; set; }
    }
}
