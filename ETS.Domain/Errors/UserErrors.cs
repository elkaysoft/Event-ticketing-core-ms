using ETS.Domain.Common;

namespace ETS.Domain.Errors
{
    public class UserErrors
    {
        public static readonly Error NotFound = new("User.NotFound", "The user was not found");
        public static readonly Error AlreadyExists = new("User.AlreadyExist", "User with this email already exists");
        public static readonly Error RegistrationFailed = new("User.RegistrationFailed", "An error occurred during admin user registration");
        public static readonly Error UpdateFailed = new("User.UpdateFailed", "An error occurred while updating the user");
    }
}
