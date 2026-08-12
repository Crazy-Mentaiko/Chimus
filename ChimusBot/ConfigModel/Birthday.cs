namespace ChimusBot.ConfigModel;

public class Birthday
{
    public int Id { get; set; }
    public ulong Target { get; set; }
    public string MonthDay { get; set; }
    public ulong Guild { get; set; }
    public ulong Channel { get; set; }
    public bool Visible { get; set; }

    public Birthday()
    {
        MonthDay = "01-01";
        Visible = true;
    }

    public Birthday(ulong target, string monthDay, ulong guild, ulong channel, bool visible = true)
    {
        Target = target;
        MonthDay = monthDay;
        Guild = guild;
        Channel = channel;
        Visible = visible;
    }
}
