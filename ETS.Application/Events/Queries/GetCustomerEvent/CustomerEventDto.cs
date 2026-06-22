namespace ETS.Application.Events.Queries.GetCustomerEvent
{
    public class CustomerEventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string BannerUrl { get; set; }
        public DateTime EventDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public List<CustomerEventItemsDto> Items { get; set; }
    }
    public class CustomerEventItemsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int AvailableUnit { get; set; }
    }
}
