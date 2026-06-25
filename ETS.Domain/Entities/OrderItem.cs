using ETS.Domain.Common;
using ETS.Domain.Enums;
using ETS.Domain.Extensions;

namespace ETS.Domain.Entities
{
    public class OrderItem : Entity<Guid>
    {
        public Guid OrderId { get; private set; }
        public Guid EventCategoryId { get; private set; }
        public string Title { get; private set; }
        public int Unit { get; private set; }
        public decimal UnitPrice { get; private set; }
        public string? QRCodeReference { get; private set; }
        public string? QRCodeUrl { get; private set; }
        public TicketStatus RedemptionStatus { get; private set; }
        public TicketGenerationStatus TicketGenerationStatus { get; set; }
        public OrderStatus PaymentStatus { get; set; }
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
                Id = Guid.NewGuid(),
                PaymentStatus = OrderStatus.Pending,
                QRCodeReference = $"{Cryptography.CharGenerator.genID(12, CharacterSet.LOWER_ALPHABET_ONLY)}{DateTime.UtcNow:yyyyMMddHmmss}"
            };
        }

        public void UpdatePaymentStatus(OrderStatus paymentStatus)
        {
            PaymentStatus = paymentStatus;
        }
    }
}
