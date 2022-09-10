using NLog;

namespace ChimusBot.Utils;

public static class Log
{
    private static readonly Logger Logger = LogManager.GetLogger("Chimus");

    public static void Info(string message) => Logger.Info(message);
    public static void Debug(string message) => Logger.Debug(message);
    public static void Warn(string message) => Logger.Warn(message);
    public static void Error(string message) => Logger.Error(message);
    public static void Fatal(Exception exception, string message) => Logger.Fatal(exception, message);
}