using ETS.Application.Abstraction.Common;
using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;

namespace ETS.Application.Tickets.Queries.GetPagedTickets
{
    public record GetPagedTicketsQuery(string? searchText, 
        DateTime? StartDate,
        DateTime? EndDate) : PaginationQuery, IQuery<PaginatedList<CustomerTicketsDto>>;

    public class GetPagedTicketsQueryHandler
    {

    }
    
}
