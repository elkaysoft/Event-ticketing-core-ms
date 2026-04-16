using ETS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS.Domain.Errors
{
    public sealed record ValidationError : Error
    {
        public ValidationError(IEnumerable<Error> errors)
            :base("Validation.General",
                 "One or more validation error occurred")
        {
            Errors = errors;
        }

        public IEnumerable<Error> Errors { get; }

        public static ValidationError FromResults(IEnumerable<Result> results) =>
            new([.. results.Where(r => r.IsFailure).Select(r => r.Error)]);
    }
}
