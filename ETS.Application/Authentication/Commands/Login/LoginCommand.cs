using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Enums;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS.Application.Authentication.Commands.Login
{
    public record LoginCommand(string Username, string Password, PlatformEnum Platform) : ICommand<LoginResponse>;

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
        }
    }

    public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            ILogger<LoginCommandHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetSingleAsync(x => x.EmailAddress == request.Username, cancellationToken); 
                if(user is null)
                    return Result.Failure<LoginResponse>(LoginErrors.NotFound);

                if(user.Status != UserStatus.Active)
                {
                    return Result.Failure<LoginResponse>(new Error("UserNotActive", $"This user is currently {user.Status}"));
                }


                if(!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    user.IncrementFailedLoginCount();
                    int passwordTrialLeft = 5 - user.FailedLoginCount;
                    string attemptMsg = passwordTrialLeft <= 1 ? "attempt" : "attempts";
                    if (user.FailedLoginCount >= 5)
                    {
                        user.UpdateStatus(UserStatus.Locked);
                    }
                    _userRepository.Update(user);
                    _unitOfWork.SaveChanges();

                    var failureError = LoginErrors.InvalidCredentials with
                    {
                        Message = $"Invalid credentials. You have {passwordTrialLeft} {attemptMsg} left before your account gets locked."
                    };
                    return Result.Failure<LoginResponse>(failureError);
                }

                user.ResetFailedLoginCount();
                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var jwtToken = await _tokenService.GenerateAccessTokenAsync(user.Id, cancellationToken);

                return new LoginResponse
                {
                    Token = jwtToken.Value,
                    User = new Domain.Models.TokenUser
                    {
                        UserId = user.Id,
                        Email = user.EmailAddress,
                        PhoneNumber = user.PhoneNumber,
                        FullName = user.FullName,
                        Role = user.Role.ToString()
                    }
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the login command for username: {Username}", request.Username);
                return Result.Failure<LoginResponse>(LoginErrors.LoginFailed);
            }
        }
    }


}
