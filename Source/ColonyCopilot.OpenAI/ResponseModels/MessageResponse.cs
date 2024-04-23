
using System;
using System.Collections.Generic;
using ColonyCopilot.OpenAI.Assistants;
using Newtonsoft.Json;

namespace ColonyCopilot.OpenAI.ResponseModels
{
    public class MessageResponse
    {
        [JsonProperty("role")]
        public string Role { get; set; }
        [JsonProperty("content")]
        public List<ContentData> Content { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }
    
    public class ContentData
    {
        [JsonProperty("type")]
        public string Type;
        [JsonProperty("text")]
        public TextData Text;
    }
        
    public class TextData
    {
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("annotations")]
        public List<string> Annotations { get; set; }
    }
}
