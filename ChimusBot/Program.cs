using ChimusBot.Bots;
using ChimusBot.ConfigModel;
using ChimusBot.Utils;

Log.Info(" _____  _   _ ________  ____   _ _____ ");
Log.Info("/  __ \\| | | |_   _|  \\/  | | | /  ___|");
Log.Info("| /  \\/| |_| | | | | .  . | | | \\ `--. ");
Log.Info("| |    |  _  | | | | |\\/| | | | |`--. \\");
Log.Info("| \\__/\\| | | |_| |_| |  | | |_| /\\__/ /");
Log.Info(" \\____/\\_| |_/\\___/\\_|  |_/\\___/\\____/ ");

var botConfig = BotConfig.Load();
if (botConfig == null)
{
    Log.Error("짭무 설정 파일이 존재하지 않습니다.");
    return 1;
}

var chimusBot = new MainBot(botConfig);

chimusBot.RunAsync().GetAwaiter().GetResult();

return 0;