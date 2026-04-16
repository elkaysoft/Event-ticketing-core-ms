using ETS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ETS.Infrastructure.Authentication
{
    internal static class ClaimsPrincipalExtensions
    {
        public static Guid? GetUserId(this ClaimsPrincipal? principal)
        {
            var userId = principal?.FindFirstValue("user_id");

            var parseUserId = Guid.TryParse(userId, out var uId);
            return parseUserId ? uId : null;
        }

        public static string? GetUserEmail(this ClaimsPrincipal? principal)
        {
            var userEmail = principal?.FindFirstValue(ClaimTypes.Email);
            return userEmail;
        }

        public static string? GetUserFullName(this ClaimsPrincipal? principal)
        {
            var fullName = principal?.FindFirstValue("fullname");
            return fullName;
        }

        public static string? GetUserPhone(this ClaimsPrincipal? principal)
        {
            var userPhone = principal?.FindFirstValue("phoneNumber");
            return userPhone;
        }

        public static RoleEnum? GetUserRole(this ClaimsPrincipal? principal)
        {
            var userRole = principal?.FindFirstValue("user_role");

            var parsedUserRole = Enum.TryParse<RoleEnum>(userRole, out var role);
            return parsedUserRole ? role : null;
        }

    }
}
