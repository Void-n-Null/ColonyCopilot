using System;

namespace ColonyCopilot.OpenAI.Functions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class AIParameterAttribute : Attribute
    {
        public string Description { get; set; }
        
        public AIParameterAttribute(string description)
        {
            Description = description;
        }
    }
}
