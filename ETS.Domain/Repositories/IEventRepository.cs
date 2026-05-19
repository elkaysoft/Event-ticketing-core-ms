using ETS.Domain.Entities;

namespace ETS.Domain.Repositories
{
    public interface IEventRepository : IRepository<Events, Guid>
    {
    }
}
