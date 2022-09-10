using System.Reflection;
using Discord;

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

    public static FileAttachment? GetAttachment(string name)
    {
        var stream = GetStream(name);
        return new FileAttachment(stream, name.Contains('/') ? Path.GetFileName(name) : name);
    }
}