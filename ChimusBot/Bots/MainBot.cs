using ChimusBot.ConfigModel;
using ChimusBot.Utils;
using NetCord;
using NetCord.Gateway;
using NetCord.Rest;

namespace ChimusBot.Bots;

public partial class MainBot : IDisposable
{
    public static MainBot? Instance { get; private set; }

    private readonly GatewayClient _client;
    private bool _disposed;

    public bool IsRunning => _client.Status != WebSocketStatus.Disconnected;

    public MainBot(BotConfig botConfig)
    {
        _client = new GatewayClient(new BotToken(botConfig.DiscordToken), new GatewayClientConfiguration
        {
            Intents = GatewayIntents.AllNonPrivileged | GatewayIntents.GuildUsers,
        });

        Instance = this;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _client.Dispose();
        Instance = null;
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    public async Task RunAsync()
    {
        _client.Connect += OnConnected;
        _client.Disconnect += OnDisconnected;
        _client.Ready += OnReady;
        _client.InteractionCreate += OnInteractionCreate;

        var presence = new PresenceProperties(UserStatusType.Online)
            .AddActivities(new UserActivityProperties(
                "명란젓의 체력을 책임진다. 부엉성기사 짭짭무",
                UserActivityType.Playing));

        await _client.StartAsync(presence);

        while (IsRunning)
        {
            await RunScheduledJobsAsync();

            var now = DateTime.Now.TimeOfDay;
            await Task.Delay(TimeSpan.FromSeconds(61 - now.Seconds));
        }
    }

    private async Task RunScheduledJobsAsync()
    {
        var now = DateTime.Now;

        if (now is { Hour: 0, Minute: 0 })
        {
            foreach (var birthday in DbHelper.GetBirthdays(now.Month, now.Day))
            {
                if (birthday.Guild == 0)
                {
                    Log.Info("Not implemented for DM");
                    continue;
                }

                if (!_client.Cache.Guilds.TryGetValue(birthday.Guild, out var guild))
                {
                    Log.Error("서버를 찾을 수 없었습니다.");
                    continue;
                }

                if (!guild.Channels.TryGetValue(birthday.Channel, out var channel) || channel is not TextGuildChannel)
                {
                    Log.Error("텍스트 채널을 찾을 수 없었습니다.");
                    continue;
                }

                await _client.Rest.SendMessageAsync(birthday.Channel,
                    new MessageProperties { Content = $"🙌오늘은 <@{birthday.Target}>의 생일!👏" });

                var chimusEmoji = guild.Emojis.Values.FirstOrDefault(emoji => emoji.Name == "china_reimus");
                if (chimusEmoji is not null)
                {
                    await _client.Rest.SendMessageAsync(birthday.Channel,
                        new MessageProperties { Content = $"<:china_reimus:{chimusEmoji.Id}>" });
                }
            }
        }

        var matchedSchedules = DbHelper.GetSchedules().Where(schedule =>
        {
            var dateTime = schedule.DateTime;
            return dateTime.Date == now.Date && dateTime.Hour == now.Hour && dateTime.Minute == now.Minute;
        }).ToArray();

        foreach (var schedule in matchedSchedules)
        {
            Log.Info($"스케쥴: ID: {schedule.Id}, 채널 - {schedule.TargetChannel}, 메시지: {schedule.Message}");

            var separator = schedule.TargetChannel.IndexOf(':');
            var guildId = ulong.Parse(schedule.TargetChannel[..separator]);
            var channelId = ulong.Parse(schedule.TargetChannel[(separator + 1)..]);

            if (!_client.Cache.Guilds.TryGetValue(guildId, out var guild) ||
                !guild.Channels.TryGetValue(channelId, out var channel) ||
                channel is not TextGuildChannel)
            {
                Log.Error($"텍스트 채널을 못 찾았음: {channelId} from {guildId}");
                continue;
            }

            await _client.Rest.SendMessageAsync(channelId, new MessageProperties { Content = schedule.Message });
        }

        foreach (var schedule in matchedSchedules)
            DbHelper.RemoveSchedule(schedule.Id);

        if (matchedSchedules.Length > 0)
            DbHelper.Flush();
    }

    private ValueTask OnConnected()
    {
        Log.Info("짭무가 Discord 서버에 접속 됨...");
        return ValueTask.CompletedTask;
    }

    private ValueTask OnDisconnected(DisconnectEventArgs args)
    {
        Log.Error($"짭무가 Discord 서버에서 접속 해제 됨... 재접속: {args.Reconnect}");
        return ValueTask.CompletedTask;
    }

    private async ValueTask OnReady(ReadyEventArgs args)
    {
        Log.Info("짭무 준비 됨.");
        Log.Info($"짭무 이름: {args.User.Username}");

        foreach (var guildId in args.GuildIds)
            await _client.RequestGuildUsersAsync(new GuildUsersRequestProperties(guildId));

        await InitializeCommandsAsync(args.ApplicationId);
        Log.Info("명령어 준비 됨.");
    }

    private async ValueTask OnInteractionCreate(Interaction interaction)
    {
        if (interaction is SlashCommandInteraction command)
            await ReactionSlashCommandAsync(command);
    }

    private async Task InitializeCommandsAsync(ulong applicationId)
    {
        await _client.Rest.BulkOverwriteGlobalApplicationCommandsAsync(
            applicationId,
            Commands.Values.Select(command => command.Properties));
    }

    private static async Task ReactionSlashCommandAsync(SlashCommandInteraction command)
    {
        Log.Info($"명령 시도: {command.Data.Name}");
        if (!Commands.TryGetValue(command.Data.Name, out var foundCommand))
        {
            Log.Error("명령어가 등록되어 있지 않음");
            await command.RespondAsync("명령어 설정이 좀 잘못된 거 같은데...");
            return;
        }

        try
        {
            await foundCommand.Handler(command);
        }
        catch (Exception ex)
        {
            Log.Error($"Error raised: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
