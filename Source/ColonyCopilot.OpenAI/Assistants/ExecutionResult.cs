using System.Collections.Generic;

namespace ColonyCopilot.OpenAI.Assistants
{
    public class ExecutionResult
    {
        public string Content { get; set; }
        public int ToolsUsedCount { get; set; }
        public List<Dictionary<string,string>> ToolOutputs { get; set; }
        
        public ExecutionResult(string content, int toolsUsedCount, List<Dictionary<string,string>> toolOutputs)
        {
            Content = content;
            ToolsUsedCount = toolsUsedCount;
            ToolOutputs = toolOutputs;
        }
    }
}