using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ColonyCopilot.OpenAI.Assistants
{
    public class Message
    {
        [JsonProperty("role")]
        public RoleType Role { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        
        public enum RoleType
        {
            User,
            Assistant
        }
        

        

    }
}
