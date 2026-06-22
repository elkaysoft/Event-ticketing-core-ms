namespace ETS.WebApi.DTO
{
    public class GetPagedTicketsFilter : RequestsPagination
    {
        public string? SearchText { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SortField { get; set; } = "CreatedAt";
    }

    public class ValidateCustomerTicketRequest
    {
        public Guid Id { get; set; }
        public int Qty { get; set; }
    }

}
