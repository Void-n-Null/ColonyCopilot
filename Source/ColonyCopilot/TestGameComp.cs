using UnityEngine;
using System;
using Verse;
using RimOpenAI;
using UnityEngine.Networking;

namespace ColonyCopilot;

public class TestGameComp : GameComponent
{
    public TestGameComp(Game game)
    {
        LoadWebRequest();
    }
    
    private async void LoadWebRequest()
    {
        var response = await APILayer.GetURL("https://jsonplaceholder.typicode.com/todos/1");
        Log.Message(response);
    }
    
}