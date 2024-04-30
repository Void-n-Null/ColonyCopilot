namespace ColonyCopilot.Rimworld.Extensions;

public static class StringExtensions
{
    public static int LevenDistance(this string source1, string source2)
    {
        return LevenshteinDistance.Calculate(source1, source2);
    }
}