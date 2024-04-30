using ColonyCopilot.OpenAI;
using ColonyCopilot.OpenAI.Functions;
using ColonyCopilot.Rimworld.ContextEnums;
using ColonyCopilot.Rimworld.DataStructures;
using Verse;

namespace ColonyCopilot.Rimworld;

/// <summary>
/// The main game manager for the Colony Copilot Rimworld mod.
/// Contains the state of the mod.
/// </summary>
public class CcpGameManager : GameComponent
{
    /// <summary>
    /// Singleton instance of the game manager.
    /// </summary>
    public static CcpGameManager Instance = null!;

    /// <summary>
    /// The game object.
    /// </summary>
    public Game Game { get; }
    
    /// <summary>
    /// The list of functions that the AI agent can use.
    /// </summary>
    public List<AIFunction> Functions { get; private set; }
    
    /// <summary>
    /// The AI agent that will be used to generate responses.
    /// </summary>
    public ColonyAgent Agent { get; private set; } = null!;

    /// <summary>
    /// The context of the colony.
    /// </summary>
    public ColonyContext Context { get; private set; }
    
    /// <summary>
    /// The list of rooms in the colony.
    /// </summary>
    public List<CCPRoom> Rooms { get; set; }

    public CcpGameManager(Game game)
    {
        Game = game ?? throw new ArgumentNullException(nameof(game));
        Instance = this;
        Functions = new List<AIFunction>();
        Rooms = new List<CCPRoom>();
        try
        {
            Functions = FunctionManager.FindFunctions();
        }
        catch (Exception e)
        {
            CLog.Error("Error Loading Functions: " + e);
            return;
        }

        CLog.Message($"Found {Functions.Count} functions");
        foreach (var function in Functions)
        {
            CLog.Message($"Function: {function.Name} - {function.Description}");
            CLog.Message($"Schema: {function}");
        }
    }

    public override async void StartedNewGame()
    {
        CLog.Message("Colony Copilot Loaded");
        Context = new ColonyContext(Game);
        CLog.Message("Initial Colony Context");
        CLog.Message(Context.ToString());
        await InitializeAgent();
        CLog.Message("Colony Agent Successfully Initialized");


        await TestStuff();
    }

    public void UpdateContext()
    {
        Context.Update(Game.CurrentMap);
    }

    public async Task TestStuff()
    {
        foreach (var roomType in Enum.GetValues(typeof(RoomType)))
        {
            var room = PawnFunctions.BuildRoom("Test Room", 6, ResourceChoice.Wood, (RoomType)roomType);
            CLog.Message(room);
        }
        CLog.Message("Test Complete. Room count: " + Rooms.Count);
        
    }

    public override void GameComponentTick()
    {
        base.GameComponentTick();
        DesignationQueue.ExecuteDesignations();
    }

    public static void CallAgent(string message)
    {
        if (string.IsNullOrEmpty(message.Trim()))
        {
            return;
        }
        Task.Run(() => Instance.TalkToAgent(message));
    }

    private async Task TalkToAgent(string message)
    {
        await Agent.SendUserMessage(message);
        await Agent.GetAndSpeakResponse();
    }

    public async Task AgentAction()
    {
        await Agent.GetAndSpeakResponse(
            $"Here is some information about your colony. Use it to help you make a decision:\n {Context}");
    }


    private async Task InitializeAgent()
    {
        var instructions = "You are a Rimworld Playing Agent. You will try to succeed at rimworld. The user is not going to help you. You must do everything yourself. You must survive. You must thrive. You must build a colony";
        try
        {
            Agent = await ColonyAgent.CreateInstance(ColonyCopilotMod.Client, instructions, "gpt-3.5-turbo-16k", Functions);
        }
        catch (Exception e)
        {
            CLog.Error("Error initializing agent: " + e);
            throw;
        }
    }
}