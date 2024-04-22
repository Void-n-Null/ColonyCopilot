using UnityEngine;
using System;
using System.Net.Cache;
using RimOpenAI;
using RimOpenAI.Assistants;
using RimOpenAI.Web;
using UnityEngine.Networking;
using Verse;
using Message = RimOpenAI.Assistants.Message;
using Thread = RimOpenAI.Assistants.Thread;

namespace ColonyCopilot;

/// <summary>
/// Interact with the API Wrapper to handle interactions with LLMs
/// </summary>
public static class APILayer
{
    public static async Task<string> GetURL(string url)
    {
        string response;
        var request = new UnityWebRequest(url);
        try
        {
            response = (string)await UwrHandler.SendRequest(request);
            return response;
        }
        catch (Exception e)
        {
            Log.Error("Error in GetURL: " + e.Message);
            throw;
        }
    }
    
    /*public static async Task<string> GetAssistantResponse(string content)
    {
        Client client = new Client();
        Assistant assistant = await Assistant.Create(client, "John", "gpt-4-1106-preview", "This is a test assistant");
        Thread thread = await Thread.Create(assistant);
        Message message = new Message {Content = content};
        await thread.AddMessage(message);
        Run run = await thread.StartRun();
        
        while (run.Status != "completed")
        {
            await Task.Delay(1000);
            run = await run.Retrieve();
        }
        
        List<Message> threadMessages = await 
    }*/
}