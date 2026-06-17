using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Repositories;

namespace ETS.Infrastructure.Repositories
{
    public class OrderItemRepository : Repository<OrderItem, Guid>, IOrderItemRepository
    {
        public OrderItemRepository(IWriteApplicationDbContext writeDbContext,
            IReadApplicationDbContext readDbContext) : base(writeDbContext, readDbContext)
        {
        }

        public async Task<List<OrderItem>> GetOrderItemsByIds(List<Guid> ids, CancellationToken cancellationToken)
        {
            return await GetAllAsync(ec => ids.Contains(ec.Id), cancellationToken);
        }
    }
}
