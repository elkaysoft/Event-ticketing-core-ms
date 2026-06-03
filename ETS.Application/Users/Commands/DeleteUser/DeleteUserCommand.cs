using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;

namespace ETS.Application.Users.Commands.DeleteUser
{
    public record DeleteUserCommand(long Id) : ICommand<bool>;

    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserCommandHandler(IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetSingleAsync(x => x.Id == request.Id, cancellationToken);
            if (user == null)
            {
                return Result.Failure<bool>(UserErrors.NotFound);
            }

            user.SoftDelete();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(true);
        }
    }
}
