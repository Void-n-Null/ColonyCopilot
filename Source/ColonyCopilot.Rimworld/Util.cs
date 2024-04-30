using ColonyCopilot.Rimworld.ContextEnums;
using ColonyCopilot.Rimworld.Extensions;
using RimWorld;
using Verse;
namespace ColonyCopilot.Rimworld;

public static class Util
{
    public static void GenerateBlueprint(BuildableDef def, ThingDef material, IntVec3 position, Rot4 rotation, Map map)
    {
        var report = GenConstruct.CanPlaceBlueprintAt(def, position, rotation, map);
        if (!report.Accepted)
        {
            throw new Exception("Cannot place blueprint at position: " + report.Reason);
        }
        GenConstruct.PlaceBlueprintForBuild(def, position, map, rotation, Faction.OfPlayer, material);
    }

    public static List<Pawn> Colonists => CcpGameManager.Instance.Game.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer);
    
    /// <summary>
    /// Find the closest thing to any colonist.
    /// </summary>
    /// <param name="things"> The list of things to search through.</param>
    /// <param name="minDistance"> The minimum distance to search for.</param>
    /// <returns> The closest thing to any colonist.</returns>
    public static Thing? FindClosestThingToAnyColonist(List<Thing> things, float minDistance = 3f)
    {
        return ClosestThingsToAnyColonist(things, 1, minDistance).FirstOrDefault();
    }

    /// <summary>
    /// Find the closest things to any colonist.
    /// </summary>
    /// <param name="things"> The list of things to search through.</param>
    /// <param name="returnCount"> The ammount of things to return.</param>
    /// <param name="minDistance"> The minimum distance to search for.</param>
    /// <returns> A list of the closest things to any colonist.</returns>
    public static List<Thing> ClosestThingsToAnyColonist(List<Thing> things, int returnCount, float minDistance = 3f)
    {
        if (things.Count == 0)
            return new List<Thing>();

        var thingDistances = things.Select(thing =>
        {
            var minDistanceToColonist = Colonists.Min(pawn => pawn.Position.DistanceTo(thing.Position));
            return (thing, minDistanceToColonist);
        }).ToList();

        var closestThings = thingDistances
                //Order by distance
            .OrderBy(x => x.minDistanceToColonist)
                //Take the closest X things
            .Take(returnCount)
                //Select the thing
            .Select(x => x.thing)
                //ToList
            .ToList();

        return closestThings;
    }
    public static bool IsValidPlacement(List<IntVec3> cells, Rot4 rot, BuildableDef thingDef, Map map)
    {
        foreach (var cell in cells)
        {
            if (!cell.InBounds(map) || !cell.Walkable(map) || !GenConstruct.CanPlaceBlueprintAt(thingDef, cell, rot, map))
                return false;
        }
        return true;
    }
}