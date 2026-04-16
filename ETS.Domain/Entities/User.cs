using ETS.Domain.Common;
using ETS.Domain.Enums;

namespace ETS.Domain.Entities
{
    public class User: Entity<long>
    {
        public string FullName { get; private set; } = string.Empty;
        public string EmailAddress { get; private set; } = string.Empty;
        public string? PhoneNumber { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public RoleEnum Role { get; private set; }

        public static User Create(string fullName, string email, string phoneNumber, string password, RoleEnum role)
        {
            return new User() { FullName = fullName, EmailAddress = email, PhoneNumber = phoneNumber, Role = role, Password = password };
        }

        public void ChangePassword(string newPassword)
        {
            Password = newPassword;
        }

    }
}
