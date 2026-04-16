using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS.Domain.Common
{
    public static class Helpers
    {
        public static string SanitizeForLogging(this string? input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return string.Create(input.Length, input, static (span, s) =>
            {
                for(var i = 0; i < s.Length; i++)
                {
                    var c = s[i];
                    if(c is '\r' or '\n' or '\t' or '\u2028' or '\u2029')
                    {
                        span[i] = ' ';
                    }
                    else if (char.IsControl(c))
                    {
                        span[i] = ' ';
                    }
                    else
                    {
                        span[i] = c;
                    }
                }
            });
        }
    }
}
