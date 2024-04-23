using System.Collections.Generic;
using Newtonsoft.Json;

namespace ColonyCopilot.OpenAI.ResponseModels
{
    public class MessageResponseList
    {
        [JsonProperty("data")]
        public List<MessageResponse> Data { get; set; }
    }
}
