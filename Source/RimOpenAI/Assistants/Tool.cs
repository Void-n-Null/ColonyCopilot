using UnityEngine;
using System;

namespace RimOpenAI.Assistants
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
