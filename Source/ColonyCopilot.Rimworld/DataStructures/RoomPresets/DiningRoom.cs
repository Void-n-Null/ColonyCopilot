using RimWorld;
using Verse;

namespace ColonyCopilot.Rimworld.DataStructures.RoomPresets;

public class DiningRoom : RoomPreset
{
    public override ZoneFactory.ZoneType ZoneType => ZoneFactory.ZoneType.None;
    public override List<BuildableDef> RequiredThings => new List<BuildableDef>
    {
        ThingDefOf.DiningChair,
        ThingDefOf.DiningChair,
    };
    public override List<List<ThingDef>> RequireOneOfSelection
    {
        get
        {
            var tables = new List<ThingDef>
            {
                ThingDefOf.Table1x2c,
                ThingDefOf.Table2x2c,
            };
            return new List<List<ThingDef>>
            {
                tables
            };
        }
    }

    public override int RecomendedSize => 8;
    public override int DoorCount => 1;
    public override bool NeedsWalls => true;
    public override bool NeedsFloor => true;
}