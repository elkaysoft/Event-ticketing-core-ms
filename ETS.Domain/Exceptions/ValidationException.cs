using ETS.Domain.Common;
using ETS.Domain.Errors;

namespace ETS.Domain.Exceptions
{
    public sealed class ValidationException: Exception
    {
        /// <summary>
        /// Validation exception
        /// </summary>
        /// <param name="validationError"></param>
        public ValidationException(ValidationError validationError)            
        {
            Errors = [.. validationError.Errors];
        }

        public IEnumerable<Error> Errors { get; }
    }
}
