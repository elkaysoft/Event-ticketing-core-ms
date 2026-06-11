using ETS.Domain.Entities;

namespace ETS.Domain.Repositories
{
    public interface IEventRepository : IRepository<Events, Guid>
    {
        Task<bool> IsEventOverlapping(string location,
            DateTime eventDate,
            string kickOffTime,
            CancellationToken cancellationToken);

        Task<bool> IsUpdatedEventOverlapping(Guid eventId, string location,
            DateTime eventDate,
            string kickOffTime,
            CancellationToken cancellationToken);

        Task<List<Events>> GetActiveEvents();

    }
}
