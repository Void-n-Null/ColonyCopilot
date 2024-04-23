// Assistant.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColonyCopilot.OpenAI.Web;
using ColonyCopilot.OpenAI;
using ColonyCopilot.OpenAI.Assistants;
using ColonyCopilot.OpenAI.ResponseModels;
using Newtonsoft.Json;

namespace ColonyCopilot.OpenAI.Assistants
{
    public class Assistant
    {
        [JsonProperty("id")]
        public string Id { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("instructions")]
        public string Instructions { get; private set; }
        [JsonProperty("tools")]
        public List<Tool> Tools { get; set; }
        [JsonProperty("model")]
        public string Model { get; private set; }
        public Client Client { get; private set; }

        private Assistant() { }

        public static async Task<List<Assistant>> RetrieveAll(Client client)
        {
            var url = "https://api.openai.com/v1/assistants";
            var headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + client.ApiKey},
                {"OpenAI-Beta", "assistants=v2"}
            };

            var response = await HttpRequestHandler.SendGetRequest(url, headers);
            var assistantList = JsonConvert.DeserializeObject<AssistantResponseList>(response);
            var output = assistantList.Assistants.Select(data => new Assistant
            {
                Id = data.Id,
                Name = data.Name,
                Instructions = data.Instructions,
                Tools = new List<Tool>(),
                Model = data.Model,
                Client = client
            }).ToList();
            return output;
        }

        public static async Task<Assistant> Create(Client client, string name, string model, string instructions = null)
        {
            var url = "https://api.openai.com/v1/assistants";
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + client.ApiKey },
                { "OpenAI-Beta", "assistants=v2" }
            };
            var body = JsonConvert.SerializeObject(new
            {
                name = name,
                model = model,
                instructions = instructions
            });

            var response = await HttpRequestHandler.SendPostRequest(url, headers, body);
            var assistantResponse = JsonConvert.DeserializeObject<AssistantResponse>(response);
            
            var assistant = new Assistant
            {
                Id = assistantResponse.Id,
                Name = assistantResponse.Name,
                Instructions = assistantResponse.Instructions,
                Model = assistantResponse.Model,
                Tools = new List<Tool>(),
                Client = client
            };
            

            return assistant;
        }
        public static async Task Delete(Client client, string assistantId)
        {
            var url = "https://api.openai.com/v1/assistants/" + assistantId;
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + client.ApiKey },
                { "OpenAI-Beta", "assistants=v2" }
            };

            await HttpRequestHandler.SendDeleteRequest(url, headers);
        }
        
        public static async Task DeleteAll(Client client)
        {
            var assistants = await RetrieveAll(client);
            foreach (var assistant in assistants)
            {
                await Delete(client, assistant.Id);
            }
        }
        
        public static async Task<Assistant> RetrieveOrCreate(Client client, string name, string model, string instructions = null)
        {
            var existingAssistants = await RetrieveAll(client);
            foreach (var existingAssistant in existingAssistants)
            {
                if (existingAssistant.Name != name) continue;
                return existingAssistant;
            }
            return await Create(client, name, model, instructions);
        }
    }
}