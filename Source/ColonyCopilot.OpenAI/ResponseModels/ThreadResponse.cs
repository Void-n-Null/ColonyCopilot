
using System;
using Newtonsoft.Json;

namespace ColonyCopilot.OpenAI.ResponseModels
{
    public class ThreadResponse
    {                 
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
