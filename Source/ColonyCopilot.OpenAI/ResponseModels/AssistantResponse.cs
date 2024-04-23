using System;
using Newtonsoft.Json;

namespace ColonyCopilot.OpenAI.ResponseModels
{
    public class AssistantResponse
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
}
