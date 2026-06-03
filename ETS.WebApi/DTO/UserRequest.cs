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

    public class GetPaginatedUserFilter : RequestsPagination
    {
        public bool IsAscending { get; set; }
        public string? SortField { get; set; }
        public RoleEnum? Role { get; set; }
        public string? SearchText { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateUserRequest
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string EmailAddress { get; set; } = string.Empty;
        [Required]
        public RoleEnum Role { get; set; }
    }
}
