using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Extensions.OpenAI.TextCompletion;
using Azure.AI.OpenAI;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using System.Linq;
using System.Collections;
using System.Text;

namespace RealityHack25.MindBubble.Function
{
    public class Bubble
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<Bubble> _logger;

        const string GPT_FUNCTION_URL = "https://mind-bubble.openai.azure.com/openai/deployments/gpt-4o/chat/completions?api-version=2024-02-15-preview";

        public Bubble(ILogger<Bubble> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }



        /// <summary>
        /// This sample takes a prompt as input, sends it directly to the OpenAI completions API, and results the 
        /// response as the output.
        /// </summary>
        [Function(nameof(GenericCompletion))]
        public IActionResult GenericCompletion(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            [TextCompletionInput("{Query.message}", Model = "%CHAT_MODEL_DEPLOYMENT_NAME%")] TextCompletionResponse response,
            ILogger log)
        {
            _logger.LogInformation($"CAlling GPT with prompt");
            string text = response.Content;
            return new OkObjectResult(text);
        }

        [Function("Bubble")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "mindBubble/{keyword}/{latitude}/{longitude}")] HttpRequest req,
            string keyword,
            double latitude,
            double longitude)
        {
            _logger.LogInformation($"Got Request for: {keyword}/{latitude}/{longitude}");
            _logger.LogInformation("gathering posts");
            var posts = await GatherPosts(keyword, latitude, longitude);
            _logger.LogInformation("got posts");

            // Build the URL for Function B
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("api-key", Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY"));
                var payload = new
                {
                    messages = new object[]
                    {
                  new {
                      role = "system",
                      content = new object[] {
                          new {
                              type = "text",
                              text = "You are an AI assistant that helps people find information."
                          }
                      }
                  },
                  new {
                      role = "user",
                      content = new object[] {
                        //   new {
                        //       type = "image_url",
                        //       image_url = new {
                        //           url = $"data:image/jpeg;base64,{encodedImage}"
                        //       }
                        //   },
                          new {
                              type = "text",
                              text = GeneratePrompt(posts)
                          }
                      }
                  }
                    },
                    temperature = 0.7,
                    top_p = 0.95,
                    max_tokens = 800,
                    stream = false
                };

                var gptResponse = await httpClient.PostAsync(GPT_FUNCTION_URL, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

                if (gptResponse.IsSuccessStatusCode)
                {
                    var responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<GptResponse>(await gptResponse.Content.ReadAsStringAsync());
                    var content = responseData?.Choices[0].Message.Content;
                    content = content?.Replace("```json", "").Replace("```", "");
                    return new OkObjectResult(content);
                }
                _logger.LogError($"Error: {gptResponse.StatusCode}, {gptResponse.ReasonPhrase}");
                return new StatusCodeResult(500);
            }
        }

        private async Task<string> GatherPosts(string keyword, double latitude, double longitude)
        {//right now gather real posts is not feasable .. so fake them using chatGPT
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var locationInfo = latitude != 0 && longitude != 0 ? $" around the coordinates ({latitude}, {longitude})" : "";
                    httpClient.DefaultRequestHeaders.Add("api-key", Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY"));
                    var payload = new
                    {
                        messages = new object[]
                        {
                  new {
                      role = "system",
                      content = new object[] {
                          new {
                              type = "text",
                              text = "You are an AI assistant that generates realistic social media posts based on given hashtags and optional geographical coordinates."
                          }
                      }
                  },
                  new {
                      role = "user",
                      content = new object[] {
                        //   new {
                        //       type = "image_url",
                        //       image_url = new {
                        //           url = $"data:image/jpeg;base64,{encodedImage}"
                        //       }
                        //   },
                          new {
                              type = "text",
                              text = $"Generate 20 realistic social media posts based on the hashtag #{keyword}{locationInfo}. Make them varied, engaging, and suitable for a range of contexts. Ensure the response is in JSON format with the following structure:\n" +
                   "\n" +
                   "```json\n" +
                   "{\n" +
                   "  \"posts\": [\n" +
                   "    { \"content\": \"Post 1 content here\" },\n" +
                   "    { \"content\": \"Post 2 content here\" },\n" +
                   "    ...\n" +
                   "  ]\n" +
                   "}\n" +
                   "```"
                          }
                      }
                  }
                        },
                        temperature = 0.7,
                        top_p = 0.95,
                        max_tokens = 800,
                        stream = false
                    };

                    var gptResponse = await httpClient.PostAsync(GPT_FUNCTION_URL, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

                    if (gptResponse.IsSuccessStatusCode)
                    {
                        var responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(await gptResponse.Content.ReadAsStringAsync());
                        return ""+ responseData;
                    }
                    _logger.LogError($"Error: {gptResponse.StatusCode}, {gptResponse.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating posts");
            }
            //dummy backup
            return "{\n"+
"  \"posts\": [\n"+
"    { \"content\": \"The future of reality is being shaped right here. #VR\" },\n"+
"    { \"content\": \"Testing out some jaw-dropping tech innovations. #VR\" },\n"+
"    { \"content\": \"Our team's project is finally coming together. #Innovation\" },\n"+
"    { \"content\": \"Every moment here is a lesson in innovation. #VR\" },\n"+
"    { \"content\": \"Building something that could change the world. #Boston\" },\n"+
"    { \"content\": \"Can't wait to share our prototype with the judges! #AR\" },\n"+
"    { \"content\": \"Loving the collaborative spirit at the hackathon. #realityHack\" },\n"+
"    { \"content\": \"Breakthrough idea in the making! Stay tuned. 🚀 #Hackathon\" },\n"+
"    { \"content\": \"The view through AR lenses is simply mind-blowing. #Innovation\" },\n"+
"    { \"content\": \"Our team's project is finally coming together. #Innovation\" },\n"+
"    { \"content\": \"Pushing the limits of tech, one line of code at a time. #Hackathon\" },\n"+
"    { \"content\": \"Can't wait to share our prototype with the judges! #Innovation\" },\n"+
"    { \"content\": \"Just wrapped up a great session on spatial mapping! #AR\" },\n"+
"    { \"content\": \"Pushing the limits of tech, one line of code at a time. #Boston\" },\n"+
"    { \"content\": \"Pushing the limits of tech, one line of code at a time. #Hackathon\" },\n"+
"    { \"content\": \"Unity + AR = Endless possibilities. #DevLife #Innovation\" }\n"+
"  ]\n"+
"}\n";
        }

        private string GeneratePrompt(string posts)
        {
            return $"Please summarize and group the following posts into meaningful themes based on their content. Provide the response only in a JSON format with the following structure:\r\n" +
"\r\n" +
"```json\r\n" +
"{\r\n" +
"  \"themes\": {\r\n" +
"    \"Theme Name 1\": [\r\n" +
"      \"Post 1\",\r\n" +
"      \"Post 2\"\r\n" +
"    ],\r\n" +
"    \"Theme Name 2\": [\r\n" +
"      \"Post 3\",\r\n" +
"      \"Post 4\"\r\n" +
"    ]\r\n" +
"  }\r\n" +
"}\r\n" +
"```\r\n" +
"\r\n" +
"```\r\n" +
$"{posts}\r\n" +
"```\r\n" +
"Group similar posts under the same theme name, and name the themes appropriately to reflect the content. Ensure the JSON format is strictly followed with no additional text or explanations.";


        }
    }
}
