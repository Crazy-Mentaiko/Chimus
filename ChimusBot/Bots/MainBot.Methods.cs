using System.Text.RegularExpressions;
using ChimusBot.Utils;
using NetCord;
using NetCord.Rest;

namespace ChimusBot.Bots;

partial class MainBot
{

    private static async Task IntroduceChimus(SlashCommandInteraction command)
    {
        await command.RespondAsync("나는 대륙의 기상 차이무스! 당신들의 생일을 책임진다!");
    }

    private static async Task AddBirthday(SlashCommandInteraction command)
    {
        var targetValue = command.Data.Options.First(option => option.Name == "생일자").Value;
        var birthday = command.Data.Options.First(option => option.Name == "생일").Value ?? string.Empty;
        var channelValue = command.Data.Options.First(option => option.Name == "채널").Value;

        if (!ulong.TryParse(targetValue, out var targetId))
        {
            await command.RespondAsync("생일자 입력이 잘못됐어.");
            return;
        }

        if (string.IsNullOrEmpty(birthday) || !BirthdayPattern().IsMatch(birthday))
        {
            await command.RespondAsync("생일 입력이 잘못됐어.");
            return;
        }

        var resolvedData = command.Data.ResolvedData;
        if (!ulong.TryParse(channelValue, out var channelId) ||
            resolvedData is null ||
            resolvedData.Channels is null ||
            !resolvedData.Channels.TryGetValue(channelId, out var channel))
        {
            await command.RespondAsync("채널 입력이 잘못됐어.");
            return;
        }
        
        DbHelper.AddBirthday(targetId, birthday, command.GuildId ?? 0, channel.Id);
        DbHelper.Flush();

        await command.RespondAsync("생일 입력에 성공했다!");
    }

    private static async Task ShowBirthdays(SlashCommandInteraction command)
    {
        var embed = new EmbedProperties
        {
            Title = "알고 있는 생일자 목록",
            Description = "아래 생일이 되면 뿌린다!",
            Color = new Color(255, 0, 0)
        };

        var fields = new List<EmbedFieldProperties>();
        var guild = command.Guild;
        foreach (var birthday in DbHelper.GetBirthdays())
        {
            if (guild is null || !guild.Users.TryGetValue(birthday.Target, out var user))
                continue;
            fields.Add(new EmbedFieldProperties
            {
                Name = user.Nickname ?? user.GlobalName ?? user.Username,
                Value = birthday.MonthDay,
                Inline = true,
            });
        }

        embed.WithFields(fields);

        await command.RespondAsync(embed: embed);
    }

    private static readonly string[] RainbowSixSiegeAttacks =
    {
        "예비병력",
        "대처", "슬레지", "써마이트", "애쉬", "트위치", "몽타뉴", "글라즈", "퓨즈", "블리츠", "IQ",
        "벅", "블랙비어드", "카피탕", "히바나", "자칼", "잉", "조피아", "도깨비",
        "라이언", "핀카", "매버릭", "노마드", "그리드락", "뇌크", "아마루", "칼리",
        "야나", "에이스", "제로", "플로레스", "오사", "아자미", "센스"
    };

    private static readonly string[] RainbowSixSiegeDefends =
    {
        "예비병력",
        "스모크", "뮤트", "펄스", "캐슬", "DOC", "룩", "캅칸", "타찬카", "예거", "밴딧",
        "프로스트", "발키리", "카베이라", "에코", "미라", "리전", "엘라", "비질",
        "마에스트로", "알리바이", "클래시", "카이드", "모지", "워든", "고요", "와마이",
        "오릭스", "멜루시", "아루니", "썬더버드", "쏜", "그림"
    };

    private static async Task RandomPickR6S(SlashCommandInteraction command)
    {
        var isAttack = bool.TryParse(command.Data.Options.FirstOrDefault(option => option.Name == "공격")?.Value, out var attack) && attack;
        var containsRoe = bool.TryParse(command.Data.Options.FirstOrDefault(option => option.Name == "예비병력")?.Value, out var roe) && roe;

        var pool = isAttack ? RainbowSixSiegeAttacks : RainbowSixSiegeDefends;

        var index = RandomUtil.Random(containsRoe ? 0 : 1, pool.Length);
        var found = pool[index];

        await command.RespondAsync(text: $"`{found}` 해");
    }

    private static async Task PickOne(SlashCommandInteraction command)
    {
        var items = new[]
        {
            command.Data.Options.First(option => option.Name == "항목1").Value,
            command.Data.Options.First(option => option.Name == "항목2").Value,
            command.Data.Options.FirstOrDefault(option => option.Name == "항목3")?.Value,
            command.Data.Options.FirstOrDefault(option => option.Name == "항목4")?.Value,
            command.Data.Options.FirstOrDefault(option => option.Name == "항목5")?.Value,
            command.Data.Options.FirstOrDefault(option => option.Name == "항목6")?.Value,
            command.Data.Options.FirstOrDefault(option => option.Name == "항목7")?.Value,
            command.Data.Options.FirstOrDefault(option => option.Name == "항목8")?.Value,
            command.Data.Options.FirstOrDefault(option => option.Name == "항목9")?.Value,
            command.Data.Options.FirstOrDefault(option => option.Name == "항목10")?.Value,
        };
        var choiceList = items.Where(item => !string.IsNullOrEmpty(item)).ToArray();

        await command.RespondAsync($"이거: {RandomUtil.PickOne(choiceList)} 💁");
    }

    private static async Task Eue(SlashCommandInteraction command)
    {
        var guild = command.Guild;
        if (guild is null)
        {
            await command.RespondAsync("서버 내에서만 쓸 수 있어.");
            return;
        }

        var ruin1 = guild.Emojis.Values.FirstOrDefault(emote => emote.Name == "longruin1");
        var ruin2 = guild.Emojis.Values.FirstOrDefault(emote => emote.Name == "longruin2");
        var ruin3 = guild.Emojis.Values.FirstOrDefault(emote => emote.Name == "longruin3");

        if (ruin1 == null || ruin2 == null || ruin3 == null)
        {
            await command.RespondAsync("필요한 이모지 중에 뭔가 없어.");
            return;
        }
        
        await command.RespondAsync($"<:longruin1:{ruin1.Id}><:longruin2:{ruin2.Id}><:longruin3:{ruin3.Id}>");
    }

    private static async Task AddSchedule(SlashCommandInteraction command)
    {
        var guild = command.Guild;
        if (guild == null)
        {
            await command.RespondAsync("서버 내에서만 사용할 수 있어. 🧏");
            return;
        }
        
        var message = command.Data.Options.First(option => option.Name == "메시지").Value ?? string.Empty;
        var channelValue = command.Data.Options.First(option => option.Name == "채널").Value;
        var dateTime = command.Data.Options.First(option => option.Name == "일시").Value ?? string.Empty;

        var resolvedData = command.Data.ResolvedData;
        if (!ulong.TryParse(channelValue, out var channelId) ||
            resolvedData is null ||
            resolvedData.Channels is null ||
            !resolvedData.Channels.TryGetValue(channelId, out var channel) ||
            channel is not IGuildChannel)
        {
            await command.RespondAsync("채널 값이 잘못됐음. 🧏");
            return;
        }
        
        if (string.IsNullOrEmpty(dateTime) || !DateTime.TryParse(dateTime, out var scheduleDateTime))
        {
            await command.RespondAsync("날짜 값이 잘못됐음. 🧏");
            return;
        }
            
        DbHelper.AddSchedule(message, $"{guild.Id}:{channel.Id}", scheduleDateTime);
        DbHelper.Flush();

        await command.RespondAsync("일정 등록했음. 🙆");
    }

    private static async Task RemoveSchedule(SlashCommandInteraction command)
    {
        var no = command.Data.Options.First(option => option.Name == "번호").Value;
        if (!int.TryParse(no, out var scheduleId))
        {
            await command.RespondAsync("값이 잘못됐습니다 🧏");
            return;
        }
            
        DbHelper.RemoveSchedule(scheduleId);
        DbHelper.Flush();
        
        await command.RespondAsync("일정 제거했음. 🙆");
    }

    private static async Task ListupSchedules(SlashCommandInteraction command)
    {
        var embed = new EmbedProperties().WithTitle("일정이 다음과 같이 등록돼있어. 💁");
        var fields = new List<EmbedFieldProperties>();
            
        foreach (var schedule in DbHelper.GetSchedules())
        {
            var id = schedule.Id;
            var message = schedule.Message;
            var channelInfo = schedule.TargetChannel;
            var scheduled = schedule.DateTime.ToString("yyyy-MM-dd HH:mm");

            var colonPosition = channelInfo.IndexOf(':');
            var guildId = ulong.Parse(channelInfo[..colonPosition]);
            var channelId = ulong.Parse(channelInfo[(colonPosition + 1)..]);

            if (Instance!._client.Cache.Guilds.TryGetValue(guildId, out var guild) &&
                guild.Channels.TryGetValue(channelId, out var channel) &&
                channel is TextGuildChannel &&
                command.GuildId == guildId)
            {
                fields.Add(new EmbedFieldProperties { Name = "#", Value = id.ToString(), Inline = true });
                fields.Add(new EmbedFieldProperties { Name = "메시지", Value = message, Inline = true });
                fields.Add(new EmbedFieldProperties { Name = "채널 / 일정", Value = $"<#{channelId}> / {scheduled}", Inline = true });
            }
        }

        embed.WithFields(fields);
        await command.RespondAsync(embed: embed);
    }

    private static readonly string[] MagicalConchList = {
        "🐚 가만히 있어", "🐚 안 돼", "🐚 다시 한 번 물어봐",
        "🐚 그럼", "🐚 언젠가는", "🐚 그것도 안 돼"
    };
    private static readonly string[] ImageHomeList =
    {
        "want_to_go_home.png", "want_to_go_home_2.jpg", "want_to_go_home_3.jpg", "want_to_go_home_4.jpg",
        "want_to_go_home_5.jpg", "want_to_go_home_6.jpg", "want_to_go_home_7.jpg", "want_to_go_home_8.jpg",
        "want_to_go_home_9.jpg", "want_to_go_home_10.jpg", "want_to_go_home_11.jpg", "want_to_go_home_12.jpg",
        "want_to_go_home_13.jpg", "want_to_go_home_14.jpg", "want_to_go_home_15.jpg", "want_to_go_home_16.jpg",
    };
    private static readonly string[] ImageReimusNunnaList = {"sister_remmu.jpg"};
    private static readonly string[] ImageXylophoneList = {"xylophone.png"};
    private static readonly string[] ImageSaidNoList = {"i_took_thats_no.png", "i_took_thats_no_2.jpg"};
    private static readonly string[] ImageRuinmanList = {"rivo.jpg"};
    private static readonly string[] ImageBeskin31List = {"thirties_1.png", "thirties_2.png", "thirties_3.jpg"};
    private static readonly string[] ImageLickingList =
    {
        "licking_1.jpg", "licking_2.jpg", "licking_3.jpg", "licking_4.png", "licking_5.jpg", "licking_6.jpg",
        "licking_7.gif", "licking_8.gif", "licking_9.gif", "licking_10.gif", "licking_11.gif", "licking_12.gif",
    };
    private static readonly string[] ImageBambooSpearList = {"bamboo_spear.jpg"};
    private static readonly string[] ImageKnewItList = {"i_knew_it.gif"};
    private static readonly string[] ImageGoSleepList =
    {
        "zara_1.jpg", "zara_2.jpg", "zara_3.jpg", "zara_4.jpg", "zara_5.jpg",
    };
    private static readonly string[] ImageSolutionList = {"solution_1.jpg"};
    private static readonly string[] ImageDeadmansList =
    {
        "deads_1.png", "deads_2.png", "deads_3.png",
    };
    private static readonly string[] ImageChimusList = {"zzapmu.jpg"};
    private static readonly string[] ImageKkangList =
    {
        "kkang_1.jpg", "kkang_2.jpg",
    };
    private static readonly string[] ImageCockroachList = {"cockroach_calisthenics.mp4"};
    private static readonly string[] ImageZzolList = {"zzol.jpg"};
    private static readonly string[] ImageYouTooList = {"you_too.jpg"};
    private static readonly string[] ImageMeTooList = {"me_too.jpg"};
    private static readonly string[] ImageFunCoolSexyList =
    {
        "yakusoku.gif", "yakusoku_2.jpg",
    };
    private static readonly string[] ImageEvaList =
    {
        "eva.jpg", "eva-25.png",
    };
    private static readonly string[] ImageShisyashoseiList = {"shisyashosei.png"};
    private static readonly string[] ImageRecycleList = {"ecyc_e.mp4"};
    private static readonly string[] ImageDeliciousMiuList = {"delicious_miu.png"};
    private static readonly string[] ImageYaList = {"ya.mp4"};
    private static readonly string[] ImageGotSteamList = {"yeol_bat_ne.jpg"};
    private static readonly string[] ImageVoteList = {"vote.mp4"};
    private static readonly string[] ImageGaepanList = {"gaepan.jpg"};
    private static readonly string[] ImageAreYouLaughList = {"are_you_laugh.jpg"};
    private static readonly string[] ImageGotoBackList = {"go_to_back.jpg"};
    private static readonly string[] ImageShutTheMouthList = {"shut_the_mouth.jpg"};
    private static readonly string[] ImageDieList =
    {
        "death_cake_when_saw_3_times.jpg", "death_cake_when_saw_3_times_renewal.jpg",
        "otagai_korose.jpg", "just_die.jpg"
    };
    private static readonly string[] ImagePigList =
    {
        "png_1.png", "pig_2.png", "pig_3.png", "pig_4.png", "pig_5.png", "pig_6.gif"
    };
    private static readonly string[] ImageExodiaList = {"exodia.jpg"};
    private static readonly string[] ImageGiveUpList =
    {
        "giveup.png", "giveup_2.jpg"
    };

    private static async Task MagicalConch(SlashCommandInteraction command) =>
        await command.RespondAsync(RandomUtil.PickOne(MagicalConchList));

    private static async Task ImageHome(SlashCommandInteraction command) => await SendImageAsync(command, ImageHomeList);
    private static async Task ImageReimusNunna(SlashCommandInteraction command) => await SendImageAsync(command, ImageReimusNunnaList);
    private static async Task ImageXylophone(SlashCommandInteraction command) => await SendImageAsync(command, ImageXylophoneList);
    private static async Task ImageSaidNo(SlashCommandInteraction command) => await SendImageAsync(command, ImageSaidNoList);
    private static async Task ImageRuinman(SlashCommandInteraction command) => await SendImageAsync(command, ImageRuinmanList);
    private static async Task ImageBeskin31(SlashCommandInteraction command) => await SendImageAsync(command, ImageBeskin31List);
    private static async Task ImageLicking(SlashCommandInteraction command) => await SendImageAsync(command, ImageLickingList);
    private static async Task ImageBambooSpear(SlashCommandInteraction command) => await SendImageAsync(command, ImageBambooSpearList);
    private static async Task ImageKnewIt(SlashCommandInteraction command) => await SendImageAsync(command, ImageKnewItList);
    private static async Task ImageGoSleep(SlashCommandInteraction command) => await SendImageAsync(command, ImageGoSleepList);
    private static async Task ImageSolution(SlashCommandInteraction command) => await SendImageAsync(command, ImageSolutionList);
    private static async Task ImageDeadmans(SlashCommandInteraction command) => await SendImageAsync(command, ImageDeadmansList);
    private static async Task ImageChimus(SlashCommandInteraction command) => await SendImageAsync(command, ImageChimusList);
    private static async Task ImageKkang(SlashCommandInteraction command) => await SendImageAsync(command, ImageKkangList);
    private static async Task ImageCockroach(SlashCommandInteraction command) => await SendImageAsync(command, ImageCockroachList);
    private static async Task ImageZzol(SlashCommandInteraction command) => await SendImageAsync(command, ImageZzolList);
    private static async Task ImageYouToo(SlashCommandInteraction command) => await SendImageAsync(command, ImageYouTooList);
    private static async Task ImageMeToo(SlashCommandInteraction command) => await SendImageAsync(command, ImageMeTooList);
    private static async Task ImageFunCoolSexy(SlashCommandInteraction command) => await SendImageAsync(command, ImageFunCoolSexyList);
    private static async Task ImageEva(SlashCommandInteraction command) => await SendImageAsync(command, ImageEvaList);
    private static async Task ImageShisyashosei(SlashCommandInteraction command) => await SendImageAsync(command, ImageShisyashoseiList);
    private static async Task ImageRecycle(SlashCommandInteraction command) => await SendImageAsync(command, ImageRecycleList);
    private static async Task ImageDeliciousMiu(SlashCommandInteraction command) => await SendImageAsync(command, ImageDeliciousMiuList);
    private static async Task ImageYa(SlashCommandInteraction command) => await SendImageAsync(command, ImageYaList);
    private static async Task ImageGotSteam(SlashCommandInteraction command) => await SendImageAsync(command, ImageGotSteamList);
    private static async Task ImageVote(SlashCommandInteraction command) => await SendImageAsync(command, ImageVoteList);
    private static async Task ImageGaepan(SlashCommandInteraction command) => await SendImageAsync(command, ImageGaepanList);
    private static async Task ImageAreYouLaugh(SlashCommandInteraction command) => await SendImageAsync(command, ImageAreYouLaughList);
    private static async Task ImageGotoBack(SlashCommandInteraction command) => await SendImageAsync(command, ImageGotoBackList);
    private static async Task ImageShutTheMouth(SlashCommandInteraction command) => await SendImageAsync(command, ImageShutTheMouthList);
    private static async Task ImageDie(SlashCommandInteraction command) => await SendImageAsync(command, ImageDieList);
    private static async Task ImagePig(SlashCommandInteraction command) => await SendImageAsync(command, ImagePigList);
    private static async Task ImageExodia(SlashCommandInteraction command) => await SendImageAsync(command, ImageExodiaList);
    private static async Task ImageGiveUp(SlashCommandInteraction command) => await SendImageAsync(command, ImageGiveUpList);

    private static async Task SendImageAsync(SlashCommandInteraction command, string[] imageList)
    {
        var resourceName = RandomUtil.PickOne(imageList);
        var stream = EmbedResourceUtil.GetStream(resourceName);
        if (stream is null)
        {
            await command.RespondAsync("문제가 발생했음!");
            return;
        }

        await using (stream)
            await command.RespondWithFileAsync(stream, EmbedResourceUtil.GetFileName(resourceName));
    }

    [GeneratedRegex("^[0-1][0-9]-[0-3][0-9]$")]
    private static partial Regex BirthdayPattern();
}
