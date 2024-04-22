// Run.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyCopilot.OpenAI.Web;
using Newtonsoft.Json;

namespace ColonyCopilot.OpenAI.Assistants
{
    public class Run
    {
        public string Id { get; set; }
        public string Status { get; set; }
        
        public Thread Thread { get; set; }
        
        public async Task<Run> Retrieve()
        {
            var url = $"https://api.openai.com/v1/threads/{Thread.Id}/runs/{Id}";
            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {Thread.Assistant.Client.ApiKey}" },
                { "OpenAI-Beta", "assistants=v2" }
            };

            var response = await HttpRequestHandler.SendGetRequest(url, headers);
            return JsonConvert.DeserializeObject<Run>(response);
        }
        
        public static async Task<Run> Create(Assistant assistant, Thread thread, string instructions = null)
        {
            var runData = new
            {
                assistant_id = assistant.Id,
                instructions
            };

            var url = $"https://api.openai.com/v1/threads/{thread.Id}/runs";
            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {assistant.Client.ApiKey}" },
                { "OpenAI-Beta", "assistants=v2" }
            };
            var body = JsonConvert.SerializeObject(runData);

            var response = await HttpRequestHandler.SendPostRequest(url, headers, body);
            var run = JsonConvert.DeserializeObject<Run>(response);
            run.Thread = thread;
            return run;
        }
    }
}