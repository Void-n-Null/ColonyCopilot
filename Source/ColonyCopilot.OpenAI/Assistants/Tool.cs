using System;

namespace ColonyCopilot.OpenAI.Assistants
{
    public class Tool
    {
        public string Type { get; set; }
        
        public Tool(string type)
        {
            Type = type;
        }

    }
}
