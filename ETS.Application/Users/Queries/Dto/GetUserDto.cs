namespace ETS.Application.Users.Queries.Dto
{
    public class GetUserDto
    {
        public long Id { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
