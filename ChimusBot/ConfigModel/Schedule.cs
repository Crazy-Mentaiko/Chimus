using LiteDB;

namespace ChimusBot.ConfigModel;

[Serializable]
public class Schedule
{
    [BsonId]
    public int Id { get; set; }
    public string Message { get; set; }
    public string TargetChannel { get; set; }
    public DateTime DateTime { get; set; }

    public Schedule(string message, string targetChannel, DateTime dateTime)
    {
        Message = message;
        TargetChannel = targetChannel;
        DateTime = dateTime;
    }
}