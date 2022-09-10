using ChimusBot.ConfigModel;
using ChimusBot.Utils;
using Discord;
using Discord.Interactions.Builders;
using Discord.WebSocket;

namespace ChimusBot.Bots;

public partial class MainBot : IDisposable
{
    private readonly DiscordSocketClient _client;
    private readonly BotConfig _botConfig;
    
    public bool IsRunning => _client.Status != UserStatus.Offline;
    
    public MainBot(BotConfig botConfig)
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            AlwaysDownloadUsers = true,
            MessageCacheSize = 512,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
        });

        _botConfig = botConfig;
    }

    ~MainBot()
    {
        Dispose(false);
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _client.Dispose();
    }
    
    public async Task RunAsync()
    {
        _client.Connected += OnConnected;
        _client.Disconnected += OnDisconnected;
        _client.Ready += OnReady;
        
        _client.MessageReceived += OnMessageReceived;
        _client.MessageUpdated += OnMessageUpdated;
        _client.MessageDeleted += OnMessageDeleted;

        _client.SlashCommandExecuted += OnSlashCommandExecuted;
        
        await _client.LoginAsync(TokenType.Bot, _botConfig.DiscordToken);
        await _client.StartAsync();

        var removingScheduleIds = new List<int>();
        
        while (IsRunning)
        {
            var now = DateTime.Now;

            if (now.Hour == 0 && now.Minute == 0)
            {
                foreach (var birthday in DbHelper.GetBirthdays(now.Month, now.Day))
                {
                    if (birthday.Guild == 0)
                    {
                        // DM
                        Log.Info("Not implemented for DM");
                    }
                    else
                    {
                        var guild = _client.Guilds.FirstOrDefault(guild => guild.Id == birthday.Guild);
                        if (guild == null)
                        {
                            Log.Error("서버를 찾을 수 없었습니다.");
                            continue;
                        }

                        var channel = guild.Channels.FirstOrDefault(channel => channel.Id == birthday.Channel);
                        if (channel == null)
                        {
                            Log.Error("채널을 찾을 수 없었습니다.");
                            continue;
                        }

                        if (channel is not SocketTextChannel textChannel)
                        {
                            Log.Error("텍스트 채널이 아닙니다.");
                            continue;
                        }

                        await textChannel.SendMessageAsync($"🙌오늘은 <@${birthday.Target}>의 생일!👏");
                        var chimusEmoji = guild.Emotes.FirstOrDefault(emote => emote.Name == "china_reimus");
                        if (chimusEmoji != null)
                            await textChannel.SendMessageAsync($"<:china_reimus:{chimusEmoji.Id}>");
                    }
                }
            }

            var nowTime = DateTime.Now.TimeOfDay;
            Thread.Sleep((60 - nowTime.Seconds + 1) * 1000);
        }
    }

    #region Callbacks
    private async Task OnConnected()
    {
        Log.Info("짭무가 Discord 서버에 접속 됨...");
        await Task.CompletedTask;
    }

    private async Task OnDisconnected(Exception ex)
    {
        Log.Fatal(ex, "짭무가 Discord 서버에서 접속 해제 됨...");
        await Task.CompletedTask;
    }

    private async Task OnReady()
    {
        await _client.SetActivityAsync(new Game("명란젓의 체력을 책임진다. 부엉성기사 짭짭무"));
        await _client.SetStatusAsync(UserStatus.Online);
        
        Log.Info("짭무 준비 됨.");
        Log.Info($"짭무 이름: {_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator}");
        
        await InitializeCommandsAsync();
        Log.Info("명령어 준비 됨.");
        
        await Task.CompletedTask;
    }

    private async Task OnMessageReceived(SocketMessage message)
    {
        await Task.CompletedTask;
    }

    private async Task OnMessageUpdated(Cacheable<IMessage, ulong> beforeMessage, SocketMessage afterMessage, ISocketMessageChannel channel)
    {
        await Task.CompletedTask;
    }

    private async Task OnMessageDeleted(Cacheable<IMessage, ulong> deletedMessage, Cacheable<IMessageChannel, ulong> channel)
    {
        await Task.CompletedTask;
    }

    private async Task OnSlashCommandExecuted(SocketSlashCommand command)
    {
        await ReactionSlashCommandAsync(command);
    }
    #endregion
    
    #region Commands

    private async Task InitializeCommandsAsync()
    {
        foreach (var builtCommand in _commands.Keys.Select(builder => builder.Build()))
        {
            try
            {
                await _client.CreateGlobalApplicationCommandAsync(builtCommand);
                Log.Info($"글로벌에 \"{builtCommand.Name}\" 명령어 등록 완료");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"글로벌에 \"{builtCommand.Name}\" 명령어 등록 실패");
            }
        }
    }

    private async Task ReactionSlashCommandAsync(SocketSlashCommand command)
    {
        Log.Info($"명령 시도: {command.Data.Name}");
        await _commands.FirstOrDefault(kv => kv.Key.Name == command.CommandName).Value.Invoke(command);
    }

    #endregion
}