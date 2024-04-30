namespace ColonyCopilot.Rimworld;

public static class ResourceLoader
{
    public static string LoadTxtFile(string fileName)
    {
        // Load the file from the resources
        string resourceName = "ColonyCopilot.Rimworld.Resources." + fileName + ".txt";
        var assembly = typeof(ResourceLoader).Assembly;
        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                throw new Exception($"Resource {resourceName} not found.");
            }
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}