using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;
using FluentValidation;

namespace ETS.Application.Events.Commands.AddEventCategory
{
    public record AddEventCategoryCommand(Guid EventId,
        string Title,
        int Qty,
        decimal Price) : ICommand<bool>;

    public class AddEventCategoryCommandValidator : AbstractValidator<AddEventCategoryCommand>
    {
        public AddEventCategoryCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(x => x.Price)
                .NotNull().WithMessage("Price is required")
                .Must(p => p > 0).WithMessage("Price amount must be greater than zero");

            RuleFor(x => x.Qty).GreaterThan(0).WithMessage("Qty must be greater than zero");
        }
    }


    public class AddEventCategoryCommandHandler : ICommandHandler<AddEventCategoryCommand, bool>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventCategoryRepository _eventCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddEventCategoryCommandHandler(IEventRepository eventRepository,
            IEventCategoryRepository eventCategoryRepository,
            IUnitOfWork unitOfWork)
        {
            _eventRepository = eventRepository;
            _eventCategoryRepository = eventCategoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(AddEventCategoryCommand request, CancellationToken cancellationToken)
        {
            var existingEvent = await _eventRepository.ExistsAsync(x => x.Id == request.EventId, 
                cancellationToken);
            if (!existingEvent)
            {
                return Result.Failure<bool>(EventErrors.NotFound);
            }

            var existingEventCategory = await _eventCategoryRepository.ExistsAsync(x => x.Title == request.Title
                        && x.EventId == request.EventId, cancellationToken);
            if (existingEventCategory)
            {
                return Result.Failure<bool>(EventErrors.AlreadyExists);
            }

            var eventCategory = EventCategory.Create(request.EventId, request.Title, request.Qty, request.Price);
            _eventCategoryRepository.Add(eventCategory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

}
