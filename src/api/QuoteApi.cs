using System.Net;
using System.Text.Json;
using Api.Accessors;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
namespace Api
{
    public class QuoteApi
    {
        private readonly IQuoteAccessor _quoteAccessor;
        private readonly ILogger _logger;
        
        public QuoteApi(IQuoteAccessor quoteAccessor, ILoggerFactory loggerFactory)
        {
            _quoteAccessor = quoteAccessor;
            _logger = loggerFactory.CreateLogger<Quote>();
        }

        [Function(nameof(GetQuotes))]
        public HttpResponseData GetQuotes([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quote")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            var jsonOpts = new JsonSerializerOptions() {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            response.WriteString(JsonSerializer.Serialize(_quoteAccessor.GetQuotes(), jsonOpts));

            return response;
        }
    }
}
