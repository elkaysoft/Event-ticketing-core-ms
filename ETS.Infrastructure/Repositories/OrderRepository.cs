using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Repositories;

namespace ETS.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order, Guid>, IOrderRepository
    {
        public OrderRepository(IWriteApplicationDbContext writeDbContext, 
            IReadApplicationDbContext readDbContext) : base(writeDbContext, readDbContext)
        {
        }
    }
}
