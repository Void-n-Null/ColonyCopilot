using ColonyCopilot.OpenAI;
using ColonyCopilot.Rimworld.Extensions;
using HarmonyLib;
using Verse;

namespace ColonyCopilot.Rimworld;
public class ColonyCopilotMod : Mod
{
    public ColonyCopilotModSettings settings;
    

    public static Client Client
    {
        get
        {
            if (_currentClient == null || _currentClient.ApiKey != LoadedModManager.GetMod<ColonyCopilotMod>().GetSettings<ColonyCopilotModSettings>().APIKey)
            {
                _currentClient = CreateClient();
            }

            return _currentClient;
        }
    }
    
    private static List<string> _modelIds = new List<string>();
    public static Client _currentClient;
    public ColonyCopilotMod(ModContentPack content) : base(content)
    {
        settings = GetSettings<ColonyCopilotModSettings>();
        var harmony = new Harmony("vnull.colonycopilot");
        harmony.PatchAll();
        Task.Run(GetModels);
    }
    
    private async Task GetModels()
    {
        var models = await Models.Retrieve(Client);
        _modelIds.Clear();
        foreach (var model in models)
        {
            _modelIds.Add(model.id);
        }
    }
    
    public override void DoSettingsWindowContents(UnityEngine.Rect inRect)
    {
        Listing_Standard listingStandard = new Listing_Standard();
        listingStandard.Begin(inRect);
        listingStandard.CheckboxLabeled("Enabled", ref settings.Enabled, "Enable or disable Colony Copilot");
        listingStandard.Gap();
        listingStandard.Label("API Key");
        settings.APIKey = listingStandard.TextEntry(settings.APIKey);
        listingStandard.Gap();
        listingStandard.Label("Model");
        settings.Model = listingStandard.TextEntry(settings.Model);
        if (!_modelIds.Contains(settings.Model.ToLower()))
        {
            var closestMatch = _modelIds.OrderBy(x => x.LevenDistance(settings.Model)).First();
            // Warn user that model is invalid
            listingStandard.Label($"Invalid model. Did you mean {closestMatch}?");
        }
        
        //If model has changed, and is valid, invoke event.
        if (settings.Model != _currentClient.Model && _modelIds.Contains(settings.Model.ToLower()))
        {
            _currentClient.SetModel(settings.Model);
            CLog.Message("Model changed to " + _currentClient.Model);
        }
        
        listingStandard.End();
    }
    
    public static Client CreateClient()
    {
        return new Client(LoadedModManager.GetMod<ColonyCopilotMod>().GetSettings<ColonyCopilotModSettings>().APIKey);
    }
    
    public override string SettingsCategory()
    {
        return "Colony Copilot";
    }
}