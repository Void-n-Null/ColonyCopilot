using Verse;

namespace ColonyCopilot.Rimworld;


public static class DesignationQueue
{
    public static readonly Queue<(Type, List<IntVec3>)> Designations = new Queue<(Type, List<IntVec3>)>();
    
    public static void AddDesignation(Type designatorType, List<IntVec3> cells)
    {
        Designations.Enqueue((designatorType, cells));
    }
    
    public static void ExecuteDesignations()
    {
        if (!UnityData.IsInMainThread)
        {
            Log.Error("DesignationQueue.ExecuteDesignations should only be called from the main thread.");
            return;
        }
        
        while (Designations.Count > 0)
        {
            var (designationType, cells) = Designations.Dequeue();
            foreach (var cell in cells)
            {
                var designator = (Designator) Activator.CreateInstance(designationType);
                designator.DesignateSingleCell(cell);
            }
        }
    }
    
}