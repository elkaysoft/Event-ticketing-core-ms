using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETS.Infrastructure.Repositories
{
    public class EventCategoryRepository : Repository<EventCategory, Guid>, IEventCategoryRepository
    {
        public EventCategoryRepository(IWriteApplicationDbContext writeDbContext,
            IReadApplicationDbContext readDbContext)
            : base(writeDbContext, readDbContext)
        {
        }
    }
}
