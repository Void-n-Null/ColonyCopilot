using System.Collections.Generic;
using System.Threading.Tasks;
using RimOpenAI;
using RimOpenAI.Assistants;
using RimOpenAI.Web;
using UnityEngine;
using UnityEngine.Networking;

namespace RimOpenAI.Assistants
{
    public class Assistant
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Instructions { get; private set; }
        public List<Tool> Tools { get; set; }
        public string Model { get; private set; }
        public Client Client { get; private set; }

        private Assistant() { }

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

        private async Task<Assistant> SaveToServer()
        {
            var request = UnityWebRequest.Post("https://api.openai.com/v1/assistants", JsonUtility.ToJson(this));
            request.SetRequestHeader("Authorization", "Bearer " + Client.ApiKey);
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");

            var response = await UwrHandler.SendRequest(request);
            var savedAssistant = JsonUtility.FromJson<Assistant>(response.ToString());
            Id = savedAssistant.Id;
            return savedAssistant;
        }

        public async Task<Assistant> Retrieve(string assistantId)
        {
            var request = UnityWebRequest.Get("https://api.openai.com/v1/assistants/" + assistantId);
            request.SetRequestHeader("Authorization", "Bearer " + Client.ApiKey);
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");

            var response = await UwrHandler.SendRequest(request);
            return JsonUtility.FromJson<Assistant>(response.ToString());
        }
    }
}