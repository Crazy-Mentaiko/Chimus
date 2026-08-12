using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChimusBot.ConfigModel;

public class BotConfig
{
    [JsonPropertyName("discord.token")]
    public string DiscordToken { get; init; } = string.Empty;

    [JsonPropertyName("db.path")]
    public string DbPath { get; init; } = "data.db";

    private const string BotConfigPath = "./config/chimus.json";

    public BotConfig() { }

    public BotConfig(string? discordToken, string? dbPath)
    {
        DiscordToken = discordToken ?? DiscordToken;
        DbPath = dbPath ?? DbPath;
        if (string.IsNullOrEmpty(DiscordToken))
            throw new ArgumentOutOfRangeException(nameof(discordToken));
    }

    public static BotConfig? LoadFromFile()
    {
        if (!File.Exists(BotConfigPath))
            return null;

        using var configFile = File.OpenRead(BotConfigPath);
        var config = JsonSerializer.Deserialize(configFile, BotConfigJsonContext.Default.BotConfig);
        return config is { DiscordToken.Length: > 0 } ? config : null;
    }

    public static BotConfig LoadFromEnvironment()
    {
        return new BotConfig(
            Environment.GetEnvironmentVariable("DISCORD_TOKEN"),
            Environment.GetEnvironmentVariable("DB_PATH")
        );
    }
}

[JsonSerializable(typeof(BotConfig))]
internal partial class BotConfigJsonContext : JsonSerializerContext;
