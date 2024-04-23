
using System;
using System.Reflection;

namespace ColonyCopilot.OpenAI.Functions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AIFunctionAttribute : Attribute
    {
        public string Description { get; set; }
        public string Name { get; set; }
        
        public AIFunctionAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
