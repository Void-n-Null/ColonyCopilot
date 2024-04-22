using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RimOpenAI.Assistants;
using RimOpenAI.Web;
using UnityEngine.Networking;

namespace RimOpenAI.Assistants
{
    public class Thread
    {
        public string Id { get; set; }
        public List<Message> Messages { get; set; }
        public Assistant Assistant { get; set; }

        public static async Task<Thread> Create(Assistant assistant)
        {
            var request = UnityWebRequest.Post("https://api.openai.com/v1/threads", "");
            request.SetRequestHeader("Authorization", $"Bearer {assistant.Client.ApiKey}");
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");
            
            var response = await UwrHandler.SendRequest(request);
            var thread = JsonUtility.FromJson<Thread>(response.ToString());
            thread.Assistant = assistant;
            return thread;
        }

        public async Task<Message> AddMessage(Message message)
        {
            var request = UnityWebRequest.Post($"https://api.openai.com/v1/threads/{Id}/messages", JsonUtility.ToJson(message));
            request.SetRequestHeader("Authorization", $"Bearer {Assistant.Client.ApiKey}");
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");

            var response = await UwrHandler.SendRequest(request);
            return JsonUtility.FromJson<Message>(response.ToString());
        }

        public async Task<List<Message>> RetrieveMessages()
        {
            var request = UnityWebRequest.Get($"https://api.openai.com/v1/threads/{Id}/messages");
            request.SetRequestHeader("Authorization", $"Bearer {Assistant.Client.ApiKey}");
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");

            var response = await UwrHandler.SendRequest(request);
            return JsonUtility.FromJson<List<Message>>(response.ToString());
        }

        public async Task<Run> StartRun(string instructions = null)
        {
            return await Run.Create(Assistant, this, instructions);
        }

        // Add methods for retrieving messages and runs if needed
    }
    
    
}
