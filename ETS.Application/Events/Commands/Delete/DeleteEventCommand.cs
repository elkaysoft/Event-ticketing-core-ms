using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;

namespace ETS.Application.Events.Commands.Delete
{
    public record DeleteEventCommand(Guid EventId) : ICommand<bool>;

    public class DeleteEventCommandHandler : ICommandHandler<DeleteEventCommand, bool>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteEventCommandHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
        {
            _eventRepository = eventRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            var eventResult = await _eventRepository.GetByIdAsync(request.EventId, cancellationToken);
            if (eventResult is null)
            {
                return Result.Failure<bool>(EventErrors.NotFound);
            }

            _eventRepository.Remove(eventResult);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(true);
        }
    }

}
