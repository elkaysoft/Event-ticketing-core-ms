using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS.Domain.Common
{
    public record Error
    {
        public static readonly Error None = new(string.Empty, string.Empty);
        public static readonly Error NullValue = new("99", "The value was not provided");


        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public string Code { get; set; }
        public string Message { get; set; }

        public static Error Create(string code, string message) => new(code, message);

    }
}
