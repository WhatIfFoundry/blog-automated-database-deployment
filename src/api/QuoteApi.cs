using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
namespace api
{
    public class QuoteApi
    {
        private readonly ILogger _logger;
        private readonly Quote[] _quotes;

        public QuoteApi(Quote[] quotes, ILoggerFactory loggerFactory)
        {
            _quotes = quotes;
            _logger = loggerFactory.CreateLogger<Quote>();
        }

        [Function(nameof(GetQuotes))]
        public HttpResponseData GetQuotes([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quote")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            response.WriteString(JsonSerializer.Serialize(_quotes));

            return response;
        }
    }
}
