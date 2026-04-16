using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Enums;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ETS.Application.Users.Commands.RegisterUser
{
    public record RegisterDefaultUserCommand(string FullName, string EmailAddress, string PhoneNumber,
        string Password, RoleEnum Role)
        :ICommand<UserDto>
    {
    }

    public class RegisterUserCommandValidator : AbstractValidator<RegisterDefaultUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("A valid email address is required");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("Phone number must not exceed 20 characters");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Fullname is required and must not exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
               .MinimumLength(8)
               .WithMessage("Password must be at least 8 characters");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid role value");                

        }
    }

    public class RegisterUserCommandHandler : ICommandHandler<RegisterDefaultUserCommand, UserDto>
    {
        private readonly ILogger<RegisterUserCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public readonly IUserRepository _userRepository;

        public RegisterUserCommandHandler(ILogger<RegisterUserCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IUserRepository userRepository)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }


        public async Task<Result<UserDto>> Handle(RegisterDefaultUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _userRepository.GetSingleAsync(x => x.EmailAddress == request.EmailAddress, 
                    cancellationToken);
                if(existingUser != null)
                {
                    return Result.Failure<UserDto>(UserErrors.AlreadyExists);
                }

                var user = User.Create(request.FullName,
                    request.EmailAddress,
                    request.PhoneNumber,
                    request.Password,
                    request.Role);

                _userRepository.Add(user); 
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new UserDto
                {
                    Email = request.EmailAddress,
                    PhoneNumber = request.PhoneNumber,
                    FullName = request.FullName,
                    Id = user.Id,
                    Role = user.Role
                };

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error registering admin user with email: {Email}", request.EmailAddress.SanitizeForLogging());
                return Result.Failure<UserDto>(UserErrors.RegistrationFailed);
            }
        }


    }
}
