// Thread.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyCopilot.OpenAI.Web;
using Newtonsoft.Json;
using ColonyCopilot.OpenAI.Assistants;

namespace ColonyCopilot.OpenAI.Assistants
{
    public class Thread
    {
        public string Id { get; set; }
        public Assistant Assistant { get; set; }
        
        private class ResponseObject 
        {
            [JsonProperty("id")]
            public string Id { get; set; }
        }

        public static async Task<Thread> Create(Assistant assistant)
        {
            var url = "https://api.openai.com/v1/threads";
            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {assistant.Client.ApiKey}" },
                { "OpenAI-Beta", "assistants=v2" }
            };
            
            var response = await HttpRequestHandler.SendPostRequest(url, headers, "");
            //Get just the ID out of the response
            var responseData = JsonConvert.DeserializeObject<ResponseObject>(response);
            var thread = new Thread
            {
                Id = responseData.Id,
                Assistant = assistant
            };
            return thread;
        }

        public async Task<Message> AddMessage(Message message)
        {
            var url = $"https://api.openai.com/v1/threads/{Id}/messages";
            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {Assistant.Client.ApiKey}" },
                { "OpenAI-Beta", "assistants=v2" }
            };
            var body = JsonConvert.SerializeObject(message);

            var response = await HttpRequestHandler.SendPostRequest(url, headers, body);
            return JsonConvert.DeserializeObject<Message>(response);
        }

        public async Task<List<Message>> RetrieveMessages()
        {
            var url = $"https://api.openai.com/v1/threads/{Id}/messages";
            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {Assistant.Client.ApiKey}" },
                { "OpenAI-Beta", "assistants=v2" }
            };

            var response = await HttpRequestHandler.SendGetRequest(url, headers);
            return JsonConvert.DeserializeObject<List<Message>>(response);
        }

        public async Task<Run> StartRun(string instructions = null)
        {
            return await Run.Create(Assistant, this, instructions);
        }
    }
}