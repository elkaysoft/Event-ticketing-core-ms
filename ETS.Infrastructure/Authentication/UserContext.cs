using ETS.Domain.Contracts;
using Microsoft.AspNetCore.Http;

namespace ETS.Infrastructure.Authentication
{
    internal sealed class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId =>
            _httpContextAccessor
            .HttpContext?
            .User
            .GetUserId();

        public string? UserEmail =>
            _httpContextAccessor
            .HttpContext?
            .User
            .GetUserEmail();

        public string? UserPhone => 
            _httpContextAccessor
            ?.HttpContext? 
            .User
            .GetUserPhone();

        public string? UserName =>
            _httpContextAccessor
            ? .HttpContext?
            .User
            .GetUserFullName();

    }
}
