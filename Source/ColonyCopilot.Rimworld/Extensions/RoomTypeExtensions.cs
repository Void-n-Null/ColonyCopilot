using ColonyCopilot.Rimworld.ContextEnums;
using ColonyCopilot.Rimworld.DataStructures;
using ColonyCopilot.Rimworld.DataStructures.RoomPresets;

namespace ColonyCopilot.Rimworld.Extensions;

public static class RoomTypeExtensions
{
    public static RoomPreset GetPreset(this RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.DiningRoom:
                return new DiningRoom();
                break;
            case RoomType.Kitchen:
                return new Kitchen();
                break;
            case RoomType.Bedroom:
                return new Bedroom();
                break;
            /*case RoomType.Hospital:
                break;
            case RoomType.Prison:
                break;
            case RoomType.Research:
                break;*/
            case RoomType.Storage:
                return new Storage();
                break;
            /*case RoomType.Recreation:
                break;*/
            default:
                throw new ArgumentOutOfRangeException(nameof(roomType), roomType, null);
        }

        return null;
    }
    
}