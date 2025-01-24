using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace RealityHack25.MindBubble.Function
{
    public class Bubble
    {
        private readonly ILogger<Bubble> _logger;

        public Bubble(ILogger<Bubble> logger)
        {
            _logger = logger;
        }

        [Function("Bubble")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
