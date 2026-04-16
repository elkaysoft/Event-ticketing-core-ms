using ETS.Domain.Enums;

namespace ETS.Application.Users.Commands.RegisterUser
{
    public class UserDto
    {
        public long Id { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string FullName { get; set; }
        public RoleEnum Role { get; set; }        
    }
}
