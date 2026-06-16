using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ETS.Application.Events.Commands.UpdateEventCategory
{
    public record UpdateEventCategoryCommand(Guid Id, 
        string Title,
        decimal Price,
        int Qty) : ICommand<bool>;

    public class UpdateEventCategoryCommandHandler : ICommandHandler<UpdateEventCategoryCommand, bool>
    {
        private readonly IEventCategoryRepository _eventCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEventCategoryCommandHandler(IEventCategoryRepository eventCategoryRepository, 
            IUnitOfWork unitOfWork)
        {
            _eventCategoryRepository = eventCategoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateEventCategoryCommand request, CancellationToken cancellationToken)
        {
            var existingEventCategory = await _eventCategoryRepository.GetSingleAsync(x => x.Id == request.Id, 
                cancellationToken);

            if(existingEventCategory is null)
            {
                return Result.Failure<bool>(EventErrors.NotFound);
            }

            existingEventCategory.Update(request.Title, request.Qty, request.Price);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

}
