using ETS.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ETS.WebApi.DTO
{
    public class AddUserRequest
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string EmailAddress { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        [Required]
        public RoleEnum Role { get; set; }
    }
}
