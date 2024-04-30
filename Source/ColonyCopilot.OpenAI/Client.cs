using System;
using System.Collections.Generic;

namespace ColonyCopilot.OpenAI
{
    public class Client
    {
        public readonly string ApiKey;
        public string Model { get; set; } = "gpt-4-turbo";
        public delegate void OnModelChange(string newModel);
        public event OnModelChange OnModelChanged;
        public Dictionary<string, string> DefaultHeaders => new Dictionary<string, string>
        {
            {"Authorization", "Bearer " + ApiKey},
            {"OpenAI-Beta", "assistants=v2"},
            {"Content-Type", "application/json"}
        };
        
        public Client(string apiKey)
        {
            ApiKey = apiKey;
        }
        
        public void SetModel(string newModel)
        {
            Model = newModel;
            OnModelChanged?.Invoke(newModel);
        }
    }
}
