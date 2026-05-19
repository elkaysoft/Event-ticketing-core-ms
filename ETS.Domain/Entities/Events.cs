using ETS.Domain.Common;

namespace ETS.Domain.Entities
{
    public class Events: Entity<Guid>
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Location { get; private set; } = string.Empty;
        public string BannerUrl { get; private set; } = string.Empty;
        public DateTime EventDate { get; private set; }
        public string KickoffTime { get; private set; } = string.Empty;
        public string EndTime { get; private set; } = string.Empty;
        public virtual IReadOnlyCollection<EventCategory> EventCategories { get; set; }

        public static Events Create(string title, 
            string description,
            string location,
            string bannerUrl,
            DateTime eventDate,
            string kickoffTime)
        {
            var events = new Events
            {
                Title = title,
                Description = description,
                Location = location,
                BannerUrl = bannerUrl,
                EventDate = eventDate,
                KickoffTime = kickoffTime
            };

            return events;
        }

        public void Update(string title,
            string description,
            string location,
            string bannerUrl,
            DateTime eventDate,
            string kickoffTime)
        {
            Title = title;
            Description = description;
            Location = location;
            BannerUrl = bannerUrl;
            EventDate = eventDate;
            KickoffTime = kickoffTime;
        }
    }
}
