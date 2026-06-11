using ETS.Domain.Entities;

namespace ETS.Domain.Repositories
{
    public interface IEventCategoryRepository: IRepository<EventCategory, Guid>
    {
        Task<List<EventCategory>> GetEventCategoriesByIds(List<Guid> ids, CancellationToken cancellationToken);
    }
}
