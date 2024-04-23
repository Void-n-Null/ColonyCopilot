// Thread.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColonyCopilot.OpenAI.Web;
using Newtonsoft.Json;
using ColonyCopilot.OpenAI.Assistants;
using ColonyCopilot.OpenAI.ResponseModels;

namespace ColonyCopilot.OpenAI.Assistants
{
    public class Thread
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public Assistant Assistant { get; set; }
        
        public const float DefaultTimeout = 20f;
        
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
            var body = JsonConvert.SerializeObject(new
            {
                role = message.Role.ToString().ToLower(),
                content = message.Content
            });

            var response = await HttpRequestHandler.SendPostRequest(url, headers, body);
            var responseObject = JsonConvert.DeserializeObject<MessageResponse>(response);
            return new Message
            {
                Role = message.Role,
                Content = message.Content,
                Id = responseObject.Id
            };
        }

        public async Task<List<Message>> RetrieveMessages(string beforeMessageID = null)
        {
            var url = $"https://api.openai.com/v1/threads/{Id}/messages";
            var headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {Assistant.Client.ApiKey}" },
                { "OpenAI-Beta", "assistants=v2" },
                { "Content-Type", "application/json" }
            };
            var queryParameters = new Dictionary<string, string>();
            if (beforeMessageID != null)
            {
                queryParameters.Add("before", beforeMessageID);
            }
    
            var response = await HttpRequestHandler.SendGetRequest(url, headers, queryParameters);
            var responseObject = JsonConvert.DeserializeObject<MessageResponseList>(response);
            var messages = new List<Message>();
            foreach (var responseMessage in responseObject.Data)
            {
                Enum.TryParse(responseMessage.Role, true, out Message.RoleType parsedRole);
                messages.Add(new Message
                {
                    Role = parsedRole,
                    Content = responseMessage.Content[0].Text.Value,
                    Id = responseMessage.Id
                });
            }
            return messages;
        }

        public async Task<Run> StartRun(string instructions = "")
        {
            return await Run.Create(Assistant, this, instructions);
        }

        public async Task<string> GetLatestMessageID()
        {
            var messages = await RetrieveMessages();
            return messages[0].Id;
        }
        
        public async Task<String> GetResponse()
        {
            var latestMessageID = await GetLatestMessageID();
            var run = await StartRun();
            float time = 0f;
            
            while (run.Status != "completed")
            {
                
                if (run.Status == "failed")
                {
                    throw new Exception("Run failed. Error: " + $"{run.LastError.Code} - {run.LastError.Message}");
                }
                await Task.Delay(500);
                time += 0.5f;
                if (time >= DefaultTimeout)
                {
                    throw new TimeoutException("Run timed out.");
                }
                run = await run.Retrieve();
            }
            var newMessages = await RetrieveMessages(latestMessageID);
            var response = newMessages[0].Content;
            byte[] nonUnicodeBytes = System.Text.Encoding.Default.GetBytes(response);
            string utf8Text = System.Text.Encoding.UTF8.GetString(nonUnicodeBytes);
            return utf8Text;
        }
    }
}