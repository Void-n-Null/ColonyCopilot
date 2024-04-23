using System;
using System.Reflection;

namespace ColonyCopilot.OpenAI.Functions
{
    public class AIFunction
    {
        public MethodInfo Method { get; set; }
        public ParameterInfo[] Parameters { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }
}
