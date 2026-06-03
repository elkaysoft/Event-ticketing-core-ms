using ETS.Domain.Common;
using ETS.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETS.Domain.Entities
{
    [Table("Users")]
    public class User: Entity<long>
    {
        public string FullName { get; private set; } = string.Empty;
        public string EmailAddress { get; private set; } = string.Empty;
        public string? PhoneNumber { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public RoleEnum Role { get; private set; }
        public int FailedLoginCount { get; private set; }
        public UserStatus Status { get; private set; }
        public DateTime? DeactivationDate { get; private set; }
        public DateTime? LastLoginDate { get; private set; }
        public DateTime? LastPasswordChangeDate { get; private set; }

        public static User Create(string fullName, string email, string phoneNumber, string password, RoleEnum role)
        {
            return new User() { FullName = fullName, EmailAddress = email, PhoneNumber = phoneNumber, Role = role, Password = password };
        }

        public void ChangePassword(string newPassword)
        {
            Password = newPassword;
        }

        public void ResetFailedLoginCount()
        {
            FailedLoginCount = 0;
            LastLoginDate = DateTime.UtcNow;
        }
        public void IncrementFailedLoginCount()
        {
            FailedLoginCount++;
        }

        public void UpdateUser(string emailAddress, string fullName, RoleEnum role)
        {
            EmailAddress = emailAddress;
            FullName = fullName;
            Role = role;
        }
         
        public void UpdateStatus(UserStatus newStatus)
        {
            Status = newStatus;            
        }

        public void SoftDelete()
        {
            IsDeleted = true;
        }

    }
}
