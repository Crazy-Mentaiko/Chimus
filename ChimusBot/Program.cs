using System.Runtime.InteropServices;
using ChimusBot.Bots;
using ChimusBot.ConfigModel;
using ChimusBot.Utils;

Log.Info(" _____  _   _ ________  ____   _ _____ ");
Log.Info("/  __ \\| | | |_   _|  \\/  | | | /  ___|");
Log.Info("| /  \\/| |_| | | | | .  . | | | \\ `--. ");
Log.Info("| |    |  _  | | | | |\\/| | | | |`--. \\");
Log.Info("| \\__/\\| | | |_| |_| |  | | |_| /\\__/ /");
Log.Info(" \\____/\\_| |_/\\___/\\_|  |_/\\___/\\____/ ");

var botConfig = BotConfig.LoadFromFile() ?? BotConfig.LoadFromEnvironment();
DbHelper.Initialize(botConfig);

var chimusBot = new MainBot(botConfig);

PosixSignalRegistration.Create(PosixSignal.SIGTERM, context =>
{
    DbHelper.Dispose();
});

chimusBot.RunAsync().GetAwaiter().GetResult();

return 0;