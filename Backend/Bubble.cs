using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Extensions.OpenAI.TextCompletion;

namespace RealityHack25.MindBubble.Function
{
    public class Bubble
    {
        private readonly ILogger<Bubble> _logger;

        public Bubble(ILogger<Bubble> logger)
        {
            _logger = logger;
        }

        [Function(nameof(WhoIs))]
        public IActionResult WhoIs([HttpTrigger(AuthorizationLevel.Function, Route = "whois/{name}")] HttpRequest req,
        [TextCompletionInput("Who is {name}?", Model = "%CHAT_MODEL_DEPLOYMENT_NAME%", Temperature ="1")] TextCompletionResponse response)
        {
            if (!String.IsNullOrEmpty(response.Content))
            {
                return new OkObjectResult(response.Content);
            }
            else
            {
                return new NotFoundObjectResult("Something went wrong.");
            }
        }

        [Function("Bubble")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
