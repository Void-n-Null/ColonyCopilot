namespace ColonyCopilot.Rimworld.Extensions;

public static class EnumExtensions
{
    public static IEnumerable<T> GetValues<T>(this Enum e)
    {
        return Enum.GetValues(e.GetType()).Cast<T>();
    }
}