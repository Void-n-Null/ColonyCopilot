using System.Runtime.InteropServices;
using ColonyCopilot.OpenAI;
using ColonyCopilot.OpenAI.Assistants;
using ColonyCopilot.OpenAI.Functions;
using Verse;
using Thread = ColonyCopilot.OpenAI.Assistants.Thread;
using Message = ColonyCopilot.OpenAI.Assistants.Message;

namespace ColonyCopilot.Rimworld;

public class ColonyAgent
{
    private string _instructions;
    private string _model;
    private Assistant _assistant;
    private Thread _mainThread;
    private List<AIFunction> _functions;
    private SpeechGenerator _speechGenerator;
    public bool initialized { get; private set; } = false;
    public Client Client => Assistant.Client;
    public bool IsRunning => Thread.IsRunning;
    
    private bool _autonomouslyRunning = false;
    public Assistant Assistant
    {
        get
        {
            if (_assistant == null || !initialized)
            {
                /*throw new NullReferenceException("Attempted to get assistant before initialization");*/
            }
            return _assistant;
        }
    }

    public Thread Thread
    {
        get
        {
            if (_mainThread == null || !initialized)
            {
                /*throw new NullReferenceException("Attempted to get thread before initialization");*/
            }
            return _mainThread;
        }
    }
    
    public static async Task<ColonyAgent> CreateInstance(Client client, string instructions, string model, List<AIFunction> functions)
    {
        var instance = new ColonyAgent();
        instance._instructions = instructions;
        instance._model = model;
        instance._functions = functions;
        await instance.Initialize(client);
        return instance;
    }

    public async Task Initialize(Client client)
    {
        client.OnModelChanged += OnModelChanged;
        //This is extra context about rimworld that the AI can use to generate responses
        //This is loaded from a text file in the resources
        var context = "";
        try
        {
            context = ResourceLoader.LoadTxtFile("RimworldContext");
        } catch (Exception e)
        {
            CLog.Error("Error loading contextFile from resources: " + e);
            throw;
        }

        var finalInstructions = $"{context} \n {_instructions}";
        
        try
        {
            _assistant = await Assistant.Create(client, $"Colony Copilot {Guid.NewGuid()}", _model, finalInstructions, _functions);
        } catch (Exception e)
        {
            CLog.Error("Error initializing assistant:  " + e);
            throw;
        }
        
        try
        {   
            _mainThread = await Thread.Create(_assistant, 20f);
            _mainThread.RanToolCall += LogToolCall;
        } catch (Exception e)
        {
            CLog.Error("Error initializing main thread: " + e);
            throw;
        }
        
        try
        {
            _speechGenerator = SpeechGenerator.Create(client);
        } catch (Exception e)
        {
            CLog.Error("Error initializing speech generator: " + e);
            throw;
        }
        
        initialized = true;
    }
    
    public void OnModelChanged(string modelID)
    {
        //Recreate the assistant with the new model
    }

    private async Task SendMessage(Message message)
    {
        try
        {
            await _mainThread.AddMessage(message);
            CLog.Message($"{message.Role.ToString()}: {message.Content}");
        } catch (Exception e)
        {
            CLog.Error("Error sending message: " + e);
            throw;
        }
    }
    
    public void LogToolCall(string toolCall)
    {
        CLog.Message($"Agent Ran Tool: {toolCall}");
    }
    
    public async Task SendUserMessage(string content)
    {
        await SendMessage(Message.User(content));
    }
    
    public async Task SendAssistantMessage(string content)
    {
        await SendMessage(Message.Assistant(content));
    }
    
    public async Task<string> GetResponse(string instruction)
    {
        try
        {
            var result = await _mainThread.GetResponse(instruction);
            CLog.Message("Response: " + result.Content);
            CLog.Message("Tools used: " + result.ToolsUsedCount);
            return result.Content;
        } catch (Exception e)
        {
            CLog.Error("Error getting response: " + e);
            throw;
        }

    }
    
    public async Task GetAndSpeakResponse(string instruction = "")
    {
        CLog.Message("Getting response...");
        var response = await GetResponse(instruction);
        CLog.Message("Response: " + response);
        await Speak(response);
    }

    public async Task Speak(string content, Voice voice = Voice.Nova)
    {
        try
        {
            await _speechGenerator.Speak( content, voice);
        } catch (Exception e)
        {
            CLog.Error("Error speaking: " + e);
        }   
        
    }

    public async Task Destroy()
    {
        //Clear all relevant data
        Assistant.Delete(Client, Assistant.Id);
    }
    
}