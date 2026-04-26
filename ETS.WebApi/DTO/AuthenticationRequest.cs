using ETS.Domain.Enums;

namespace ETS.WebApi.DTO
{
    public class LoginRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public PlatformEnum Platform { get; set; }
    }

    public class ChangePasswordRequestDto 
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
