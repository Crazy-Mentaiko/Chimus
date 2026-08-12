using System.Text.Json;
using System.Text.Json.Serialization;
using ChimusBot.ConfigModel;

namespace ChimusBot.Utils;

public static class DbHelper
{
    private static readonly object SyncRoot = new();

    private static string? _dataPath;
    private static BotData? _data;
    private static bool _dirty;

    public static void Initialize(BotConfig config)
    {
        lock (SyncRoot)
        {
            if (_data is not null)
                throw new InvalidOperationException("데이터 저장소가 이미 초기화되었습니다.");

            _dataPath = Path.GetFullPath(config.DbPath);
            _data = Load(_dataPath);
            _dirty = false;
            NormalizeIds(_data);
        }
    }

    public static void Dispose()
    {
        lock (SyncRoot)
        {
            if (_data is null)
                return;

            FlushCore();
            _data = null;
            _dataPath = null;
            _dirty = false;
        }
    }

    public static void Flush()
    {
        lock (SyncRoot)
        {
            EnsureInitialized();
            FlushCore();
        }
    }

    public static void AddBirthday(ulong target, string monthDay, ulong guild, ulong channel)
    {
        lock (SyncRoot)
        {
            var data = EnsureInitialized();
            Log.Info($"생일자: {target}, 날짜: {monthDay}, 서버: {guild}, 채널: {channel}");

            data.Birthdays.Add(new Birthday(target, monthDay, guild, channel)
            {
                Id = GetNextId(data.Birthdays.Select(item => item.Id)),
            });
            _dirty = true;
        }
    }

    public static IEnumerable<Birthday> GetBirthdays()
    {
        lock (SyncRoot)
            return EnsureInitialized().Birthdays.Select(Clone).ToArray();
    }

    public static IEnumerable<Birthday> GetBirthdays(int month, int day)
    {
        var monthDay = $"{month:00}-{day:00}";
        lock (SyncRoot)
        {
            return EnsureInitialized().Birthdays
                .Where(birthday => birthday.MonthDay == monthDay)
                .Select(Clone)
                .ToArray();
        }
    }

    public static void AddSchedule(string message, string targetChannel, DateTime dateTime)
    {
        lock (SyncRoot)
        {
            var data = EnsureInitialized();
            Log.Info($"메시지: {message}, 채널: {targetChannel}, 일시: {dateTime}");

            data.Schedules.Add(new Schedule(message, targetChannel, dateTime)
            {
                Id = GetNextId(data.Schedules.Select(item => item.Id)),
            });
            _dirty = true;
        }
    }

    public static void RemoveSchedule(int id)
    {
        lock (SyncRoot)
        {
            var removed = EnsureInitialized().Schedules.RemoveAll(schedule => schedule.Id == id);
            _dirty |= removed > 0;
        }
    }

    public static IEnumerable<Schedule> GetSchedules()
    {
        lock (SyncRoot)
            return EnsureInitialized().Schedules.Select(Clone).ToArray();
    }

    private static BotData Load(string path)
    {
        if (!File.Exists(path))
            return new BotData();

        try
        {
            using var stream = File.OpenRead(path);
            return JsonSerializer.Deserialize(stream, BotDataJsonContext.Default.BotData)
                ?? throw new InvalidDataException($"JSON 데이터가 비어 있습니다: {path}");
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException(
                $"JSON 데이터를 읽을 수 없습니다: {path}. 기존 LiteDB 파일이라면 ChimusBot.DataMigration으로 먼저 변환하세요.",
                exception);
        }
    }

    private static void FlushCore()
    {
        if (!_dirty)
            return;

        var path = _dataPath!;
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var temporaryPath = $"{path}.tmp";
        using (var stream = new FileStream(temporaryPath, FileMode.Create, FileAccess.Write, FileShare.None))
            JsonSerializer.Serialize(stream, EnsureInitialized(), BotDataJsonContext.Default.BotData);

        File.Move(temporaryPath, path, true);
        _dirty = false;
    }

    private static BotData EnsureInitialized() =>
        _data ?? throw new InvalidOperationException("데이터 저장소가 초기화되지 않았습니다.");

    private static int GetNextId(IEnumerable<int> ids)
    {
        var maxId = ids.DefaultIfEmpty(0).Max();
        return checked(maxId + 1);
    }

    private static void NormalizeIds(BotData data)
    {
        NormalizeIds(data.Birthdays, item => item.Id, (item, id) => item.Id = id);
        NormalizeIds(data.Schedules, item => item.Id, (item, id) => item.Id = id);
    }

    private static void NormalizeIds<T>(List<T> items, Func<T, int> getId, Action<T, int> setId)
    {
        var usedIds = new HashSet<int>();
        var nextId = 1;
        foreach (var item in items)
        {
            var id = getId(item);
            if (id > 0 && usedIds.Add(id))
                continue;

            while (!usedIds.Add(nextId))
                nextId++;
            setId(item, nextId++);
            _dirty = true;
        }
    }

    private static Birthday Clone(Birthday birthday) => new()
    {
        Id = birthday.Id,
        Target = birthday.Target,
        MonthDay = birthday.MonthDay,
        Guild = birthday.Guild,
        Channel = birthday.Channel,
        Visible = birthday.Visible,
    };

    private static Schedule Clone(Schedule schedule) => new()
    {
        Id = schedule.Id,
        Message = schedule.Message,
        TargetChannel = schedule.TargetChannel,
        DateTime = schedule.DateTime,
    };
}

public sealed class BotData
{
    public List<Birthday> Birthdays { get; set; } = [];
    public List<Schedule> Schedules { get; set; } = [];
}

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = true)]
[JsonSerializable(typeof(BotData))]
internal partial class BotDataJsonContext : JsonSerializerContext;
