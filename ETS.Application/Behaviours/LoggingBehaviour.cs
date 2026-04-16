using ETS.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ETS.Application.Behaviours
{
    public sealed class LoggingBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IBaseRequest
        where TResponse : Result
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, 
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            string requestName = request.GetType().Name;

            try
            {
                _logger.LogInformation("Executing request {RequestName}", requestName);

                TResponse result = await next();

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Request {RequestName} processed successfully", requestName);
                }
                else
                {
                    _logger
                        .LogError(
                        "Request {RequestName} processed with error: {ErrorCode} {ErrorDescription}", 
                        requestName,
                        result.Error.Code.SanitizeForLogging(),
                        result.Error.Message.SanitizeForLogging());
                }

                return result;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Request {RequestName} processing failure", requestName);

                throw;
            }
        }


    }
}
