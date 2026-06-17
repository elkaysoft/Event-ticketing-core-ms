using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Repositories;

namespace ETS.Infrastructure.Repositories
{
    public class EventCategoryRepository : Repository<EventCategory, Guid>, IEventCategoryRepository
    {
        public EventCategoryRepository(IWriteApplicationDbContext writeDbContext,
            IReadApplicationDbContext readDbContext)
            : base(writeDbContext, readDbContext)
        {
        }

        public async Task<List<EventCategory>> GetEventCategoriesByIds(List<Guid> ids, CancellationToken cancellationToken)
        {
            return await GetAllAsync(ec => ids.Contains(ec.Id), cancellationToken);            
        }
    }
}
