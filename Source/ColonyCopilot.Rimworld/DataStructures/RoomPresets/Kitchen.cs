using RimWorld;
using Verse;

namespace ColonyCopilot.Rimworld.DataStructures.RoomPresets;

public class Kitchen : RoomPreset
{
    public override ZoneFactory.ZoneType ZoneType => ZoneFactory.ZoneType.None;
    public override List<BuildableDef> RequiredThings => new List<BuildableDef>();
    public override List<List<ThingDef>> RequireOneOfSelection
    {
        get
        {
            List<ThingDef> stoves = new List<ThingDef>
            {
                ThingDefOf.Campfire,
                ThingDef.Named("ElectricStove"),
                ThingDef.Named("FueledStove")
            };
            
            return new List<List<ThingDef>>
            {
                stoves
            };
        }
    }

    public override int RecomendedSize => 6;
    public override int DoorCount => 1;
    public override bool NeedsWalls => true;
    public override bool NeedsFloor => true;
}