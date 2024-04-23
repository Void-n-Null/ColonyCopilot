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
        
        public class ResponseObject
        {
            [JsonProperty("role")]
            public string Role { get; set; }
            [JsonProperty("content")]
            public List<ContentObject> Content { get; set; }
            [JsonProperty("id")]
            public string Id { get; set; }
        }
        
        public class ResponseListObject
        {
            [JsonProperty("data")]
            public List<ResponseObject> Data { get; set; }
        }
        
        public class ContentObject
        {
            [JsonProperty("type")]
            public string Type;
            [JsonProperty("text")]
            public TextObject Text;
        }
        
        public class TextObject
        {
            [JsonProperty("value")]
            public string Value { get; set; }
            [JsonProperty("annotations")]
            public List<string> Annotations { get; set; }
        }
    }
}
