using ETS.Application.Users.Commands.RegisterUser;
using ETS.Domain.AppConfig;
using ETS.Domain.Errors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ETS.Application.Abstraction.Common
{
    public static class DbInitializer
    {
        public static void Seed(IServiceScope scope)
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(nameof(DbInitializer));
            var userSetupOptions = scope.ServiceProvider.GetRequiredService<IOptions<DefaultAdminUserSetupOptions>>().Value;

            var useCommand = new RegisterDefaultUserCommand(userSetupOptions.FullName,
                userSetupOptions.Email,
                userSetupOptions.PhoneNumber,
                userSetupOptions.Password,
                Domain.Enums.RoleEnum.Superadmin);
            var result = sender.Send(useCommand).Result;

            if (!result.IsSuccess)
            {
                if(result.IsFailure && result.Error == UserErrors.AlreadyExists)
                {
                    logger.LogWarning("The default admin user has already been created");
                    return;
                }
                else
                {
                    throw new Exception($"An unexpected error occurred: {result.Error.Message}");
                }
            }
        }
    }
}
