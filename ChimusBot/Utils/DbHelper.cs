using ChimusBot.ConfigModel;
using LiteDB;
using LiteDB.Engine;

namespace ChimusBot.Utils;

public static class DbHelper
{
    private static LiteDatabase? _db;

    private static ILiteCollection<Birthday>? _birthdays;
    private static ILiteCollection<Schedule>? _schedules;

    public static void Initialize(BotConfig config)
    {
        _db = new LiteDatabase(config.DbPath);
        _birthdays = _db.GetCollection<Birthday>();
        _schedules = _db.GetCollection<Schedule>();
    }

    public static void Dispose()
    {
        _birthdays = null;
        _schedules = null;
        
        _db?.Dispose();
        _db = null;
    }

    public static void Flush()
    {
        _db!.Rebuild();
    }

    public static void AddBirthday(ulong target, string monthDay, ulong guild, ulong channel)
    {
        Log.Info($"생일자: {target}, 날짜: {monthDay}, 서버: {guild}, 채널: {channel}");
        _birthdays!.Insert(new Birthday(target, monthDay, guild, channel));
    }

    public static IEnumerable<Birthday> GetBirthdays() => _birthdays!.Query().ToEnumerable();

    public static IEnumerable<Birthday> GetBirthdays(int month, int day)
    {
        var monthDay = $"{month:00}-{day:00}";
        return _birthdays!.Query().Where(birthday => birthday.MonthDay == monthDay).ToEnumerable();
    }

    public static void AddSchedule(string message, string targetChannel, DateTime dateTime)
    {
        Log.Info($"메시지: {message}, 채널: {targetChannel}, 일시: {dateTime}");
        _schedules!.Insert(new Schedule(message, targetChannel, dateTime));
    }

    public static void RemoveSchedule(int id)
    {
        _schedules!.Delete(id);
    }

    public static IEnumerable<Schedule> GetSchedules() => _schedules!.Query().ToArray();
}