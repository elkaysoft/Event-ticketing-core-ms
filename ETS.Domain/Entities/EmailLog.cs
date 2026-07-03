using ETS.Domain.Common;
using ETS.Domain.Enums;

namespace ETS.Domain.Entities
{
    public class EmailLog : Entity<long>
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public NotificationStatusEnum NotificationStatus { get; set; }
        public NotificationTargetEnum NotificationTarget { get; set; }       
        public string? ResponseData { get; set; }
        public int RetryCount { get; set; }
    }
}
