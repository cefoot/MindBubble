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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            var posts = new[]{"🚀Just kicked off my first #Hackathon! Building something cool with AI and IoT. Fingers crossed 🤞 #Innovation",
"Working non-stop for 12 hours now. Coffee is my best friend. ☕ #Hackathon #TeamNoSleep",
"The excitement of turning ideas into reality in just 48 hours! 🛠️ #HackathonLife",
"Our team is exploring blockchain for secure data sharing. Let's see where this goes! #Hackathon #Crypto",
"Day 2: Bugs, stress, and lots of laughs with the team. This is why I love #Hackathons! ❤️",
"Looking forward to pitching our idea tomorrow! Time to rehearse. 🎤 #Hackathon",
"Learning so much from mentors and peers at this event. The community is amazing! #Hackathon #Networking",
"Finally figured out the API integration. Victory never tasted so sweet! 🎉 #HackathonLife",
"Our project is called 'GreenTechAI'—aiming to make sustainable living accessible with AI. 🌍 #Hackathon",
"Sleep is overrated when you’re building the next big thing! 😎 #Hackathon #TeamWork",
"Our prototype is live! Can't wait to showcase it to the judges tomorrow. #Hackathon #Innovation",
"Highlight of the day: Free pizza and energy drinks. 🍕🥤 #HackathonSurvival",
"We’ve hit a roadblock, but the team is staying positive. Time to brainstorm a workaround. 🧠 #Hackathon",
"Joining forces with incredible people to solve real-world problems. Let’s do this! 💪 #Hackathon",
"Our project idea: An app that detects emotions and suggests mood-based playlists. 🎵😊 #Hackathon",
"3 AM coding sessions, debugging marathons, and a growing pile of snacks. #HackathonVibes",
"Just attended an amazing workshop on machine learning. So inspired right now! #HackathonLearning",
"The power of collaboration: 3 strangers, 1 vision, and a lot of code. 💻 #HackathonMagic",
"Quick power nap and back to coding. No time to waste! 💤💻 #HackathonHustle",
"Win or lose, this experience has been incredible. Thank you to everyone who made it possible! 🙌 #Hackathon"};


            // Build the URL for Function B
            string functionBUrl = "https://mind-bubble.openai.azure.com/openai/deployments/gpt-4o/chat/completions?api-version=2024-02-15-preview";
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

                var gptResponse = await httpClient.PostAsync(functionBUrl, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

                if (gptResponse.IsSuccessStatusCode)
                {
                    var responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<GptResponse>(await gptResponse.Content.ReadAsStringAsync());
                    Console.WriteLine(responseData);
                    return new OkObjectResult(responseData.Choices[0].Message.Content);
                }
                else
                {
                    _logger.LogError($"Error: {gptResponse.StatusCode}, {gptResponse.ReasonPhrase}");
                    return new StatusCodeResult(500);
                }
            }

            // Extract and return the response
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new StatusCodeResult(500);
        }

        private string GeneratePrompt(string[] posts)
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
$"{String.Join(";\r\n", posts)}" +
"Group similar posts under the same theme name, and name the themes appropriately to reflect the content. Ensure the JSON format is strictly followed with no additional text or explanations.";


        }
    }
}
