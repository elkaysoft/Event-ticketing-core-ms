using ETS.Domain.Common;

namespace ETS.Domain.Errors
{
    public class EventErrors
    {
        public static readonly Error NotFound = new("Event.NotFound", "The event was not found");
        public static readonly Error AlreadyExists = new("Event.AlreadyExist", "Event with this title already exists");
        public static readonly Error EventCreationFailed = new("Event.Failed", "Failed to create an event");
        public static readonly Error OverlappingEventError = new("Event.OverlappingError", "There is already a running event set for the selected date");
        public static readonly Error NoActiveEventFound = new("Event.NoActiveEvent", "No active event found");
    }
}
