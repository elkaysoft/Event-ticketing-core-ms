using ETS.Domain.Enums;

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
        public string EndTime { get; set; }
        public required PublishStatus PublishStatus { get; set; }
        public required List<EventCategoryRequest> EventCategories { get; set; }
    }

    public class EventCategoryRequest
    {
        public string Title { get; set; }
        public int Qty { get; set; }
        public int Price { get; set; }
    }

    public class GetPaginatedEventFilter: RequestsPagination
    {
        public bool IsAscending { get; set; }
        public string? SortField { get; set; }
        public string? SearchText { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PublishStatus? PublishStatus { get; set; }
    }

    public class UpdateEventRequest
    {
        public IFormFile? Thumbnail { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Location { get; set; }
        public DateTime EventDate { get; set; }
        public required string StartTime { get; set; }
        public string EndTime { get; set; }
        public PublishStatus PublishStatus { get; set; }
    }

    public class UpdateEventCategoryRequest
    {
        public required string Title { get; set; }
        public required int Qty { get; set; }
        public required decimal Price { get; set; }
    }


    public class CreateEventCategoryRequest
    {
        public required string Title { get; set; }
        public required int Qty { get; set; }
        public required decimal Price { get; set; }
    }

}
