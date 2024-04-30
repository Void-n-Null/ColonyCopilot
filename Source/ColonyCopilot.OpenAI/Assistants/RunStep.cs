using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ColonyCopilot.OpenAI.Assistants
{
    public class RunStep
    {
        [JsonProperty("thread_id")]
        public string ThreadId { get; set; }
        [JsonProperty("run_id")]
        public string RunId { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("step_details")]
        public StepDetails StepDetails { get; set; }
        
    }
    
    public class StepDetails
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("tool_calls")]
        public List<FunctionToolCall> ToolCalls { get; set; }
    }

    public class FunctionToolCall
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("type")]
        public string Type;
        [JsonProperty("function")]
        public FunctionObject Function;
    }

    public class FunctionObject
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("arguments")]
        public string Arguments;
        [JsonProperty("output")]
        public string Output;
    }
    
    public class RunStepList
    {
        [JsonProperty("data")]
        public List<RunStep> Data { get; set; }
    }
}
