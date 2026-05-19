using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Repositories;

namespace ETS.Infrastructure.Repositories
{
    public class EventRepository : Repository<Events, Guid>, IEventRepository
    {
        public EventRepository(IDbContext writeDbContext, 
            IDbContext readDbContext) 
            : base(writeDbContext, readDbContext)
        {
        }
    }
}
