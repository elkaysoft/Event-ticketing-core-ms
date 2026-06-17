using ETS.Domain.Entities;

namespace ETS.Domain.Repositories
{
    public interface IOrderItemRepository : IRepository<OrderItem, Guid>
    {
        Task<List<OrderItem>> GetOrderItemsByIds(List<Guid> ids, CancellationToken cancellationToken);
    }
}
