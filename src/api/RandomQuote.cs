using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace api
{
    public class Quote
    {
        private readonly ILogger _logger;

        public Quote(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Quote>();
        }

        [Function(nameof(RandomQuote))]
        public HttpResponseData RandomQuote([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req, Quote[] quotes)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }
    }
}
