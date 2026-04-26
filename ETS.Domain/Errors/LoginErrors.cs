using ETS.Domain.Common;

namespace ETS.Domain.Errors
{
    public class LoginErrors
    {
        public static readonly Error NotFound = new("User.NotFound", "The user was not found");
        public static readonly Error InvalidCredentials = new("User.InvalidCredentials", "Invalid password");
        public static readonly Error LoginFailed = new("Login.Failed", "Login failed due to an unexpected error");
        
    }
}
