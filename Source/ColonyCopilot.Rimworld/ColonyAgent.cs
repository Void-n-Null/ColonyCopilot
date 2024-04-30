using System.Runtime.InteropServices;
using ColonyCopilot.OpenAI;
using ColonyCopilot.OpenAI.Assistants;
using ColonyCopilot.OpenAI.Functions;
using Verse;
using Thread = ColonyCopilot.OpenAI.Assistants.Thread;
using Message = ColonyCopilot.OpenAI.Assistants.Message;

namespace ColonyCopilot.Rimworld;

/// <summary>
/// A class respresenting an AI Agent that will be used to manage the colony.
/// This is the titular 'Colony Copilot' the mod is named after.
/// </summary>
public class ColonyAgent
{
    private const float DefaultTimeout = 20f;

    /// <summary>
    /// The instructions for the agent.
    /// </summary>
    private string? _instructions;
    /// <summary>
    /// The model of the agent.
    /// </summary>
    private string? _model;
    /// <summary>
    /// The assistant of the agent.
    /// </summary>
    private Assistant? _assistant;
    /// <summary>
    /// The main thread of the agent.
    /// </summary>
    private Thread? _mainThread;
    /// <summary>
    /// The functions of the agent.
    /// </summary>
    private List<AIFunction>? _functions;
    /// <summary>
    /// The speech generator of the agent.
    /// </summary>
    private SpeechGenerator? _speechGenerator;
    /// <summary>
    /// Whether the agent is initialized.
    /// </summary>
    public bool initialized { get; private set; } = false;
    /// <summary>
    /// The client of the agent.
    /// </summary>
    public Client Client => Assistant.Client;
    /// <summary>
    /// Whether the agent is running.
    /// </summary>
    public bool IsRunning => Thread.IsRunning;

    /// <summary>
    /// Retrieves the assistant of the agent.
    /// </summary>
    public Assistant Assistant
    {
        get
        {
            if (_assistant == null || !initialized)
            {
                throw new NullReferenceException("Attempted to get assistant before initialization");
            }
            return _assistant;
        }
    }


    /// <summary>
    /// Retrieves the main thread of the agent.
    /// </summary>
    public Thread Thread
    {
        get
        {
            if (_mainThread == null || !initialized)
            {
                throw new NullReferenceException("Attempted to get thread before initialization");
            }
            return _mainThread;
        }
    }

    /// <summary>
    /// Retrieves the speech generator of the agent.
    /// </summary>
    private SpeechGenerator SpeechGenerator
    {
        get
        {
            if (_speechGenerator == null || !initialized)
            {
                throw new NullReferenceException("Attempted to get speech generator before initialization");
            }
            return _speechGenerator;
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ColonyAgent"/> class.
    /// </summary>
    /// <param name="client">The client of the agent.</param>
    /// <param name="instructions">The instructions for the agent.</param>
    /// <param name="model">The model of the agent.</param>
    /// <param name="functions">The functions of the agent.</param>
    /// <returns>The new instance of the <see cref="ColonyAgent"/> class.</returns>
    public static async Task<ColonyAgent> CreateInstance(Client client, string instructions, string model, List<AIFunction> functions)
    {
        var instance = new ColonyAgent();
        instance._instructions = instructions;
        instance._model = model;
        instance._functions = functions;
        await instance.Initialize(client);
        return instance;
    }

    /// <summary>
    /// Initializes the agent.
    /// </summary>
    /// <param name="client">The client of the agent.</param>
    /// <returns>The initialized agent.</returns>
    public async Task Initialize(Client client)
    {
        //This is extra context about rimworld that the AI can use to generate responses
        //This is loaded from a text file in the resources
        var context = "";
        try
        {
            context = ResourceLoader.LoadTxtFile("RimworldContext");
        }
        catch (Exception e)
        {
            CLog.Error("Error loading contextFile from resources: " + e);
            throw;
        }

        var finalInstructions = $"{context} \n {_instructions}";

        try
        {
            _assistant = await Assistant.Create(client, $"Colony Copilot {Guid.NewGuid()}", _model, finalInstructions, _functions);
        }
        catch (Exception e)
        {
            CLog.Error("Error initializing assistant:  " + e);
            throw;
        }

        try
        {
            _mainThread = await Thread.Create(_assistant, DefaultTimeout);
            _mainThread.RanToolCall += LogToolCall;
        }
        catch (Exception e)
        {
            CLog.Error("Error initializing main thread: " + e);
            throw;
        }

        try
        {
            _speechGenerator = SpeechGenerator.Create(client);
        }
        catch (Exception e)
        {
            CLog.Error("Error initializing speech generator: " + e);
            throw;
        }

        initialized = true;
    }

    /// <summary>
    /// Sends a message to the agent.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <returns>The task of sending the message.</returns>
    private async Task SendMessage(Message message)
    {
        try
        {
            await Thread.AddMessage(message);
            CLog.Message($"{message.Role.ToString()}: {message.Content}");
        }
        catch (Exception e)
        {
            CLog.Error("Error sending message: " + e);
            throw;
        }
    }

    /// <summary>
    /// A simple callback for the agent to log tool calls.
    /// </summary>
    /// <param name="toolCall">The tool call to log.</param>
    public void LogToolCall(string toolCall)
    {
        CLog.Message($"Agent Ran Tool: {toolCall}");
    }

    /// <summary>
    /// Sends a message to the agent with the context of the author being the user.
    /// </summary>
    /// <param name="content">The content of the message.</param>
    /// <returns>A task representing the sending of the message.</returns>
    public async Task SendUserMessage(string content)
    {
        await SendMessage(Message.User(content));
    }

    /// <summary>
    /// Sends a message to the agent with the context of the author being the assistant.
    /// </summary>
    /// <param name="content">The content of the message.</param>
    /// <returns>A task representing the sending of the message.</returns>
    public async Task SendAssistantMessage(string content)
    {
        await SendMessage(Message.Assistant(content));
    }

    /// <summary>
    /// Uses the thread of the agent to get a response to the current conversation.
    /// </summary>
    /// <param name="instruction">Extra context given to the agent for this specific response. Does not affect smaller models like GPT-3.5.</param>
    /// <returns>The response to the current conversation.</returns>
    public async Task<string> GetResponse(string instruction)
    {
        try
        {
            var result = await Thread.GetResponse(instruction);
            CLog.Message("Response: " + result.Content);
            CLog.Message("Tools used: " + result.ToolsUsedCount);
            return result.Content;
        }
        catch (Exception e)
        {
            CLog.Error("Error getting response: " + e);
            Thread.CancelRun();
            return "I'm sorry, A function I just ran caused an error. I'm going to stop running it.";
            throw;
        }

    }

    /// <summary>
    /// Gets a response to the current conversation and speaks it.
    /// </summary>
    /// <param name="instruction">Extra context given to the agent for this specific response. Does not affect smaller models like GPT-3.5.</param>
    /// <returns>A task representing the speaking of the response.</returns>
    public async Task GetAndSpeakResponse(string instruction = "")
    {
        CLog.Message("Getting response...");
        var response = await GetResponse(instruction);
        CLog.Message("Response: " + response);
        await Speak(response);
    }

    /// <summary>
    /// Uses the speech generator of the agent to speak the given content.
    /// </summary>
    /// <param name="content">The content to speak.</param>
    /// <param name="voice">The voice to speak the content with.</param>
    /// <returns>A task representing the speaking of the content.</returns>
    public async Task Speak(string content, Voice voice = Voice.Nova)
    {
        try
        {
            await SpeechGenerator.Speak(content, voice);
        }
        catch (Exception e)
        {
            CLog.Error("Error speaking: " + e);
        }

    }

    /// <summary>
    /// Destroys the agent.
    /// Exists to avoid flooding the OpenAI API with Agents, since one is created every time the game starts.
    /// </summary>
    /// <returns>A task representing the destruction of the agent.</returns>
    public async Task Destroy()
    {
        //Clear all relevant data from the OpenAI database
        await Assistant.Delete(Client, Assistant.Id);
        await Thread.Delete(Client, Thread.Id);
    }

}