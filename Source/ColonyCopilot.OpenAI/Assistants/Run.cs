// Run.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyCopilot.OpenAI.Web;
using Newtonsoft.Json;

namespace ColonyCopilot.OpenAI.Assistants
{
    /// <summary>
    /// Represents a run of an assistant.
    /// Very simple implementation, only includes the properties needed for the Colony Copilot project.
    /// </summary>
    public class Run
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("assistant_id")] 
        private string AssistantId { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("last_error")]
        public RunError LastError { get; set; }
        
        [JsonIgnore]
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
            var run = JsonConvert.DeserializeObject<Run>(response);
            run.Thread = Thread;
            run.AssistantId = AssistantId;
            return run;
        }

        public async Task<List<RunStep>> RetrieveSteps()
        {
            var url = $"https://api.openai.com/v1/threads/{Thread.Id}/runs/{Id}/steps";
            var response = await HttpRequestHandler.SendGetRequest(url, Thread.Assistant.Client.DefaultHeaders);
            var runStepList = JsonConvert.DeserializeObject<RunStepList>(response);
            return runStepList.Data;
        }

        public static async Task<Run> Create(Assistant assistant, Thread thread, string instructions = "")
        {
            var runValues = new
            {
                assistant_id = assistant.Id,
                instructions
            };
            var body = JsonConvert.SerializeObject(runValues);

            var response = await HttpRequestHandler.SendPostRequest($"https://api.openai.com/v1/threads/{thread.Id}/runs",
                                                                            assistant.Client.DefaultHeaders,
                                                                            body);
            var run = JsonConvert.DeserializeObject<Run>(response); 
            run.Thread = thread;
            return run;
        }
        
        public async Task Cancel()
        {
            var url = $"{Endpoints.Threads}/{Thread.Id}/runs/{Id}/cancel";
            await HttpRequestHandler.SendPostRequest(url, Thread.Assistant.Client.DefaultHeaders);
        }

        public abstract class RunError
        {
            [JsonProperty("code")]
            public string Code;
            [JsonProperty("message")]
            public string Message;
        }
    }
}