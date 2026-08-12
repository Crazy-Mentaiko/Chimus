using System.Text.Json;
using System.Text.Json.Serialization;
using ChimusBot.ConfigModel;
using ChimusBot.Utils;
using LiteDB;

namespace ChimusBot.DataMigration;

internal static class Program
{
    public static int Main(string[] args)
    {
        if (args.Contains("--help", StringComparer.OrdinalIgnoreCase) ||
            args.Contains("-h", StringComparer.OrdinalIgnoreCase))
        {
            PrintUsage();
            return 0;
        }

        var positionalArguments = args.Where(argument => !argument.StartsWith('-')).ToArray();
        var sourcePath = Path.GetFullPath(positionalArguments.ElementAtOrDefault(0) ?? "data.db");
        var outputPath = Path.GetFullPath(positionalArguments.ElementAtOrDefault(1) ?? "data.json");
        var force = args.Contains("--force", StringComparer.OrdinalIgnoreCase);

        try
        {
            Convert(sourcePath, outputPath, force);
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"변환 실패: {exception.Message}");
            return 1;
        }
    }

    private static void Convert(string sourcePath, string outputPath, bool force)
    {
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("LiteDB 파일을 찾을 수 없습니다.", sourcePath);
        if (string.Equals(sourcePath, outputPath, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("입력 파일과 출력 파일은 달라야 합니다.");
        if (File.Exists(outputPath) && !force)
            throw new IOException($"출력 파일이 이미 존재합니다. 덮어쓰려면 --force를 사용하세요: {outputPath}");

        using var database = new LiteDatabase($"Filename={sourcePath};ReadOnly=true");
        var collectionNames = database.GetCollectionNames().ToArray();

        var birthdays = ReadCollection<LegacyBirthday>(database, collectionNames, "Birthday", "Birthdays")
            .Select(item => new Birthday
            {
                Id = item.Id,
                Target = item.Target,
                MonthDay = item.MonthDay,
                Guild = item.Guild,
                Channel = item.Channel,
                Visible = item.Visible,
            })
            .ToList();

        var schedules = ReadCollection<LegacySchedule>(database, collectionNames, "Schedule", "Schedules")
            .Select(item => new Schedule
            {
                Id = item.Id,
                Message = item.Message,
                TargetChannel = item.TargetChannel,
                DateTime = item.DateTime,
            })
            .ToList();

        var data = new BotData
        {
            Birthdays = birthdays,
            Schedules = schedules,
        };

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var temporaryPath = $"{outputPath}.tmp";
        using (var stream = new FileStream(temporaryPath, FileMode.Create, FileAccess.Write, FileShare.None))
            System.Text.Json.JsonSerializer.Serialize(stream, data, MigrationJsonContext.Default.BotData);
        File.Move(temporaryPath, outputPath, true);

        Console.WriteLine($"변환 완료: {sourcePath} -> {outputPath}");
        Console.WriteLine($"생일 {birthdays.Count}개, 스케줄 {schedules.Count}개");
    }

    private static IReadOnlyList<T> ReadCollection<T>(
        LiteDatabase database,
        IReadOnlyList<string> collectionNames,
        params string[] candidates)
    {
        var collectionName = candidates
            .Select(candidate => collectionNames.FirstOrDefault(
                name => string.Equals(name, candidate, StringComparison.OrdinalIgnoreCase)))
            .FirstOrDefault(name => name is not null);

        return collectionName is null
            ? []
            : database.GetCollection<T>(collectionName).FindAll().ToArray();
    }

    private static void PrintUsage()
    {
        Console.WriteLine("사용법:");
        Console.WriteLine("  dotnet run --project ChimusBot.DataMigration -- [입력.db] [출력.json] [--force]");
        Console.WriteLine();
        Console.WriteLine("경로를 생략하면 data.db를 data.json으로 변환합니다.");
    }

    private sealed class LegacyBirthday
    {
        [BsonId]
        public int Id { get; set; }
        public ulong Target { get; set; }
        public string MonthDay { get; set; } = "01-01";
        public ulong Guild { get; set; }
        public ulong Channel { get; set; }
        public bool Visible { get; set; } = true;
    }

    private sealed class LegacySchedule
    {
        [BsonId]
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TargetChannel { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
    }
}

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = true)]
[JsonSerializable(typeof(BotData))]
internal partial class MigrationJsonContext : JsonSerializerContext;
