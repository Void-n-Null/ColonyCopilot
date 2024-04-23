using UnityEngine;
using System;
using Verse;
using ColonyCopilot.OpenAI;
using ColonyCopilot.OpenAI.Assistants;
using UnityEngine.Networking;
using Message = ColonyCopilot.OpenAI.Assistants.Message;
using Thread = ColonyCopilot.OpenAI.Assistants.Thread;

namespace ColonyCopilot.Rimworld;

public class TestGameComp : GameComponent
{
    public TestGameComp(Game game)
    {
    }

    public override async void StartedNewGame()
    {
        Log.Message("Started new game");
        try
        {
            await TestAssistant();
        } catch (Exception e)
        {
            Log.Error("Error in TestGameComp: " + e);
        }
    }

    
    
    private async Task TestAssistant()
    {
        Log.Message("Testing assistant");
        Client client = new Client("sk-proj-XWB4D8CRUlv4F4sIa9K5T3BlbkFJRq9ldOQEWCO09R9xnNfP");
        Assistant assistant = await Assistant.RetrieveOrCreate(client, "Test Assistant", "gpt-4-turbo", "You are a helpful assistant");
        Thread thread = await Thread.Create(assistant);
        Log.Message("All data structures created");
        Message message = new Message
        {
            Content = "Hey whats up broski, respond funny",
            Role = Message.RoleType.User
        };
        Log.Message("Message created");
        await thread.AddMessage(message);

        Log.Message("Getting Response...");
        var response = await thread.GetResponse();
        Log.Message("Response: \n" + response);
    }
    
}