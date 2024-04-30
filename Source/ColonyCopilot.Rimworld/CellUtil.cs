using Verse;

namespace ColonyCopilot.Rimworld;

public static class CellUtil
{
    public static IntVec3[] CornersFromSize(IntVec3 initialCell, int size)
    {
        IntVec3 posXposZ = initialCell + new IntVec3(size, 0, size);
        IntVec3 posXnegZ = initialCell + new IntVec3(size, 0, -size);
        IntVec3 negXposZ = initialCell + new IntVec3(-size, 0, size);
        IntVec3 negXnegZ = initialCell + new IntVec3(-size, 0, -size);
        IntVec3[] potentialCornerPositions = {posXposZ, posXnegZ, negXposZ, negXnegZ};
        return potentialCornerPositions;
    }
}