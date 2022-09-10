using Newtonsoft.Json;

namespace ChimusBot.ConfigModel;

[Serializable, JsonObject]
public class BotConfig
{
    [JsonProperty("discord.token"), JsonRequired]
    public readonly string DiscordToken = string.Empty;

    private const string BotConfigPath = "./config/chimus.json";
    
    public static BotConfig? Load()
    {
        if (!File.Exists(BotConfigPath))
            return null;
        
        using var configFile = File.OpenText(BotConfigPath);
        return JsonSerializer.CreateDefault().Deserialize<BotConfig>(new JsonTextReader(configFile));
    }
}