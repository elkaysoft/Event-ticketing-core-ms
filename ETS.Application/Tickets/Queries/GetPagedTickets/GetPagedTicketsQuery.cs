using ETS.Application.Abstraction.Common;
using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ETS.Application.Tickets.Queries.GetPagedTickets
{
    public record GetPagedTicketsQuery(string? SearchText, 
        DateTime? StartDate,
        DateTime? EndDate,
        string? SortField) : PaginationQuery, IQuery<PaginatedList<CustomerTicketsDto>>;

    public class GetPagedTicketsQueryHandler : IQueryHandler<GetPagedTicketsQuery, PaginatedList<CustomerTicketsDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetPagedTicketsQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        private static Expression<Func<Domain.Entities.Order, bool>> GetQueryExpression(GetPagedTicketsQuery request) => u =>
            (request.StartDate == null || u.CreatedAt >= request.StartDate.Value) &&
            (request.EndDate == null || u.CreatedAt <= request.EndDate.Value.AddDays(1).AddMinutes(-1)) &&
            (string.IsNullOrWhiteSpace(request.SearchText) ||
                EF.Functions.Like(u.EventName, $"%{request.SearchText}%") ||
                EF.Functions.Like(u.FullName, $"%{request.SearchText}%") ||
                EF.Functions.Like(u.EmailAddress, $"%{request.SearchText}%") ||
                EF.Functions.Like(u.OrderNumber, $"%{request.SearchText}%"));

        private Expression<Func<Domain.Entities.Order, CustomerTicketsDto>> Selector()
        {
            return v => new CustomerTicketsDto
            {
                Id = v.Id,
                Status = v.OrderStatus.ToString(),
                EmailAddress = v.EmailAddress,
                FullName = v.FullName,
                Qty = v.TotalTickets,
                TicketNumber = v.OrderNumber,
                Title = v.EventName
            };
        }

        private static Expression<Func<Domain.Entities.Order, object>> GetSortProperty(GetPagedTicketsQuery request) =>
           request.SortField?.ToLower() switch
           {
               "id" => p => p.Id,
               _ => p => p.CreatedAt
           };


        public async Task<Result<PaginatedList<CustomerTicketsDto>>> Handle(GetPagedTicketsQuery request, CancellationToken cancellationToken)
        {
            var filter = GetQueryExpression(request);
            var sort = GetSortProperty(request);

            var tickets = await _orderRepository.GetPaginatedAsync(
              filter,
              Selector(),
              request.PageNumber,
              request.PageSize,
              sort,
              false,
              cancellationToken: cancellationToken);

            return tickets;
        }
    }

}
