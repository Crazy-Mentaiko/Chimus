using ChimusBot.Bots;
using ChimusBot.ConfigModel;
using ChimusBot.Utils;

var botConfig = BotConfig.Load();
if (botConfig == null)
{
    Log.Error("짭무 설정 파일이 존재하지 않습니다.");
    return 1;
}

var chimusBot = new MainBot(botConfig);

chimusBot.RunAsync().GetAwaiter().GetResult();

return 0;