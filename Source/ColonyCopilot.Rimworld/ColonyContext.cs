using System.Text;
using RimWorld;
using Verse;

namespace ColonyCopilot.Rimworld;

/// <summary>
/// A storage object to hold the detailed state of the colony.
/// A very important class in the flow of Colony Copilot.
/// This class is used to generate a formatted string of the current state of the colony.
/// This string is then sent to the AI Agent for processing.
/// </summary>
public class ColonyContext
{
    public Tracker? Tracker { get; set; } = null;
    public List<Pawn>? Colonists { get; set; } = null;
    public ResourceCounter? ResourceCounter { get; set; } = null;
    private Map _map = null!;
    
    
    public ColonyContext(Game game)
    {
        Update(game.CurrentMap);
    }
    
    public void Update(Map map)
    {
        _map = map;
        try
        {
            if (Tracker == null)
            {
                Tracker = new Tracker(map);
            } else
            {
                Tracker.Update(map);
            }
            var allPawns = map.mapPawns.AllPawns ?? throw new Exception("All Pawns list is null");
            Colonists = allPawns.Where(pawn => pawn.IsColonist).ToList();
            ResourceCounter = map.resourceCounter ?? throw new Exception("Resource Counter is null");
            
        } catch (Exception e)
        {
            CLog.Error("Error Setting Colony Context: " + e);
        }
    }
    
    
    /// <summary>
    /// The crux of the colony context. Convert the data into a formatted string.
    /// </summary>
    /// <returns> A formatted string of the current state of the colony.</returns>
    public override string ToString()
    {
        //Create a string builder. It's gonna be a long ride.
        var sb = new StringBuilder();
        
        
        AddColonistOverview(sb);
        AddIndividuals(sb);
        AddMineables(sb);
        AddResources(sb);
        
        return sb.ToString();
    }
    
    private void AddColonyOverview(StringBuilder sb)
    {
        var biome = _map.Biome.defName;
        var season = GenLocalDate.Season(_map.Tile).ToString();
        var temperature = _map.mapTemperature.OutdoorTemp;
        var time = GenLocalDate.HourFloat(_map);
        sb.Append("[Colony Overview]\n");
        sb.Append($"Biome: {biome}\n");
        sb.Append($"Season: {season}\n");
        sb.Append($"Temperature: {temperature}C\n");
        sb.Append($"Time: {time}\n");
    }
    
    private void AddColonistOverview(StringBuilder sb)
    {
        var totalColonistsInColony = Colonists.Count;
        if (totalColonistsInColony == 0)
            return;
        //Append colonists data.
        sb.Append("[Colonists]\n");
        sb.Append($"Total Colonists: {totalColonistsInColony}\n");
    }

    private void AddIndividuals(StringBuilder sb)
    {
        var count = Colonists.Count;
        if (count == 0)
            return;
        sb.Append("[Individuals]\n");
        for (var i = 0; i < count; i++)
        {
            var pawn = Colonists[i];
            sb.Append($"{i}. {pawn.Name} - {pawn.gender} - {pawn.ageTracker.AgeBiologicalYears}\n");
            sb.Append("- Faction: " + pawn.Faction.Name + "\n");
            sb.Append("- Mood: " + pawn.needs.mood.MoodString + "\n");
            sb.Append("- Health: " + pawn.health.summaryHealth.SummaryHealthPercent * 100 + "% \n");
            sb.Append("- Rest: " + pawn.needs.rest.CurLevelPercentage * 100 + "% \n");
            sb.Append("- Current Activity: " +  pawn.GetJobReport() + "\n");
        }
    }

    private void AddMineables(StringBuilder sb)
    {
        var mineableTracker = Tracker;
        if (mineableTracker.MineableDictionary.Count <= 0)
            return;
        if (mineableTracker.MineableDictionary.All(mineable => mineable.Value.Count <= 0))
            return;
        sb.Append("[Ore found on Map]\n");
        foreach (var mineable in mineableTracker.MineableDictionary)
        {
            if (mineable.Value.Count <= 0)
                continue;
            sb.Append($"{mineable.Key}: {mineable.Value.Count}\n");
        }
    }
    
    private void AddResources(StringBuilder sb)
    {
        if (ResourceCounter.AllCountedAmounts.Count <= 0)
            return;
        if (ResourceCounter.AllCountedAmounts.All(resource => resource.Value <= 0))
            return;
        sb.Append("[Resources In Storage]\n");
        var resources = ResourceCounter.AllCountedAmounts;
        foreach (var resource in resources)
        {
            if (resource.Value <= 0)
                continue;
            sb.Append($"{resource.Key}: {resource.Value}\n");
        }
        
    }
    
    public void AddExistingRooms(StringBuilder sb)
    {
        if (CcpGameManager.Instance.Rooms.Count == 0)
            return;
        sb.Append("[Rooms]\n");
        foreach (var room in CcpGameManager.Instance.Rooms)
        {
            sb.Append($"{room.Name} - Area: {room.CellRect.Area} - Room Type: \n");
        }
    }
}