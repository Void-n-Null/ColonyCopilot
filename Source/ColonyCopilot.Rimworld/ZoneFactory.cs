using RimWorld;
using Verse;
using Zone = Verse.Zone;

namespace ColonyCopilot.Rimworld;

public class ZoneFactory
{
    public StorageSettings StorageSettings { get; set; } = new StorageSettings();
    
    public enum ZoneType
    {
        Storage,
        None
    }
    
    public static Zone? CreateZone(ZoneType zoneType)
    {
        var zoneManager = CcpGameManager.Instance.Game.CurrentMap.zoneManager;
        return zoneType switch
        {
            ZoneType.Storage => new Zone_Stockpile(StorageSettingsPreset.DefaultStockpile, zoneManager),
            _ => null
        };
    }
}