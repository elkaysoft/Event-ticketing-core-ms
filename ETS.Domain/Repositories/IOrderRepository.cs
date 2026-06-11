using ETS.Domain.Entities;

namespace ETS.Domain.Repositories
{
    public interface IOrderRepository : IRepository<Order, Guid>
    {
    }
}
