using UnityEngine;
using System;
using ColonyCopilot.OpenAI.Functions;
using ColonyCopilot.Rimworld.ContextEnums;
using ColonyCopilot.Rimworld.DataStructures;
using ColonyCopilot.Rimworld.Extensions;
using RimWorld;
using Unity.Jobs.LowLevel.Unsafe;
using Verse;
using Verse.AI;

namespace ColonyCopilot.Rimworld;

public static class PawnFunctions
{
    private static Game game => CcpGameManager.Instance.Game;
    
    [AIFunction("CreatePerson", "Creates a person.")]
    public static string CreateColonist(
        [AIParameter("The name of the person")] string name,
        [AIParameter("The gender of the person. Should reflect the name")] Gender gender)
    {
        var pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPlayer);
        pawn.Name = new NameSingle(name);
        pawn.skills.skills.ForEach(skill => skill.Level = 20);
        GenPlace.TryPlaceThing(pawn, game.CurrentMap.Center, game.CurrentMap, ThingPlaceMode.Near);
        return $"[SUCCESS] Created {gender} person named {name}";
    }
    
    [AIFunction("RequestMine","Request that your pawns mine a specific ore.")]
    public static string RequestMineMaterial([AIParameter("The type of ore you want to mine")] OreChoice ore)
    {
        var oreTiles = CcpGameManager.Instance.Context.Tracker.GetOre(ore);
        if (oreTiles.Count == 0)
        {
            return $"[FAIL] No {ore} ore found on the map.";
        }
        var sanitized_ores = new List<Thing>();
        foreach (var oreTile in oreTiles)
        {
            //If fogged? Skip
            if (oreTile.Fogged())
            {
                continue;
            }
            sanitized_ores.Add(oreTile);
        }

        Thing? closestOre = Util.FindClosestThingToAnyColonist(sanitized_ores);
        if (closestOre != null)
            DesignationQueue.AddDesignation(typeof(Designator_MineVein), new List<IntVec3> { closestOre.Position });
        return $"[SUCCESS] Requested mining of the nearest vein of {ore} ore.";
    }
    

    [AIFunction("BuildRoom", "Creates a Blueprint on the map for a room. Colonists will use this blueprint to construct a room.")]
    public static string BuildRoom(
        [AIParameter("The name of the room you want to build. This should reflect what the room will be used for, or what it's future type will be")] string name,
        [AIParameter("The size of the room in tiles.")] int size,
        [AIParameter("The material you want to build the room out of.")] ResourceChoice material,
        [AIParameter("The type of room you want to build.")] RoomType roomType
        )
    {
        
        var preset = roomType.GetPreset();
        bool requiresMaterials = preset.NeedsWalls || preset.NeedsFloor;
        if (requiresMaterials && roomType != RoomType.Storage)
        {
            var count = game.CurrentMap.resourceCounter.GetCount(material.GetThingDef());
            bool hasStorage = CcpGameManager.Instance.Rooms.Exists(room => room.RoomType == RoomType.Storage);
            if (!hasStorage)
            {
                return "[FAIL] You need a storage room to store materials for building.";
            }
            if (count <= 0)
            {
                return $"[FAIL] You do not have any {material} to build with.";
            }
        }
        
        var reccomendedSize = $"The recommended size for a {roomType.ToString()} is {preset.RecomendedSize}.";
        if (size <= 0)
        {
            return $"[FAIL] {size} is too small of a room size. Room size must be greater than 0." + reccomendedSize;
        }

        if (size > 11)
        {
            return $"[FAIL] {size} is too large of a room size. The maximum room size is 11." + reccomendedSize;
        }

        
        if (preset.MinimumCellsRequired > size * size)
        {
            float requiredSize = Mathf.Sqrt(preset.MinimumCellsRequired) + 1;
            int requiredSizeInt = Mathf.CeilToInt(requiredSize);
            return $"[FAIL] The room size of {size} is too small for the room type. The minimum size for a {roomType.ToString()} is {requiredSizeInt}.";
        }

        bool roomPlaced;
        try
        {
            roomPlaced = AttemptCreatePlaceAndApplyRoom(name, size, material, roomType);
        } catch (Exception e)
        {
            return $"[FAIL] Error placing room: {e.Message}";
        }

        
        if (roomPlaced)
        {
            return $"[SUCCESS] Created room {name} of size {size} and material {material}. It is a room of type {roomType}.";
        }
        return $"[FAIL] Could not place room {name}. No valid locations for a room of size {size} were found on the map. Try a smaller size.";
    }

    private const int MaxBuildRoomAttempts = 100;
    private static bool AttemptCreatePlaceAndApplyRoom(string name, int size, ResourceChoice material, RoomType roomType)
    {
        for (int i = 0; i < MaxBuildRoomAttempts; i++)
        {
            var randomPosition = CellFinder.RandomNotEdgeCell(size+1, game.CurrentMap);
            foreach (var cornerPos in CellUtil.CornersFromSize(randomPosition, size))
            {
                CCPRoom room;
                try
                {
                    //Create
                    room = new CCPRoom(randomPosition, cornerPos)
                    {
                        RoomType = roomType
                    };
                } catch (Exception e)
                {
                    throw new Exception($"Error creating room to be placed: {e}");
                }
                try
                {
                    //Validate
                    if (!room.IsValid(game.CurrentMap)) continue; 
                } catch (Exception e)
                {
                    throw new Exception($"Error checking if room is valid: {e}");
                }
                
                
                try
                {
                    //Place
                    room.Name = name;
                    room.PlaceRoom(game.CurrentMap, material.GetThingDef(), material.GetTerrainDef());
                } catch (Exception e)
                {
                    throw new Exception($"Error using try place room: {e}");
                }
                
                try
                {
                    //Apply
                    room.ApplyPreset();
                }
                catch (Exception e)
                {
                    throw new Exception($"Error applying preset to room: {e}");
                }
                return true;
            }
        }

        return false;
    }

    [AIFunction("RequestChopTree", "Request that your pawns chop down a certain number of trees.")]
    public static string RequestChopTree([AIParameter("The number of trees you want to chop down. Usually 5 is enough")] int count)
    {
        var trees = CcpGameManager.Instance.Context.Tracker.Trees;
        if (trees.Count == 0)
        {
            return "[FAIL] No trees found on the map.";
        }
        if (count > trees.Count)
        {
            return $"[SUCCESS] Since there are only {trees.Count} trees  instead of {count}, we will chop down all of them.";
        }
        var targetedTrees = Util.ClosestThingsToAnyColonist(trees, count);
        var treeCells = targetedTrees.ConvertAll(tree => tree.Position);
        DesignationQueue.AddDesignation(typeof(Designator_PlantsHarvestWood), treeCells);
        return $"[SUCCESS] Requested chopping down of {count} trees.";
    }
}