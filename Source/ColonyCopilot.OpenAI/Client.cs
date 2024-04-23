using System;

namespace ColonyCopilot.OpenAI
{
    public class Client
    {
        public readonly string ApiKey;
        
        public Client(string apiKey)
        {
            ApiKey = apiKey;
        }
    }
}
