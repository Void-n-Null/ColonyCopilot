using Verse;

namespace ColonyCopilot.Rimworld;

public class ColonyCopilotModSettings : ModSettings
{
    public bool Enabled = true;
    public string APIKey = "";
    public string Model = "gpt-4-turbo";
    
    public override void ExposeData()
    {
        Scribe_Values.Look(ref Enabled, "Enabled", true);
        Scribe_Values.Look(ref APIKey, "APIKey", "");
        Scribe_Values.Look(ref Model, "Model", "gpt-4-turbo");
    }
}