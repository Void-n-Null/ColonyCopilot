using RimWorld;
using Verse;

namespace ColonyCopilot.Rimworld.DataStructures.RoomPresets;

public class Storage : RoomPreset
{
    public override ZoneFactory.ZoneType ZoneType => ZoneFactory.ZoneType.Storage;
    public override List<BuildableDef> RequiredThings => new List<BuildableDef>();
    public override List<List<ThingDef>> RequireOneOfSelection => new List<List<ThingDef>>();
    public override int RecomendedSize => 11;
    public override int DoorCount => 0;
    public override bool NeedsWalls => false;
    public override bool NeedsFloor => false;
}