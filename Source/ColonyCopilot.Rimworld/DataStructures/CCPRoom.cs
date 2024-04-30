using ColonyCopilot.Rimworld.ContextEnums;
using ColonyCopilot.Rimworld.Extensions;
using RimWorld;
using Verse;

namespace ColonyCopilot.Rimworld.DataStructures;

public class CCPRoom
{
    public string Name { get; set; }
    public CellRect CellRect { get; set; }
    public List<IntVec3> WallCells { get; set; }
    public List<IntVec3> DoorCells { get; set; }
    public RoomType RoomType { get; set; }
    
    public RoomPreset RoomPreset => RoomType.GetPreset();
    
    public bool HasWalls { get; set; }
    
    public bool Placed { get; set; } = false;
    
    public CCPRoom(CellRect cellRect, RoomType roomType = RoomType.Bedroom)
    {
        CellRect = cellRect;
        RoomType = roomType;
        SetExteriorTiles();
    }

    private void SetExteriorTiles()
    {
        WallCells = RoomPreset.NeedsWalls ? CellRect.EdgeCells.ToList() : new List<IntVec3>();
        DoorCells = RoomPreset.NeedsWalls ? GetRandomDoors(RoomPreset.DoorCount, CellRect) : new List<IntVec3>();
    }

    public List<IntVec3> CellsThatCanBeConstructedOn()
    {
        var map = CcpGameManager.Instance.Game.CurrentMap;
        //Any cell that is open on the map
        var openCells = CellRect.Cells.Where(c => GenConstruct.CanPlaceBlueprintAt(ThingDefOf.Wall, c, Rot4.North, map));
        return openCells.ToList();
    }
    
    public CCPRoom(IntVec3 startPosition, IntVec3 endPosition,RoomType roomType = RoomType.Bedroom)
    {
        CellRect = CellRect.FromLimits(startPosition, endPosition);
        RoomType = roomType;
        SetExteriorTiles();
    }
    
    private void Initialize()
    {
        CcpGameManager.Instance.Rooms.Add(this);
    }

    private const int MaxDoorSearchAttempts = 100;
    private static List<IntVec3> GetRandomDoors(int count, CellRect rect)
    {
        var doors = new List<IntVec3>();
        int doorSearchAttempts = 0;
        while (doors.Count < count && doorSearchAttempts < MaxDoorSearchAttempts)
        {
            doorSearchAttempts++;
            var doorCell = rect.EdgeCells.RandomElement();
            if (rect.IsCorner(doorCell))
                continue;
            if (doors.Any(d => d.DistanceTo(doorCell) < 2))
                continue;
            doors.Add(doorCell);
        }
        return doors;
    }
    
    /// <summary>
    /// Checks if the room intersects with another room.
    /// This specific implimentation checks if the rooms share any interior cells.
    /// Edge cells are not considered intersections.
    /// </summary>
    /// <param name="otherRoom"> The room to check for intersections with. </param>
    /// <returns> True if the rooms intersect, false otherwise. </returns>
    private bool IntersectsRoom(CCPRoom otherRoom)
    {
        IEnumerable<IntVec3> edgeIntersctions = CellRect.EdgeCells.Intersect(otherRoom.CellRect.EdgeCells);
        IEnumerable<IntVec3> allIntersections = CellRect.Cells.Intersect(otherRoom.CellRect.Cells);
        //Remove the edge intersections from the all intersections
        return allIntersections.Except(edgeIntersctions).Any();
    }
    
    

    /// <summary>
    /// Checks if the room is valid by the following criteria:
    /// 1. The walls can be placed.
    /// 2. The doors can be placed.
    /// 3. The room is within the map bounds.
    /// </summary>
    /// <param name="map"></param>
    /// <returns> True if the room is valid, false otherwise. </returns>
    public bool IsValid(Map map)
    {
        bool allDoorsCanBePlaced = Util.IsValidPlacement(DoorCells, Rot4.North, ThingDefOf.Door, map);
        bool allWallsCanBePlaced = Util.IsValidPlacement(WallCells, Rot4.North, ThingDefOf.Wall, map);
        bool roomIsInBounds = CellRect.InBounds(map);
        bool intersectsWithAnyRoom = CcpGameManager.Instance.Rooms.Any(r => r != this && IntersectsRoom(r));
        return allDoorsCanBePlaced && allWallsCanBePlaced && roomIsInBounds && !intersectsWithAnyRoom;
    }
    
    public void PlaceRoom(Map map, ThingDef wallMaterial, TerrainDef? floorMaterial = null)
    {
        if (!IsValid(map))
        {
            throw new Exception("Room is not valid to be placed");
        }

        if (RoomPreset.NeedsFloor)
        {
            var terrainDesignator = new Designator_Build(floorMaterial);
            foreach (var cell in CellsThatCanBeConstructedOn())
            {
                terrainDesignator.DesignateSingleCell(cell);
            }
            
        }
        if (RoomPreset.NeedsWalls)
        {
            foreach (var cell in WallCells)
            {
                Util.GenerateBlueprint(ThingDefOf.Wall, wallMaterial, cell, Rot4.North, map);
            }
            foreach (var cell in DoorCells)
            {
                Util.GenerateBlueprint(ThingDefOf.Door, wallMaterial, cell, Rot4.North, map);
            }
        }

        

        Initialize();
        Placed = true;
    }
    
    private const int MaxSearchAttempts = 100;
    public void ApplyPreset()
    {
        var preset = RoomPreset;
        
        if (!preset.HasSpace(this))
            throw new Exception("Room does not have enough space for this preset");

        
        try
        {
            CreateBlueprintsForPreset(preset);
        }
        catch (Exception e)
        {
            throw new Exception("Error creating buildable blueprints: " + e);
        }
        
            try
            {
                ApplyZone(preset.ZoneType);
            }
            catch (Exception e)
            {
                throw new Exception("Error applying zone to room: " + e);
            }
        
    }

    //TODO: Refactor this to be more efficient
    private void CreateBlueprintsForPreset(RoomPreset preset)
    {
        var buildList = new List<BuildableDef>();
        foreach (var thing in preset.RequiredThings)
        {
            buildList.Add(thing);
        }
        
        foreach (var selection in preset.RequireOneOfSelection)
        {
            var thing = selection.RandomElement();
            buildList.Add(thing);
        }
        int attempts = 0;
        //Create a list that will slowly be reduced as the room is built
        var selectableCells = new List<IntVec3>(CellsThatCanBeConstructedOn());
        while (buildList.Count > 0 && attempts < MaxSearchAttempts)
        {
            attempts++;
            var thing = buildList.RandomElement();
            var position = selectableCells.RandomElement();
            foreach (var rot in Rot4.AllRotations)
            {
                try
                {

                    Util.GenerateBlueprint(thing, ThingDefOf.WoodLog, position, rot,
                        CcpGameManager.Instance.Game.CurrentMap);
                }
                catch (Exception e)
                {
                    continue;
                }

                //All of the cells that the building takes up are no longer selectable
                foreach (var cell in GenAdj.OccupiedRect(position, rot, thing.Size))
                {
                    selectableCells.Remove(cell);
                }
                attempts = 0;
                buildList.Remove(thing);
                break;
            }
        }
    }

    public void ApplyZone(ZoneFactory.ZoneType zoneType)
    {
        if (zoneType == ZoneFactory.ZoneType.None)
            return;
        var zone = ZoneFactory.CreateZone(zoneType);
        var zoneManager = CcpGameManager.Instance.Game.CurrentMap.zoneManager;
        zoneManager.RegisterZone(zone);
        foreach (var cell in CellRect.Cells)
        {
            //TODO: Handle this more gracefully lol
            try
            {
                zone.AddCell(cell);
            } catch (Exception e)
            {
                continue;
                //Ah shoot, must be a wall or something
            }
            
        }
    }
}