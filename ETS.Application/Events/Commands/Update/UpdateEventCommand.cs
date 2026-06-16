using ETS.Application.Abstraction.Mediation;
using ETS.Application.Events.Commands.Add;
using ETS.Domain.Common;
using ETS.Domain.Contracts;
using ETS.Domain.Entities;
using ETS.Domain.Enums;
using ETS.Domain.Errors;
using ETS.Domain.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ETS.Application.Events.Commands.Update
{

    public record UpdateEventCommand(IFormFile? Thumbnail,
        Guid EventId,
        string Title,
        string Description,
        string Location,
        DateTime EventDate,
        string StartTime,
        string EndTime,
        PublishStatus PublishStatus) : ICommand<bool>;


    public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
    {
        int FileUploadSizeLimit = 5242880;
        public UpdateEventCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
            RuleFor(x => x.Location).NotEmpty().WithMessage("Location is required");
            RuleFor(x => x.StartTime).NotEmpty().WithMessage("Start Time is required");
            RuleFor(x => x.EndTime).NotEmpty().WithMessage("End Time is required");

            RuleFor(x => x.Thumbnail)
                .Cascade(CascadeMode.Stop)
               .Must(DocumentExtentionValidation)
               .WithMessage("Invalid document extension, you are only allowed to upload 'jpg, jpeg, png and pdf' files")
               .Must(DocumentSizeValidation)
               .WithMessage("Document too large, file cannot exceed 1 MB");

        }

        private bool DocumentSizeValidation(IFormFile? doc)
        {
            if (doc is not null)
            {
                if (doc.Length > FileUploadSizeLimit)
                    return false;
            }

            return true;
        }

        private bool DocumentExtentionValidation(IFormFile? doc)
        {
            string allowedExt = ".jpg,.png,.jpeg,.pdf";
            if (doc is not null)
            {
                var ext = Path.GetExtension(doc.FileName);
                if (!allowedExt.Contains(ext))
                    return false;
            }

            return true;
        }
    }

    public class UpdateEventCommandHandler : ICommandHandler<UpdateEventCommand, bool>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventCategoryRepository _eventCategoryRepository;
        private readonly IDocumentService _documentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateEventCommandHandler> _logger;

        public UpdateEventCommandHandler(IEventRepository eventRepository,
            IEventCategoryRepository eventCategoryRepository,
            IDocumentService documentService,
            IUnitOfWork unitOfWork,
            ILogger<UpdateEventCommandHandler> logger)
        {
            _eventRepository = eventRepository;
            _eventCategoryRepository = eventCategoryRepository;
            _documentService = documentService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        public async Task<Result<bool>> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingEvent = await _eventRepository.GetSingleAsync(x => x.Id == request.EventId, cancellationToken);
                if (existingEvent == null)
                {
                    return Result.Failure<bool>(EventErrors.NotFound);
                }

                var eventOverlapping = await _eventRepository.IsUpdatedEventOverlapping(request.EventId,
                    request.Location, 
                    request.EventDate,
                    request.StartTime, 
                    cancellationToken);
                if (eventOverlapping)
                {
                    return Result.Failure<bool>(EventErrors.OverlappingEventError);
                }

                string thumbnailUrl = existingEvent.BannerUrl;
                if(request.Thumbnail is not null)
                {
                    thumbnailUrl = _documentService.UploadDocument(request.Thumbnail);
                }

                existingEvent.Update(request.Title,
                    request.Description, 
                    request.Location, 
                    thumbnailUrl, 
                    request.EventDate, 
                    request.StartTime,
                    request.EndTime,
                    request.PublishStatus);
                               

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success(true);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating event with title {Title}", request.Title);
                return Result.Failure<bool>(EventErrors.EventCreationFailed);
            }
        }


    }
}
