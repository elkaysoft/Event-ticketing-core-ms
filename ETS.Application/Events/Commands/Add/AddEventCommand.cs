using ETS.Application.Abstraction.Mediation;
using ETS.Application.Users.Commands.RegisterUser;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ETS.Application.Events.Commands.Add
{
    public class EventCategoryRequest
    {
        public required string Title { get; set; }
        public required int Qty { get; set; }
        public required int Price { get; set; }
    }

    public record AddEventCommand(IFormFile Thumbnail, 
        string Title, 
        string Description, 
        string Location, 
        DateTime EventDate,
        string StartTime,
        string EndTime,
        List<EventCategoryRequest> EventCategories) : ICommand<EventsDto>;


    public class AddEventCommandValidator : AbstractValidator<AddEventCommand>
    {
        int FileUploadSizeLimit = 5242880;

        public AddEventCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
            RuleFor(x => x.Location).NotEmpty().WithMessage("Location is required");
            RuleFor(x => x.StartTime).NotEmpty().WithMessage("Start Time is required");
            RuleFor(x => x.EndTime).NotEmpty().WithMessage("End Time is required");

            RuleFor(x => x.Thumbnail)
                .Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("Thumbnail is required")               
               .WithMessage("Incomplete document")
               .Must(DocumentValidation)
               .WithMessage("Invalid document format")
               .Must(DocumentExtentionValidation)
               .WithMessage("Invalid document extension, you are only allowed to upload 'jpg, jpeg, png and pdf' files")
               .Must(DocumentSizeValidation)
               .WithMessage("Document too large, file cannot exceed 1 MB");
        }

        private bool DocumentSizeValidation(IFormFile doc)
        {
            if (doc.Length > FileUploadSizeLimit)
                return false;

            return true;
        }

        private bool DocumentExtentionValidation(IFormFile doc)
        {
            string allowedExt = ".jpg,.png,.jpeg,.pdf";
            string ext = Path.GetExtension(doc.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !allowedExt.Contains(ext))
                return false;

            return true;
        }

        private bool DocumentValidation(IFormFile doc)
        {
            if (doc is not IFormFile || doc.Length == 0)
                return false;

            return true;
        }

    }

    public class AddEventCommandHandler : ICommandHandler<AddEventCommand, EventsDto>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventCategoryRepository _eventCategoryRepository;
        private readonly IDocumentService _documentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AddEventCommandHandler> _logger;


        public AddEventCommandHandler(IEventRepository eventRepository,
            IEventCategoryRepository eventCategoryRepository,
            IDocumentService documentService,
            IUnitOfWork unitOfWork,
            ILogger<AddEventCommandHandler> logger)
        {
            _eventRepository = eventRepository;
            _eventCategoryRepository = eventCategoryRepository;
            _documentService = documentService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<EventsDto>> Handle(AddEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingEvent = await _eventRepository.GetSingleAsync(x => x.Title == request.Title, cancellationToken);
                if (existingEvent != null)
                {
                    return Result.Failure<EventsDto>(EventErrors.AlreadyExists);
                }

                // upload thumbnail and get url
                 var thumbnailUrl = _documentService.UploadDocument(request.Thumbnail);

                var newEvent = Domain.Entities.Events.Create(request.Title, 
                    request.Description, 
                    request.Location, 
                    thumbnailUrl,
                    request.EventDate, 
                    request.StartTime);

                var eventCategories = request.EventCategories.Select(x => EventCategory.Create(newEvent.Id, x.Title, x.Qty, x.Price)).ToList();

                _eventRepository.Add(newEvent); 
                _eventCategoryRepository.AddRange(eventCategories);
                
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new EventsDto
                {
                    BannerUrl = thumbnailUrl,
                    Description = request.Description,
                    Id = newEvent.Id,
                    Location = request.Location,
                    Title = request.Title,
                };
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error adding event {Event}", request.Title.SanitizeForLogging());
                return Result.Failure<EventsDto>(EventErrors.EventCreationFailed);
            }
        }


    }

}
