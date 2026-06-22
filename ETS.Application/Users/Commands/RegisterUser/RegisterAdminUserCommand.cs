using ETS.Application.Abstraction.Mediation;
using ETS.Domain.AppConfig;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Enums;
using ETS.Domain.Errors;
using ETS.Domain.Extensions;
using ETS.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly AppSettingsConfigOption _appSettingsConfigOption;
        private readonly IUserContext _userContext;

        public RegisterAdminUserCommandHandler(ILogger<RegisterAdminUserCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            IEmailService emailService,
            IOptions<AppSettingsConfigOption> appSettingsConfigOption,
            IUserContext userContext)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _emailService = emailService;
            _appSettingsConfigOption = appSettingsConfigOption.Value;
            _userContext = userContext;
        }


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

                string verificationKey = $"{Cryptography.CharGenerator.genID(12, CharacterSet.ALPHA_NUMERIC_CASE)}|{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
                
                var temporaryPassword = Cryptography.CharGenerator.genID(10, CharacterSet.ALPHA_NUMERIC_NON_CASE);
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(temporaryPassword);
                var user = User.Create(request.FullName,
                    request.EmailAddress,
                    request.PhoneNumber,
                    hashedPassword,
                    request.Role);
                _userRepository.Add(user);

                long.TryParse(_userContext.UserId, out var userId);
                var verificationToken = VerificationToken.Create(userId, verificationKey, DateTime.UtcNow.AddDays(2),
                    VerificationPurpose.AccountActivation);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // send email here
                var emailBody = GetActivationLink(request.FullName, verificationKey);
                await _emailService.SendEmail(new Domain.Models.Postmark.Requests.SendEmailRequest
                {
                    Subject = "Complete Registration",
                    To = request.EmailAddress,
                    HtmlBody = emailBody                    
                }, cancellationToken);

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

        private string GetActivationLink(string name, string token)
        {
            string activationUrl = $"{_appSettingsConfigOption.PasswordSetupUrl}?seckey={token}";
            string emailBody = EmailTemplateConstant.SetupPassword
                .Replace("[ADMIN_NAME]", name)
                .Replace("[CHANGE_PASSWORD_URL]", activationUrl);
            return emailBody;
        }

    }
}
