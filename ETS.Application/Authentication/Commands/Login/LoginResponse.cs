using ETS.Domain.Models;

namespace ETS.Application.Authentication.Commands.Login
{
    public class LoginResponse
    {
        public TokenResponse Token { get; set; } = null!;
        public TokenUser User { get; set; } = null!;
    }    
}
