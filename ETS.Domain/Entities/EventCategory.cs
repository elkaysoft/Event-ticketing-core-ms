using ETS.Domain.Common;

namespace ETS.Domain.Entities
{
    public class EventCategory: Entity<Guid>
    {
        public Guid EventId { get; private set; }
        public string Title { get; private set; }
        public int Qty { get; private set; }
        public int UnitSold { get; private set; } = 0;
        public decimal Price { get; private set; }
        public virtual Events Event { get; set; }

        public static EventCategory Create(Guid eventId, string title, int qty, decimal price)
        {
            return new EventCategory { EventId =  eventId, Title = title, Qty = qty, Price = price };
        }

        public void Update(string title, int qty, decimal price)
        {
            Title = title;
            Qty = qty;  
            Price = price;
        }

        public void UpdateQuantity(int unitSold)
        {
            

        }


        public void AddToUnitSold(int qty)
        {
            UnitSold += qty;
        }

        public void RemoveFromUnitSold(int qty)
        {
            UnitSold -= qty;
        }

    }
}
