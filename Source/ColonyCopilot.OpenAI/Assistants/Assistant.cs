// Assistant.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyCopilot.OpenAI.Web;
using ColonyCopilot.OpenAI;
using ColonyCopilot.OpenAI.Assistants;
using Newtonsoft.Json;

namespace ColonyCopilot.OpenAI.Assistants
{
    public class Assistant
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Instructions { get; private set; }
        public List<Tool> Tools { get; set; }
        public string Model { get; private set; }
        public Client Client { get; private set; }
        
        private class ResponseObject
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("instructions")]
            public string Instructions { get; set; }
            [JsonProperty("model")]
            public string Model { get; set; }
        }

        private class ResponseObjectList
        {
            [JsonProperty("data")]
            public List<ResponseObject> Data { get; set; }
            
        }

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
            var responseObject = JsonConvert.DeserializeObject<ResponseObjectList>(response.ToString());
            var assistants = new List<Assistant>();
            
            foreach (var assistant in responseObject.Data)
            {
                assistants.Add(new Assistant
                {
                    Id = assistant.Id,
                    Name = assistant.Name,
                    Instructions = assistant.Instructions,
                    Model = assistant.Model,
                    Tools = new List<Tool>(),
                    Client = client
                });
            }
            
            return assistants;
        }

        public static async Task<Assistant> Create(Client client, string name, string model, string instructions = null)
        {
            var assistant = new Assistant
            {
                Name = name,
                Model = model,
                Instructions = instructions,
                Client = client
            };

            return await assistant.SaveToServer();
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

        private async Task<Assistant> SaveToServer()
        {
            var url = "https://api.openai.com/v1/assistants";
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + Client.ApiKey },
                { "OpenAI-Beta", "assistants=v2" }
            };
            var body = JsonConvert.SerializeObject(new
            {
                name = Name,
                model = Model,
                instructions = Instructions
            });

            var response = await HttpRequestHandler.SendPostRequest(url, headers, body);
            var responseObject = JsonConvert.DeserializeObject<ResponseObject>(response.ToString());
            
            Id = responseObject.Id;
            Name = responseObject.Name;
            Instructions = responseObject.Instructions;
            Model = responseObject.Model;
            Tools = new List<Tool>();

            return this;
        }
    }
}