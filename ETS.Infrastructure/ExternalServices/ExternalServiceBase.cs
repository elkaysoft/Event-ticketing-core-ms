using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using System.Web;
using System.Net.Http.Headers;
using ETS.Domain.Common;
using System.Reflection;

namespace ETS.Infrastructure.ExternalServices
{
    public class ExternalServiceBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        protected string BaseUriString = "";
        private readonly int DefaultRestTimeout = 60000;

        public ExternalServiceBase(IServiceProvider serviceProvider, ILogger logger)
        {
            _logger = logger;
            _httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>()!;
            _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>() ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        private int CheckTimeout(int timeout)
        {
            return timeout > 0 ? timeout : DefaultRestTimeout;
        }

        public async Task<T> Send<T>(HttpMethod httpMethod,
            string requestUri,
            object? requestData = null,
            IDictionary<string, string>? httpHeaders = null,
            int timeout = 0,
            bool shouldForwardHeaders = false,
            List<string>? requestAcceptHeaders = null,
            IDictionary<string, string> queryParams = null,
            CancellationToken cancellation = default) where T : class, new()
        {
            HttpContent? content = null;

            if(requestData is not null)
            {
                var serializationSetting = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                var jsonData = JsonConvert.SerializeObject(requestData, serializationSetting);
                content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            }

            return await SendRequestAsync<T>(httpMethod,
                requestUri,
                content,
                httpHeaders,
                timeout,
                shouldForwardHeaders,
                requestAcceptHeaders,
                queryParams,
                responseDeserializer: null,
                cancellation);
        }


        private async Task<T> SendRequestAsync<T>(HttpMethod httpMethod,
            string requestUri,
            HttpContent? httpContent,
            IDictionary<string, string>? httpHeaders,
            int timeout,
            bool shouldForwardHeaders,
            List<string>? requestAcceptHeaders,
            IDictionary<string, string>? queryParams,
            Func<string, HttpResponseMessage, T?>? responseDeserializer,
            CancellationToken cancellationToken = default) where T : class, new()
        {
            timeout = CheckTimeout(timeout);

            HttpClient httpClient = new HttpClient();

            if (shouldForwardHeaders)
            {
                var headers = GetCurrentRequestHeaders();
                if(headers is not null)
                {
                    foreach(var item in headers)
                    {
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value.ToString());
                    }
                }
            }

            httpClient.Timeout = TimeSpan.FromMicroseconds(timeout);

            UriBuilder uriBuilder = new(BaseUriString)
            {
                Path = requestUri?.TrimStart('/')
            };

            if(queryParams != null && queryParams.Count > 0)
            {
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                foreach(var param in queryParams)
                {
                    query[param.Key] = param.Value;
                }
                uriBuilder.Query = query.ToString();
            }

            var absoluteUrl = uriBuilder.Uri.ToString();

            using var httpRequestMessage = new HttpRequestMessage(httpMethod, absoluteUrl);
            if (httpContent != null)
                httpRequestMessage.Content = httpContent;

            if(requestAcceptHeaders is null || !requestAcceptHeaders.Any())
            {
                httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            else
            {
                foreach(var header in requestAcceptHeaders)
                {
                    httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(header));
                }
            }


            if(httpHeaders != null && httpHeaders.Count > 0)
            {
                foreach(var header in httpHeaders)
                {
                    if(httpClient.DefaultRequestHeaders.Contains(header.Key))  
                        httpClient.DefaultRequestHeaders.Remove(header.Key);
                    httpRequestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeout);

            var resp = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cts.Token); 
            var respStr = await resp.Content.ReadAsStringAsync(cts.Token);

            _logger.LogDebug("Response from {requestUri} was {respStr}", requestUri?.SanitizeForLogging(), 
                respStr.SanitizeForLogging());

            if(!resp.IsSuccessStatusCode || string.IsNullOrWhiteSpace(respStr))
            {
                _logger.LogError("Request failed, {response}, {statusCode}", respStr.SanitizeForLogging(), 
                    resp.StatusCode.ToString().SanitizeForLogging());
                return SetHttpResponseMessageOnResponseObject(default(T), resp);
            }

            T? respObj = null;
            try
            {
                respObj = responseDeserializer != null 
                    ? responseDeserializer(respStr, resp) 
                    : JsonConvert.DeserializeObject<T>(respStr);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occured while deserializing the response Message content");
            }
            respObj = SetHttpResponseMessageOnResponseObject(respObj, resp);
            return respObj;
        }

        protected T SetHttpResponseMessageOnResponseObject<T>(T? responseObject, HttpResponseMessage responseMessage) 
            where T : class, new()
        {
            responseObject ??= Activator.CreateInstance<T>();
            try
            {
                var type = typeof(T);
                if (!type.GetInterfaces().Contains(typeof(IHttpResponseMessage)))
                {
                    return responseObject;
                }
                var prop = responseObject.GetType()
                    .GetProperty(nameof(IHttpResponseMessage.ResponseMessage), BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if(prop != null && prop.CanWrite)
                {
                    var existing = prop.GetValue(responseObject) as HttpResponseMessage;
                    if (existing is not null)
                        return responseObject;
                    prop.SetValue(responseObject, responseMessage);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An exception occured while trying to set HttpResponseMessage on response object\nExceptionMessage : {exceptionMessage}",
                    ex.Message.SanitizeForLogging());
            }

            return responseObject;
        }

        private IHeaderDictionary GetCurrentRequestHeaders()
        {
            var currentContext = _httpContextAccessor?.HttpContext;
            if (currentContext is null)
                return null!;

            var authHeader = currentContext.Request.Headers.Authorization.ToString();
            var accessTokenHeader = currentContext.Request.Headers["AccessToken"].ToString();

            IHeaderDictionary headers = new HeaderDictionary();
            headers.Append("Authorization", authHeader);

            return headers;
        }

    }
}
