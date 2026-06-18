using ETS.Domain.Common;
using ETS.Domain.Enums;

namespace ETS.Domain.Entities
{
    public class Order: Entity<Guid>
    {
        public Guid EventId { get; set; }
        public string EventName { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string OrderNumber { get; set; }
        public string PaystackAccessCode { get; set; }
        public OrderStatus OrderStatus  { get; set; }
        public DateTime? PaymentConfirmedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancelletionReason { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalTickets { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual Events Event { get; set; }

        public static Order Create(string eventName,
            Guid eventId,
            string fullName,
            string emailAddress, 
            string phoneNumber,
            string orderNumber,
            decimal taxAmount,
            decimal subTotal,
            decimal totalAmount,
            string paystackAccessCode,
            int totalTickets)
        {
            return new Order
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                EmailAddress = emailAddress,
                PhoneNumber = phoneNumber,
                OrderNumber = orderNumber,
                OrderStatus = OrderStatus.Pending,
                TaxAmount = taxAmount,
                SubTotal = subTotal,
                TotalAmount = totalAmount,
                PaystackAccessCode = paystackAccessCode,
                TotalTickets = totalTickets,
                EventId = eventId,
                EventName = eventName
            };
        }

        public void UpdateStatus(OrderStatus orderStatus)
        {
            OrderStatus = orderStatus;
            PaymentConfirmedAt = DateTime.UtcNow;
            CompletedAt = DateTime.UtcNow;
        }

    }
}
