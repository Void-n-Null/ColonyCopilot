using ColonyCopilot.Rimworld.ContextEnums;
using RimWorld;
using Verse;

namespace ColonyCopilot.Rimworld.Extensions;

public static class ResourceChoiceExtensions
{
    public static ThingDef GetThingDef(this ResourceChoice choice)
    {
        return choice switch
        {
            ResourceChoice.Wood => ThingDefOf.WoodLog,
            ResourceChoice.Steel => ThingDefOf.Steel,
            ResourceChoice.Silver => ThingDefOf.Silver,
            ResourceChoice.Sandstone => ThingDefOf.Sandstone,
            _ => throw new ArgumentOutOfRangeException(nameof(choice), choice, null)
        };
    } 
    
    public static ThingDef GetCountOf(this ResourceChoice choice)
    {
        var counts = CcpGameManager.Instance.Game.CurrentMap.resourceCounter.AllCountedAmounts;
        var thingDef = choice.GetThingDef();
        return counts.FirstOrDefault(count => count.Key == thingDef).Key;

    }
    
    public static TerrainDef? GetTerrainDef(this ResourceChoice choice)
    {
        return choice switch
        {
            ResourceChoice.Wood => TerrainDefOf.WoodPlankFloor,
            ResourceChoice.Steel => TerrainDefOf.Concrete,
            ResourceChoice.Silver => TerrainDef.Named("SilverTile"),
            ResourceChoice.Sandstone => TerrainDef.Named("TileSandstone"),
            _ => throw new ArgumentOutOfRangeException(nameof(choice), choice, null)
        };
    }
}