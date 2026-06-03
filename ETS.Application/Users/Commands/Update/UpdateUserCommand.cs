using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Enums;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ETS.Application.Users.Commands.Update
{
    public record UpdateUserCommand(long UserId, string FullName, string Email, RoleEnum Role) : ICommand<bool>;

    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("FullName is required");
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email address is required")
                .EmailAddress()
                .WithMessage("Invalid email address");
            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Invalid role");
        }
    }

    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, bool>
    {
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(ILogger<UpdateUserCommandHandler> logger,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetSingleAsync(x => x.Id == request.UserId, cancellationToken);
                if(user == null)
                {
                    return Result.Failure<bool>(UserErrors.NotFound);
                }

                user.UpdateUser(request.Email, request.FullName, request.Role);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success(true);
            }
            catch(Exception ex)
            {
               _logger.LogError(ex, "Error updating user with email {Email}", request.Email);
                return Result.Failure<bool>(UserErrors.UpdateFailed);
            }
        }



    }
}
