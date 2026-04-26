using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ETS.Application.Authentication.Commands.ChangePassword
{
    public record ChangePasswordCommand(string EmailAddress,
        string CurrentPassword, 
        string NewPassword,
        string ConfirmNewPassword) : ICommand<ChangePasswordResponse>;
    
    public class ChangePasswordCommandValidator: AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email is required");

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is requred");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(8).WithMessage("New password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("New password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("New password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("New password must contain at least one digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("New password must contain at least one special character.");
            RuleFor(x => x.ConfirmNewPassword)
                .Equal(x => x.NewPassword).WithMessage("Confirm password must match the new password");
        }
    }

    public class ChangePasswordCommandHandler(IUserRepository _userRepository,
        IUnitOfWork _unitOfWork,
        ILogger<ChangePasswordCommandHandler> _logger) 
        : ICommandHandler<ChangePasswordCommand, ChangePasswordResponse>
    {

        public async Task<Result<ChangePasswordResponse>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetSingleAsync(x => x.EmailAddress == request.EmailAddress,
                    cancellationToken);
                if(user is null)
                {
                    return Result.Failure<ChangePasswordResponse>(UserErrors.NotFound);
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.ConfirmNewPassword);
                user.ChangePassword(hashedPassword);

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new ChangePasswordResponse
                {
                    EmailAddress = request.EmailAddress,
                    Message = "Password successfully changed"
                };
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "An error occured while processing the request");
                return Result.Failure<ChangePasswordResponse>(new Error("ChangePassword.Failed", "An error occured while trying to perform request, pls try again."));
            }
        }
    }

}
