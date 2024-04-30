using ColonyCopilot.Rimworld.ContextEnums;
using RimWorld;
using Verse;

namespace ColonyCopilot.Rimworld;

public class Tracker 
{
    public Dictionary<OreChoice,List<Thing>> MineableDictionary { get; private set; } = new Dictionary<OreChoice, List<Thing>>();
    public List<Thing> Trees { get; private set; } = new List<Thing>();

    public Tracker(Map map)
    {
        Update(map);
    }

    public void Update(Map Map)
    {
        MineableDictionary = new Dictionary<OreChoice, List<Thing>>
        {
            [OreChoice.Steel] = Map.listerThings.ThingsOfDef(ThingDefOf.MineableSteel),
            [OreChoice.Gold] = Map.listerThings.ThingsOfDef(ThingDefOf.MineableGold),
            [OreChoice.Silver] = Map.listerThings.ThingsOfDef(ThingDef.Named("MineableSilver")),
            [OreChoice.Plasteel] = Map.listerThings.ThingsOfDef(ThingDef.Named("MineablePlasteel")),
            [OreChoice.Jade] = Map.listerThings.ThingsOfDef(ThingDef.Named("MineableJade")),
            [OreChoice.Uranium] = Map.listerThings.ThingsOfDef(ThingDef.Named("MineableUranium")),
            [OreChoice.Component] = Map.listerThings.ThingsOfDef(ThingDefOf.MineableComponentsIndustrial)
        };
        
        Trees = Map.listerThings.ThingsInGroup(ThingRequestGroup.Plant).Where(thing => thing.def.plant.IsTree).ToList();
    }
    
    
    public List<Thing> GetOre(OreChoice choice)
    {
        return MineableDictionary[choice];
    }
}