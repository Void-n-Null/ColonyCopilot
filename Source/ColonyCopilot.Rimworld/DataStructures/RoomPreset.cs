using RimWorld;
using Verse;
using Exception = System.Exception;

namespace ColonyCopilot.Rimworld.DataStructures;

public abstract class RoomPreset
{
    public abstract ZoneFactory.ZoneType ZoneType { get;}
    public abstract List<BuildableDef> RequiredThings { get;}
    public abstract List<List<ThingDef>> RequireOneOfSelection { get;}
    public abstract int RecomendedSize { get; }
    public abstract int DoorCount { get; }
    
    public abstract bool NeedsWalls { get; }
    
    public abstract bool NeedsFloor { get; }
    
    public bool HasSpace(CCPRoom room)
    {
        if (RequiredThings.Count == 0 && RequireOneOfSelection.Count == 0)
        {
            return true;
        }

        int tilesAvailable = room.CellsThatCanBeConstructedOn().Count;
        return tilesAvailable >= MinimumCellsRequired;
    }

    public int MinimumCellsRequired
    {
        get
        {
            int tilesNeededForRequiredThings = 0;
            foreach (var thing in RequiredThings)
            {
                tilesNeededForRequiredThings += thing.Size.x * thing.Size.z;
            }
            int largestPotentialTilesNeeded = 0;
            //Find the biggest combination of things
            foreach (var selection in RequireOneOfSelection)
            {
                //Find the biggest thing in each list
                var biggestThing = selection.MaxBy(t => t.size.x * t.size.z);
                largestPotentialTilesNeeded += biggestThing.size.x * biggestThing.size.z;
            }
        
            return tilesNeededForRequiredThings + largestPotentialTilesNeeded;
        }
    }
}