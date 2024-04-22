using UnityEngine;
using System;
using System.Threading.Tasks;
using RimOpenAI.Web;
using UnityEngine.Networking;

namespace RimOpenAI.Assistants
{
    public class Run
    {
        public string Id { get; set; }
        public string Status { get; set; }
        
        public Thread Thread { get; set; }
        
        public async Task<Run> Retrieve()
        {
            var request = UnityWebRequest.Get($"https://api.openai.com/v1/threads/{Thread.Id}/runs/{Id}");
            request.SetRequestHeader("Authorization", $"Bearer {Thread.Assistant.Client.ApiKey}");
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");
            
            var response = await UwrHandler.SendRequest(request);
            return JsonUtility.FromJson<Run>(response.ToString());
        }
        
        public static async Task<Run> Create(Assistant assistant, Thread thread, string instructions = null)
        {
            var runData = new
            {
                assistant_id = assistant.Id,
                instructions
            };

            var request = UnityWebRequest.Post($"https://api.openai.com/v1/threads/{thread.Id}/runs", JsonUtility.ToJson(runData));
            request.SetRequestHeader("Authorization", $"Bearer {assistant.Client.ApiKey}");
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");

            var response = await UwrHandler.SendRequest(request);
            var run = JsonUtility.FromJson<Run>(response.ToString());
            run.Thread = thread;
            return run;
        }
    }
}
