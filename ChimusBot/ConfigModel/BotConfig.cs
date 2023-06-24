using Newtonsoft.Json;

namespace ChimusBot.ConfigModel;

[Serializable, JsonObject]
public class BotConfig
{
    [JsonProperty("discord.token"), JsonRequired]
    public readonly string DiscordToken = string.Empty;
    [JsonProperty("db.path")]
    public readonly string DbPath = "data.db";

    private const string BotConfigPath = "./config/chimus.json";
    
    public BotConfig() { }

    public BotConfig(string? discordToken, string? dbPath)
    {
        DiscordToken = discordToken ?? DiscordToken;
        DbPath = dbPath ?? DbPath;
        if (string.IsNullOrEmpty(DiscordToken))
            throw new ArgumentOutOfRangeException();
    }
    
    public static BotConfig? LoadFromFile()
    {
        if (!File.Exists(BotConfigPath))
            return null;
        
        using var configFile = File.OpenText(BotConfigPath);
        return JsonSerializer.CreateDefault().Deserialize<BotConfig>(new JsonTextReader(configFile));
    }

    public static BotConfig LoadFromEnvironment()
    {
        return new BotConfig(
            Environment.GetEnvironmentVariable("DISCORD_TOKEN"),
            Environment.GetEnvironmentVariable("DB_PATH")
        );
    }
}