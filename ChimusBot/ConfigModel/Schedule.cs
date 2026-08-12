namespace ChimusBot.ConfigModel;

public class Schedule
{
    public int Id { get; set; }
    public string Message { get; set; }
    public string TargetChannel { get; set; }
    public DateTime DateTime { get; set; }

    public Schedule()
    {
        Message = string.Empty;
        TargetChannel = string.Empty;
    }

    public Schedule(string message, string targetChannel, DateTime dateTime)
    {
        Message = message;
        TargetChannel = targetChannel;
        DateTime = dateTime;
    }
}
