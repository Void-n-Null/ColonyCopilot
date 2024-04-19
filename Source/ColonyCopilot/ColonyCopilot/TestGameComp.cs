using UnityEngine;
using System;
using OpenAI_API;
using OpenAI_API.Chat;
using Verse;

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
        var auth = new APIAuthentication("sk-proj-U38PNrUNI0bDmuAoWrHOT3BlbkFJ4ffBv3FfcSolwvSPeuKi");
        var openAIClient = new OpenAIAPI(auth);
        var chat = openAIClient.Chat.CreateConversation();
        chat.AppendMessage(new ChatMessage(ChatMessageRole.System, "You are a snarky AI who says snarky things."));
        chat.AppendMessage(new ChatMessage(ChatMessageRole.User, "Hello!"));
        var completion = chat.GetResponseFromChatbotAsync();
        completion.ContinueWith(completedTask => OnMessageCompletion(completedTask.Result));
        completion.Start();
    }
    
    public void OnMessageCompletion(string response)
    {
        Log.Message("COLONY COPILOT: OnMessageCompletion() called!");
        Log.Message(response);
    }
}