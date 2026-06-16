using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;

namespace ETS.Application.Events.Commands.DeleteEventCategory
{
    public record DeleteEventCategoryCommand(Guid Id) : ICommand<bool>;

    public class DeleteEventCategoryCommandHandler : ICommandHandler<DeleteEventCategoryCommand, bool>
    {
        private readonly IEventCategoryRepository _eventCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteEventCategoryCommandHandler(IEventCategoryRepository eventCategoryRepository,
            IUnitOfWork unitOfWork)
        {
            _eventCategoryRepository = eventCategoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteEventCategoryCommand request, CancellationToken cancellationToken)
        {
            var eventCategory = await _eventCategoryRepository.GetSingleAsync(x => x.Id == request.Id, 
                cancellationToken);
            if(eventCategory is null)
            {
                return Result.Failure<bool>(EventErrors.NotFound);
            }

            _eventCategoryRepository.Remove(eventCategory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

}
