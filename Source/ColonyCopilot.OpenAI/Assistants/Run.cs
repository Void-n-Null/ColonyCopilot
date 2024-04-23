// Run.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyCopilot.OpenAI.Web;
using Newtonsoft.Json;

namespace ColonyCopilot.OpenAI.Assistants
{
    public class Run
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("assistant_id")]
        public string AssistantId { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("last_error")]
        public RunError LastError { get; set; }
        
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



        public static async Task<Run> Create(Assistant assistant, Thread thread, string instructions)
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

        public class RunError
        {
            [JsonProperty("code")]
            public string Code;
            [JsonProperty("message")]
            public string Message;
        }
    }
}