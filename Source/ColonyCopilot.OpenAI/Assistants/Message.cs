using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ColonyCopilot.OpenAI.Assistants
{
    public class Message
    {
        public RoleType Role { get; set; }
        public string Content { get; set; }
        public string Id { get; set; }
        
        public enum RoleType
        {
            User,
            Assistant
        }
        
        public static Message User(string content)
        {
            return new Message
            {
                Role = RoleType.User,
                Content = content
            };
        }
        
        public static Message Assistant(string content)
        {
            return new Message
            {
                Role = RoleType.Assistant,
                Content = content
            };
        }
    }
}
