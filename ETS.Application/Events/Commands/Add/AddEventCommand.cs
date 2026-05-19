using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace ETS.Application.Events.Commands.Add
{
    public record AddEventCommand(IFormFile Thumbnail, 
        string Title, 
        string Description, 
        string Location, 
        DateTime EventDate,
        string StartTime,
        string EndTime) : ICommand<Result<EventsDto>>;


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

    public class AddEventCommandHandler : ICommandHandler<AddEventCommand, Result<EventsDto>>
    {
        private readonly IEventRepository _eventRepository;

        public AddEventCommandHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public Task<Result<Result<EventsDto>>> Handle(AddEventCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

}
