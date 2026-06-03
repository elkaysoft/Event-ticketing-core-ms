namespace ETS.Application.Events.Commands.Add
{
    public class AddEventsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string BannerUrl { get; set; }
    }
}
