using Serilog.Events;
using Serilog.Formatting;

namespace ETS.Domain.Common
{
    public class LogSanitizer : ITextFormatter
    {
        readonly ITextFormatter _formatter;

        public LogSanitizer(ITextFormatter formatter)
        {
            _formatter = formatter;
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            var sw = new StringWriter();
            _formatter.Format(logEvent, sw);
            var messageString = sw.ToString();
            var sanitizedMessage = messageString.SanitizeForLogging();

            output.WriteLine(sanitizedMessage);
        }
    }
}
