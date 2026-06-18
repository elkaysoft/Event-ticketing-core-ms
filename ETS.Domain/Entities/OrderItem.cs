using ETS.Domain.Common;

namespace ETS.Domain.Entities
{
    public class OrderItem : Entity<Guid>
    {
        public Guid OrderId { get; set; }
        public Guid EventCategoryId { get; set; }
        public string Title { get; set; }
        public int Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public virtual EventCategory EventCategory { get; set; }
        public virtual Order Order { get; set; }

        public static OrderItem Create(string titel, Guid orderId, Guid eventCategoryId, int unit, decimal unitPrice)
        {
            return new OrderItem
            {
                Title = titel,
                OrderId = orderId,
                EventCategoryId = eventCategoryId,
                Unit = unit,
                UnitPrice = unitPrice,
                Id = Guid.NewGuid()
            };
        }
    }
}
