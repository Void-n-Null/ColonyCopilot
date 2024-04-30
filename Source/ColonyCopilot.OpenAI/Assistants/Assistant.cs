// Assistant.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ColonyCopilot.OpenAI.Web;
using ColonyCopilot.OpenAI;
using ColonyCopilot.OpenAI.Assistants;
using ColonyCopilot.OpenAI.Functions;
using ColonyCopilot.OpenAI.ResponseModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ColonyCopilot.OpenAI.Assistants
{
    /// <summary>
    /// Represents an assistant in the OpenAI API.
    /// Contains methods for creating, retrieving, and deleting assistants.
    /// </summary>
    public class Assistant
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Instructions { get; private set; }
        public string Model { get; private set; }
        public Client Client { get; private set; }
        
        public List<AIFunction> Functions { get; private set; }

        /// <summary>
        /// No-args constructor for JSON deserialization.
        /// </summary>
        public Assistant() { }
        
        /// <summary>
        /// Creates a new assistant with the specified parameters.
        /// </summary>
        /// <param name="client"> The client to use for the request. </param>
        /// <returns> A list of all assistants found on the client. </returns>
        public static async Task<List<Assistant>> RetrieveAll(Client client)
        {
            var response = await HttpRequestHandler.SendGetRequest("https://api.openai.com/v1/assistants", client.DefaultHeaders);
            var assistantList = JsonConvert.DeserializeObject<AssistantResponseList>(response);
            var output = assistantList.Assistants.Select(data => new Assistant
            {
                Id = data.Id,
                Name = data.Name,
                Instructions = data.Instructions,
                Model = data.Model,
                Client = client
            }).ToList();
            return output;
        }
        
        /// <summary>
        /// Requests an update to the model of the assistant.
        /// </summary>
        /// <param name="newModel"> The new model to use. </param>
        public void RequestUpdateModel(string newModel)
        {
            Task.Run(() => Update(Client, Id, new Dictionary<string, object>
            {
                {"model", newModel}
            }));
            Model = newModel;
        }
        
        /// <summary>
        /// Updates any data of the assistant.
        /// </summary>
        /// <param name="client"> The client to use for the request. </param>
        /// <param name="assistantId"> The ID of the assistant to update. </param>
        /// <param name="data"> The data to change. </param>
        public static async Task Update(Client client, string assistantId, Dictionary<string, object> data)
        {
            var body = JsonConvert.SerializeObject(data);
            await HttpRequestHandler.SendPostRequest(Endpoints.Assistants + $"/{assistantId}", client.DefaultHeaders, body);
        }

        /// <summary>
        /// Creates a new assistant with the specified parameters.
        /// </summary>
        /// <param name="client"> The client to use for the request. </param>
        /// <param name="name"> The name of the assistant. </param>
        /// <param name="model"> The model to use for the assistant. </param>
        /// <param name="instructions"> The instructions for the assistant. </param>
        /// <returns> The created assistant. </returns>
        public static async Task<Assistant> Create(Client client, string name, string model, string instructions = null, List<AIFunction> functions = null)
        {
            if (functions == null)
            {
                functions = new List<AIFunction>();
            }
            
            var toolStrings = new List<string>();
            foreach (var function in functions)
            {
                toolStrings.Add(AIFunction.CreateTool(function));
            }

            var toolString = "[]";
            
            if (toolStrings.Count != 0)
            {
                //Join the tools with a comma
                toolString = toolStrings.Aggregate((current, next) => current + "," + next);
                toolString = "[" + toolString + "]";
            }
            JToken tools = JToken.Parse(toolString);
            var body = JsonConvert.SerializeObject(new
            {
                name,
                model,
                instructions,
                tools
            });
            
            Console.WriteLine(body);
            
            

            var response = await HttpRequestHandler.SendPostRequest(Endpoints.Assistants, client.DefaultHeaders, body);
            var assistantResponse = JsonConvert.DeserializeObject<AssistantResponse>(response);
            
            var assistant = new Assistant
            {
                Id = assistantResponse.Id,
                Name = assistantResponse.Name,
                Instructions = assistantResponse.Instructions,
                Model = assistantResponse.Model,
                Client = client,
                Functions = functions
            };

            client.OnModelChanged += assistant.RequestUpdateModel;
            
            return assistant;
        }

        
        /// <summary>
        /// Deletes the assistant with the specified ID.
        /// </summary>
        /// <param name="client"> The client to use for the request. </param>
        /// <param name="assistantId"> The ID of the assistant to delete. </param>
        public static async Task Delete(Client client, string assistantId)
        {
            await HttpRequestHandler.SendDeleteRequest(Endpoints.Assistants + $"/{assistantId}", client.DefaultHeaders);
        }
        
        /// <summary>
        /// Deletes all assistants on the client.
        /// </summary>
        /// <param name="client"> The client to use for the request. </param>
        public static async Task DeleteAll(Client client)
        {
            var assistants = await RetrieveAll(client);
            int pages = 0;
            while (assistants.Count != 0 || pages < 10)
            {
                foreach (var assistant in assistants)
                {
                    await Delete(client, assistant.Id);
                }
                assistants = await RetrieveAll(client);
                pages++;
                Task.Delay(500);
            }
        }
        
        /// <summary>
        /// Retrieves an assistant with the specified name, or creates one if it doesn't exist.
        /// </summary>
        /// <param name="client"> The client to use for the request. </param>
        /// <param name="name"> The name of the assistant. </param>
        /// <param name="model"> The model to use for the assistant. </param>
        /// <param name="instructions"> The instructions for the assistant. </param>
        /// <returns></returns>
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