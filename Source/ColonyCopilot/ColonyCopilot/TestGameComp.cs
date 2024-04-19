using UnityEngine;
using System;
using OpenAI;
using OpenAI.Chat;
using Verse;
using Message = OpenAI.Chat.Message;

namespace ColonyCopilot;

public class TestGameComp : GameComponent
{
    public TestGameComp(Game game)
    {
        Log.Message("COLONY COPILOT: Constructed TestGameComp");
    }


    public override void LoadedGame()
    {
        Log.Message($"COLONY COPILOT: LoadedGame() called!");
        var auth = new OpenAI.OpenAIAuthentication("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");
        var openAIClient = new OpenAI.OpenAIClient(auth);
        var messages = new List<Message>
        {
            new(Role.System, "You are a snarky AI that says things like 'I'm sorry Dave, I'm afraid I can't do that'"),
            new(Role.User, "Hello, AI!"),
        };
        var request = new ChatRequest(messages);
        var completion = openAIClient.ChatEndpoint.GetCompletionAsync(request);
        completion.ContinueWith(completedTask => OnMessageCompletion(completedTask.Result));
        completion.Start();
    }
    
    public void OnMessageCompletion(ChatResponse response)
    {
        Log.Message("COLONY COPILOT: OnMessageCompletion() called!");
        Log.Message(response.FirstChoice.Message.Content);
    }
}