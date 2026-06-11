using ETS.Domain.Common;

namespace ETS.Domain.Entities
{
    public class OrderItem : Entity<Guid>
    {
        public Guid OrderId { get; set; }
        public Guid EventCategoryId { get; set; }
        public int Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public virtual EventCategory EventCategory { get; set; }
        public virtual Order Order { get; set; }

        public static OrderItem Create(Guid orderId, Guid eventCategoryId, int unit, decimal unitPrice)
        {
            return new OrderItem
            {
                OrderId = orderId,
                EventCategoryId = eventCategoryId,
                Unit = unit,
                UnitPrice = unitPrice,
                Id = Guid.NewGuid()
            };
        }
    }
}
