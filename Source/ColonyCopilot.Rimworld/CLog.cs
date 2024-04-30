using UnityEngine;
using System;
using Verse;

namespace ColonyCopilot.Rimworld;

public static class CLog
{
    private const string Prefix = "[ColonyCopilot] ";
    private const bool MessagesEnabled = true;
    public static void Message(string message, int? tokenCount = null)
    {
        if (!MessagesEnabled) return;
        Log.Message($"{Prefix}{message}");
        if (tokenCount != null)
        {
            Log.Message("Token Count: " + tokenCount);
        }
    }
    
    public static void Warning(string message)
    {
        Log.Warning($"{Prefix}{message}");
    }
    
    public static void Error(string message)
    {
        Log.Error($"{Prefix}{message}");
    }
}