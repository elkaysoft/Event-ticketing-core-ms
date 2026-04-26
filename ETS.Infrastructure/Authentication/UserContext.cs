using ETS.Domain.Contracts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ETS.Infrastructure.Authentication
{
    internal sealed class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId =>
            Principal
            .GetUserId();

        public string? UserEmail =>
            Principal
            .GetUserEmail();

        public string? UserPhone => 
            Principal
            .GetUserPhone();

        public string? UserName =>
            Principal
            .GetUserFullName();

    }
}
