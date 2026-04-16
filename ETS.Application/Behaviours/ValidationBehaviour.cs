using ETS.Application.Abstraction.Mediation;
using ETS.Domain.Common;
using ETS.Domain.Errors;
using FluentValidation;
using MediatR;
using ValidationException = ETS.Domain.Exceptions.ValidationException;

namespace ETS.Application.Behaviours
{
    public sealed class ValidationBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IBaseCommand
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next(cancellationToken);
            }

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

            var validationErrors = validationResults
                .Where(vr => vr.Errors.Count != 0)
                .SelectMany(vr => vr.Errors)
                .Select(vf => new Error(
                    vf.PropertyName,
                    vf.ErrorMessage))
                .ToList();

            if(validationErrors.Count != 0)
            {
                throw new ValidationException(new ValidationError(validationErrors));                
            }

            return await next(cancellationToken);
        }

    }
}
