using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;

namespace ETS.Application.Events.Commands.UpdateEventCategory
{
    public record UpdateEventCategoryCommand(Guid Id, 
        string Title,
        decimal Price,
        int Qty) : ICommand<bool>;

    public class UpdateEventCategoryCommandHandler : ICommandHandler<UpdateEventCategoryCommand, bool>
    {
        private readonly IEventCategoryRepository _eventCategoryRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEventCategoryCommandHandler(IEventCategoryRepository eventCategoryRepository,
            IUnitOfWork unitOfWork,
            IEventRepository eventRepository)
        {
            _eventCategoryRepository = eventCategoryRepository;
            _unitOfWork = unitOfWork;
            _eventRepository = eventRepository;
        }

        public async Task<Result<bool>> Handle(UpdateEventCategoryCommand request, CancellationToken cancellationToken)
        {
            var eventCategory = await _eventCategoryRepository.GetSingleAsync(x => x.Id == request.Id, 
                cancellationToken, includeExpressions: p => p.Event);                        

            if(eventCategory is null)
            {
                return Result.Failure<bool>(EventErrors.NotFound);
            }

            var theEvent = eventCategory.Event;
            if(theEvent is null)
            {
                return Result.Failure<bool>(EventErrors.NotFound);
            }

            if(request.Qty < eventCategory.UnitSold)
            {
                return Result.Failure<bool>(EventErrors.EventCategoryTooLow);
            }

            // Fetch all event categories upfront before any modification to ensure we have the complete list for accurate total recalculation later.
            var allEventItems = await _eventCategoryRepository.GetAllAsync(x => x.EventId == eventCategory.EventId,
                cancellationToken);
            if (allEventItems.Count == 0)
            {
                return Result.Failure<bool>(EventErrors.NotFound);
            }

            eventCategory.Update(request.Title, request.Qty, request.Price);

            //Explicitly replace the stale reference in allEventItems with the modified eventItems
            var index = allEventItems.FindIndex(i => i.Id == eventCategory.Id);
            if (index >= 0)
                allEventItems[index] = eventCategory;

            // Recalculate cart totals
            RecalculateEventCategories(theEvent, allEventItems);

            _eventRepository.Update(theEvent);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        private void RecalculateEventCategories(Domain.Entities.Events theEvent, List<EventCategory> eventItems)
        {
            var total = eventItems.Any() ? eventItems.Sum(x => x.Qty) : 0;
            theEvent.UpdateTotal(total);
        }

    }

}
