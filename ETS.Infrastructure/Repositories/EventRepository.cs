using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Repositories;

namespace ETS.Infrastructure.Repositories
{
    public class EventRepository : Repository<Events, Guid>, IEventRepository
    {
        public EventRepository(IWriteApplicationDbContext writeDbContext, 
            IReadApplicationDbContext readDbContext) 
            : base(writeDbContext, readDbContext)
        {
        }

        public async Task<bool> IsEventOverlapping(string location, 
            DateTime eventDate, 
            string kickOffTime, 
            CancellationToken cancellationToken)
        {
            return await ExistsAsync(x => x.Location == location && x.EventDate == eventDate && x.KickoffTime == kickOffTime, cancellationToken);
        }

        public async Task<bool> IsUpdatedEventOverlapping(Guid eventId, string location,
            DateTime eventDate,
            string kickOffTime,
            CancellationToken cancellationToken)
        {
            return await ExistsAsync(x => x.Id != eventId && x.Location == location && 
                    x.EventDate == eventDate 
                    && x.KickoffTime == kickOffTime, cancellationToken);
        }

    }
}
