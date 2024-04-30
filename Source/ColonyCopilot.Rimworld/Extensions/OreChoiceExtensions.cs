using ColonyCopilot.Rimworld.ContextEnums;
using RimWorld;
using Verse;

namespace ColonyCopilot.Rimworld.Extensions;

public static class OreChoiceExtensions
{
    public static ThingDef GetThingDef(this OreChoice choice)
    {
        return choice switch
        {
            OreChoice.Steel => ThingDefOf.MineableSteel,
            OreChoice.Plasteel => ThingDef.Named("MineablePlasteel"),
            OreChoice.Gold => ThingDefOf.MineableGold,
            OreChoice.Silver => ThingDef.Named("MineableSilver"),
            OreChoice.Jade => ThingDef.Named("MineableJade"),
            OreChoice.Uranium => ThingDef.Named("MineableUranium"),
            OreChoice.Component => ThingDefOf.MineableComponentsIndustrial,
            _ => throw new ArgumentOutOfRangeException(nameof(choice), choice, null)
        };
    }
}