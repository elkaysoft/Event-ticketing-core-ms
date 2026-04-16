using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Enums;
using ETS.Domain.Errors;
using ETS.Domain.Extensions;
using ETS.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS.Application.Users.Commands.RegisterUser
{
    public record RegisterAdminUserCommand(string FullName, string EmailAddress, string PhoneNumber, RoleEnum Role) 
        : ICommand<UserDto>;
    
    public class RegisterAdminUserValidator : AbstractValidator<RegisterAdminUserCommand>
    {
        public RegisterAdminUserValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("FullName is required");

            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithMessage("Email address is required")
                .EmailAddress()
                .WithMessage("Invalid email address");

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Invalid role");
        }
    }

    public class RegisterAdminUserCommandHandler : ICommandHandler<RegisterAdminUserCommand, UserDto>
    {
        private readonly ILogger<RegisterAdminUserCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public readonly IUserRepository _userRepository;


        public async Task<Result<UserDto>> Handle(RegisterAdminUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _userRepository.GetSingleAsync(x => x.EmailAddress == request.EmailAddress,
                   cancellationToken);
                if (existingUser != null)
                {
                    return Result.Failure<UserDto>(UserErrors.AlreadyExists);
                }

                var temporaryPassword = Cryptography.CharGenerator.genID(10, CharacterSet.ALPHA_NUMERIC_NON_CASE);
                var user = User.Create(request.FullName,
                    request.EmailAddress,
                    request.PhoneNumber,
                    temporaryPassword,
                    request.Role);

                _userRepository.Add(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // send email here

                return new UserDto
                {
                    Email = request.EmailAddress,
                    PhoneNumber = request.PhoneNumber,
                    FullName = request.FullName,
                    Id = user.Id,
                    Role = request.Role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering admin user with email: {Email}", request.EmailAddress.SanitizeForLogging());
                return Result.Failure<UserDto>(UserErrors.RegistrationFailed);
            }

        }
    }


}
