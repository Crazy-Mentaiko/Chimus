using System.Reflection;

namespace ChimusBot.Utils;

public static class EmbedResourceUtil
{
    private static readonly Assembly _selfAssembly = Assembly.GetExecutingAssembly();

    public static Stream? GetStream(string name)
    {
        if (name.Contains('/'))
            name = name.Replace('/', '.');

        var foundName = _selfAssembly.GetManifestResourceNames()
            .FirstOrDefault(resName => resName.EndsWith(name, StringComparison.OrdinalIgnoreCase));
        
        return string.IsNullOrEmpty(foundName)
            ? null
            : _selfAssembly.GetManifestResourceStream(foundName);
    }

    public static string GetFileName(string name) => name.Contains('/') ? Path.GetFileName(name) : name;
}
