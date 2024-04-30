using RimWorld;
using Verse;

namespace ColonyCopilot.Rimworld.DataStructures.RoomPresets;

public class Bedroom : RoomPreset
{
    public override ZoneFactory.ZoneType ZoneType => ZoneFactory.ZoneType.None;

    public override List<BuildableDef> RequiredThings => new List<BuildableDef>
    {
        ThingDefOf.Bed
    };
    public override List<List<ThingDef>> RequireOneOfSelection => new List<List<ThingDef>>();
    public override int RecomendedSize => 6;
    public override int DoorCount => 1;
    public override bool NeedsWalls => true;
    public override bool NeedsFloor => true;
}