using ETS.Domain.Common;
using ETS.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETS.Domain.Entities
{
    [Table("Events")]
    public class Events: Entity<Guid>
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Location { get; private set; } = string.Empty;
        public string BannerUrl { get; private set; } = string.Empty;
        public DateTime EventDate { get; private set; }
        public string KickoffTime { get; private set; } = string.Empty;
        public string EndTime { get; private set; } = string.Empty;
        public PublishStatus PublishStatus { get; private set; } = PublishStatus.Draft;
        public virtual IReadOnlyCollection<EventCategory> EventCategories { get; set; }

        /// <summary>
        /// Create an event
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="location"></param>
        /// <param name="bannerUrl"></param>
        /// <param name="eventDate"></param>
        /// <param name="kickoffTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static Events Create(string title, 
            string description,
            string location,
            string bannerUrl,
            DateTime eventDate,
            string kickoffTime,
            string endTime)
        {
            var events = new Events
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                Location = location,
                BannerUrl = bannerUrl,
                EventDate = eventDate,
                KickoffTime = kickoffTime,
                EndTime = endTime,
                PublishStatus = PublishStatus.Draft
            };

            return events;
        }

        public void Update(string title,
            string description,
            string location,
            string bannerUrl,
            DateTime eventDate,
            string kickoffTime,
            string endTime,
            PublishStatus? publishStatus)
        {
            Title = title;
            Description = description;
            Location = location;
            BannerUrl = bannerUrl;
            if (publishStatus.HasValue)
            {
                PublishStatus = publishStatus.Value;
            }
            EventDate = eventDate;
            KickoffTime = kickoffTime;
            EndTime = endTime;
        }
    }
}
