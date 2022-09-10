using LiteDB;

namespace ChimusBot.ConfigModel;

[Serializable]
public class Birthday
{
    [BsonId]
    public int Id { get; set; }
    public ulong Target { get; set; }
    public string MonthDay { get; set; }
    public ulong Guild { get; set; }
    public ulong Channel { get; set; }
    public bool Visible { get; set; }

    public Birthday()
    {
        Id = 0;
        Target = 0;
        MonthDay = "01-01";
        Guild = 0;
        Channel = 0;
        Visible = true;
    }

    public Birthday(ulong target, string monthDay, ulong guild, ulong channel, bool visible = true)
    {
        Id = 0;
        Target = target;
        MonthDay = monthDay;
        Guild = guild;
        Channel = channel;
        Visible = visible;
    }
}