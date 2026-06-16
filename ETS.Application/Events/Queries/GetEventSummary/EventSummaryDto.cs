namespace ETS.Application.Events.Queries.GetEventSummary
{
    public class EventSummaryDto
    {
        public int PublishedCount { get; set; }
        public int DraftedCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
    }
}
