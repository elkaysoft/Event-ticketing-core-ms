using ETS.Domain.Entities;

namespace ETS.Domain.Repositories
{
    public interface IOrderItemRepository : IRepository<OrderItem, Guid>
    {
    }
}
