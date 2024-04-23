
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ColonyCopilot.OpenAI.ResponseModels
{
    public class AssistantResponseList
    {
        [JsonProperty("data")]
        public List<AssistantResponse> Assistants { get; set; }
    }
}
