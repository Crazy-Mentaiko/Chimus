using ChimusBot.ConfigModel;
using LiteDB;
using LiteDB.Engine;

namespace ChimusBot.Utils;

public static class DbHelper
{
    private static readonly LiteDatabase _db;

    private static readonly ILiteCollection<Birthday> _birthdays; 

    static DbHelper()
    {
        _db = new LiteDatabase("data.db");
        _birthdays = _db.GetCollection<Birthday>();
    }

    public static void Flush()
    {
        _db.Rebuild();
    }

    public static void AddBirthday(ulong target, string monthDay, ulong guild, ulong channel)
    {
        Log.Info($"생일자: {target}, 날짜: {monthDay}, 서버: {guild}, 채널: {channel}");
        _birthdays.Insert(new Birthday(target, monthDay, guild, channel));
    }

    public static IEnumerable<Birthday> GetBirthdays() => _birthdays.Query().ToEnumerable();

    public static IEnumerable<Birthday> GetBirthdays(int month, int day)
    {
        var monthDay = $"{month:00}-{day:00}";
        return _birthdays.Query().Where(birthday => birthday.MonthDay == monthDay).ToEnumerable();
    }
}