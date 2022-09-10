using System.Text.RegularExpressions;
using ChimusBot.Utils;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace ChimusBot.Bots;

public partial class MainBot
{
    private static readonly Dictionary<SlashCommandBuilder, Func<SocketSlashCommand, Task>> _commands = new()
    {
        {
            new SlashCommandBuilder().WithName("소개").WithDescription("짭무를 소개합니다."),
            IntroduceChimus
        },
        {
            new SlashCommandBuilder().WithName("생일추가").WithDescription("생일 목록에 생일을 추가합니다.")
                .AddOption("생일자", ApplicationCommandOptionType.User, "생일자를 멘션 형태로 작성")
                .AddOption("생일", ApplicationCommandOptionType.String, "월월-일일")
                .AddOption("채널", ApplicationCommandOptionType.Channel, "생일을 표시할 채널")
                .WithDefaultMemberPermissions(GuildPermission.Administrator | GuildPermission.ManageGuild),
            AddBirthday
        },
        {
            new SlashCommandBuilder().WithName("생일목록").WithDescription("생일 목록을 보여줍니다."),
            ShowBirthdays
        },
        {
            new SlashCommandBuilder().WithName("랜덤픽").WithDescription("레인보우식스 시즈 랜덤픽")
                .AddOption("공격", ApplicationCommandOptionType.Boolean, "공격이면 true, 수비면 false")
                .AddOption("예비병력", ApplicationCommandOptionType.Boolean, "예비병력 포함이면 true", isRequired: false),
            RandomPickR6S
        },
        {
            new SlashCommandBuilder().WithName("골라줘").WithDescription("골라준다.")
                .AddOption("항목1", ApplicationCommandOptionType.String, "필수", isRequired: true)
                .AddOption("항목2", ApplicationCommandOptionType.String, "필수", isRequired: true)
                .AddOption("항목3", ApplicationCommandOptionType.String, "선택", isRequired: false)
                .AddOption("항목4", ApplicationCommandOptionType.String, "선택", isRequired: false)
                .AddOption("항목5", ApplicationCommandOptionType.String, "선택", isRequired: false)
                .AddOption("항목6", ApplicationCommandOptionType.String, "선택", isRequired: false)
                .AddOption("항목7", ApplicationCommandOptionType.String, "선택", isRequired: false)
                .AddOption("항목8", ApplicationCommandOptionType.String, "선택", isRequired: false)
                .AddOption("항목9", ApplicationCommandOptionType.String, "선택", isRequired: false)
                .AddOption("항목10", ApplicationCommandOptionType.String, "선택", isRequired: false),
            PickOne
        },
        {
            new SlashCommandBuilder().WithName("으").WithDescription("으;"),
            Eue
        },
        {
            new SlashCommandBuilder().WithName("소라고둥").WithDescription("마법의 소라고둥님"),
            MagicalConch
        },
        {
            new SlashCommandBuilder().WithName("집").WithDescription("랜덤 이미지"),
            ImageHome
        },
        {
            new SlashCommandBuilder().WithName("쪼옥").WithDescription("랜덤 이미지"),
            ImageSunkist
        },
        {
            new SlashCommandBuilder().WithName("사이퍼즈").WithDescription("랜덤 이미지"),
            ImageCyphers
        },
        {
            new SlashCommandBuilder().WithName("누나").WithDescription("랜덤 이미지"),
            ImageReimusNunna
        },
        {
            new SlashCommandBuilder().WithName("띵똥땡똥").WithDescription("랜덤 이미지"),
            ImageXylophone
        },
        {
            new SlashCommandBuilder().WithName("아니").WithDescription("랜덤 이미지"),
            ImageSaidNo
        },
        {
            new SlashCommandBuilder().WithName("파멸맨").WithDescription("랜덤 이미지"),
            ImageRuinman
        },
        {
            new SlashCommandBuilder().WithName("31").WithDescription("랜덤 이미지"),
            ImageBeskin31
        },
        {
            new SlashCommandBuilder().WithName("똥").WithDescription("랜덤 이미지"),
            ImagePoo
        },
        {
            new SlashCommandBuilder().WithName("할짝").WithDescription("랜덤 이미지"),
            ImageLicking
        },
        {
            new SlashCommandBuilder().WithName("죽창").WithDescription("랜덤 이미지"),
            ImageBambooSpear
        },
        {
            new SlashCommandBuilder().WithName("이랄줄").WithDescription("랜덤 이미지"),
            ImageKnewIt
        },
        {
            new SlashCommandBuilder().WithName("자라").WithDescription("랜덤 이미지"),
            ImageGoSleep
        },
        {
            new SlashCommandBuilder().WithName("해결책").WithDescription("랜덤 이미지"),
            ImageSolution
        },
        {
            new SlashCommandBuilder().WithName("망자").WithDescription("랜덤 이미지"),
            ImageDeadmans
        },
        {
            new SlashCommandBuilder().WithName("짭무").WithDescription("랜덤 이미지"),
            ImageChimus
        },
        {
            new SlashCommandBuilder().WithName("깡").WithDescription("랜덤 이미지"),
            ImageKkang
        },
        {
            new SlashCommandBuilder().WithName("고키부리").WithDescription("랜덤 이미지"),
            ImageCockroach
        },
        {
            new SlashCommandBuilder().WithName("쫄").WithDescription("랜덤 이미지"),
            ImageZzol
        },
        {
            new SlashCommandBuilder().WithName("야너두").WithDescription("랜덤 이미지"),
            ImageYouToo
        },
        {
            new SlashCommandBuilder().WithName("야나두").WithDescription("랜덤 이미지"),
            ImageMeToo
        },
        {
            new SlashCommandBuilder().WithName("펀쿨섹").WithDescription("랜덤 이미지"),
            ImageFunCoolSexy
        },
        {
            new SlashCommandBuilder().WithName("뭐지").WithDescription("랜덤 이미지"),
            ImageWhat
        },
        {
            new SlashCommandBuilder().WithName("최선").WithDescription("랜덤 이미지"),
            ImageDoesTheBest
        },
        {
            new SlashCommandBuilder().WithName("에바").WithDescription("랜덤 이미지"),
            ImageEva
        },
        {
            new SlashCommandBuilder().WithName("죽은자의소생").WithDescription("랜덤 이미지"),
            ImageShisyashosei
        },
        {
            new SlashCommandBuilder().WithName("두손두발").WithDescription("랜덤 이미지"),
            ImageBothHandBothFeet
        },
        {
            new SlashCommandBuilder().WithName("이끼끼").WithDescription("랜덤 이미지"),
            ImageRecycle
        },
        {
            new SlashCommandBuilder().WithName("미우").WithDescription("랜덤 이미지"),
            ImageDeliciousMiu
        },
        {
            new SlashCommandBuilder().WithName("피곤").WithDescription("랜덤 이미지"),
            ImageTired
        },
        {
            new SlashCommandBuilder().WithName("난똥즈").WithDescription("랜덤 이미지"),
            ImageNanddongs
        },
        {
            new SlashCommandBuilder().WithName("야").WithDescription("랜덤 이미지"),
            ImageYa
        },
        {
            new SlashCommandBuilder().WithName("열받네").WithDescription("랜덤 이미지"),
            ImageGotSteam
        },
        {
            new SlashCommandBuilder().WithName("투표").WithDescription("랜덤 이미지"),
            ImageVote
        },
        {
            new SlashCommandBuilder().WithName("개판").WithDescription("랜덤 이미지"),
            ImageGaepan
        },
        {
            new SlashCommandBuilder().WithName("웃어").WithDescription("랜덤 이미지"),
            ImageAreYouLaugh
        },
        {
            new SlashCommandBuilder().WithName("돌아가").WithDescription("랜덤 이미지"),
            ImageGotoBack
        },
        {
            new SlashCommandBuilder().WithName("입닫아").WithDescription("랜덤 이미지"),
            ImageShutTheMouth
        },
        {
            new SlashCommandBuilder().WithName("생활패턴").WithDescription("랜덤 이미지"),
            ImageLifePattern
        },
        {
            new SlashCommandBuilder().WithName("죽어").WithDescription("랜덤 이미지"),
            ImageDie
        },
        {
            new SlashCommandBuilder().WithName("부숴버리겠어").WithDescription("랜덤 이미지"),
            ImageWillCrashYou
        },
        {
            new SlashCommandBuilder().WithName("사방").WithDescription("랜덤 이미지"),
            ImageSabang
        },
        {
            new SlashCommandBuilder().WithName("돼지").WithDescription("랜덤 이미지"),
            ImagePig
        },
        {
            new SlashCommandBuilder().WithName("인생").WithDescription("랜덤 이미지"),
            ImageLife
        },
        {
            new SlashCommandBuilder().WithName("엑조디아").WithDescription("랜덤 이미지"),
            ImageExodia
        },
        {
            new SlashCommandBuilder().WithName("포기해").WithDescription("랜덤 이미지"),
            ImageGiveUp
        },
    };

    static MainBot()
    {
        
    }

    private static async Task IntroduceChimus(SocketSlashCommand command)
    {
        await command.RespondAsync("나는 대륙의 기상 차이무스! 당신들의 생일을 책임진다!");
    }

    private static async Task AddBirthday(SocketSlashCommand command)
    {
        var target = command.Data.Options.First(option => option.Name == "생일자")?.Value as SocketUser;
        var birthday = command.Data.Options.First(option => option.Name == "생일")?.Value as string ?? string.Empty;
        var channel = command.Data.Options.First(option => option.Name == "채널")?.Value as SocketChannel;

        if (target == null)
        {
            await command.RespondAsync("생일자 입력이 잘못됐어.");
            return;
        }

        if (string.IsNullOrEmpty(birthday) || !Regex.IsMatch(birthday, "[0-1][0-9]-[0-3][0-9]"))
        {
            await command.RespondAsync("생일 입력이 잘못됐어.");
            return;
        }

        if (channel == null)
        {
            await command.RespondAsync("채널 입력이 잘못됐어.");
            return;
        }
        
        if (channel is SocketDMChannel)
            DbHelper.AddBirthday(target.Id, birthday, 0, channel.Id);
        else if (channel is SocketGuildChannel guildChannel)
            DbHelper.AddBirthday(target.Id, birthday, guildChannel.Guild.Id, channel.Id);
        DbHelper.Flush();

        await command.RespondAsync("생일 입력에 성공했다!");
    }

    private static async Task ShowBirthdays(SocketSlashCommand command)
    {
        var embed = new EmbedBuilder
        {
            Title = "알고 있는 생일자 목록",
            Description = "아래 생일이 되면 뿌린다!",
            Color = Color.Red
        };

        foreach (var birthday in DbHelper.GetBirthdays())
        {
            var targetId = birthday.Target;
            var user = (command.Channel as SocketGuildChannel)?.Guild.Users.FirstOrDefault(user => user.Id == targetId);
            if (user == null)
                continue;
            embed.AddField(name: user.DisplayName, value: birthday.MonthDay, inline: true);
        }

        await command.RespondAsync(embed: embed.Build());
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

    private static async Task RandomPickR6S(SocketSlashCommand command)
    {
        var isAttack = command.Data.Options.FirstOrDefault(option => option.Name == "공격?")?.Value;
        var containsRoe = command.Data.Options.FirstOrDefault(option => option.Name == "예비병력포함?")?.Value;

        var pool = isAttack is true ? RainbowSixSiegeAttacks : RainbowSixSiegeDefends;

        var index = RandomUtil.Random(containsRoe is true ? 0 : 1, pool.Length);
        var found = pool[index];

        await command.RespondAsync(text: $"`{found}` 해");
    }

    private static async Task PickOne(SocketSlashCommand command)
    {
        var items = new[]
        {
            command.Data.Options.First(option => option.Name == "항목1").Value as string,
            command.Data.Options.First(option => option.Name == "항목2").Value as string,
            command.Data.Options.FirstOrDefault(option => option.Name == "항목3")?.Value as string,
            command.Data.Options.FirstOrDefault(option => option.Name == "항목4")?.Value as string,
            command.Data.Options.FirstOrDefault(option => option.Name == "항목5")?.Value as string,
            command.Data.Options.FirstOrDefault(option => option.Name == "항목6")?.Value as string,
            command.Data.Options.FirstOrDefault(option => option.Name == "항목7")?.Value as string,
            command.Data.Options.FirstOrDefault(option => option.Name == "항목8")?.Value as string,
            command.Data.Options.FirstOrDefault(option => option.Name == "항목9")?.Value as string,
            command.Data.Options.FirstOrDefault(option => option.Name == "항목10")?.Value as string,
        };
        var choiceList = items.Where(item => !string.IsNullOrEmpty(item)).ToArray();

        await command.RespondAsync($"이거: {RandomUtil.PickOne(choiceList)} 💁");
    }

    private static async Task Eue(SocketSlashCommand command)
    {
        if (command.Channel is not SocketGuildChannel guildChannel)
        {
            await command.RespondAsync("서버 내에서만 쓸 수 있어.");
            return;
        }

        var ruin1 = guildChannel.Guild.Emotes.FirstOrDefault(emote => emote.Name == "longruin1");
        var ruin2 = guildChannel.Guild.Emotes.FirstOrDefault(emote => emote.Name == "longruin2");
        var ruin3 = guildChannel.Guild.Emotes.FirstOrDefault(emote => emote.Name == "longruin3");

        if (ruin1 == null || ruin2 == null || ruin3 == null)
        {
            await command.RespondAsync("필요한 이모지 중에 뭔가 없어.");
            return;
        }
        
        await command.RespondAsync($"<:longruin1:{ruin1.Id}><longruin2:{ruin2.Id}><longruin3:{ruin3.Id}>");
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
    private static readonly string[] ImageSunkistList = {"sunkist.png"};
    private static readonly string[] ImageCyphersList =
    {
        "cyphers_1.jpg", "cyphers_2.jpg", "cyphers_3.jpg", "cyphers_4.jpg", "cyphers_5.jpg", "cyphers_6.jpg",
        "cyphers_8.jpg", "cyphers_9.jpg", "cyphers_10.jpg", "cyphers_11.jpg",
    };
    private static readonly string[] ImageReimusNunnaList = {"sister_remmu.jpg"};
    private static readonly string[] ImageXylophoneList = {"xylophone.png"};
    private static readonly string[] ImageSaidNoList = {"i_took_thats_no.png", "i_took_thats_no_2.jpg"};
    private static readonly string[] ImageRuinmanList = {"rivo.jpg"};
    private static readonly string[] ImageBeskin31List = {"thirties_1.png", "thirties_2.png", "thirties_3.jpg"};
    private static readonly string[] ImagePooList = {"dung.jpg"};
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
    private static readonly string[] ImageWhatList = {"wat.gif"};
    private static readonly string[] ImageDoesTheBestList = {"do_the_best.png"};
    private static readonly string[] ImageEvaList =
    {
        "eva.jpg", "eva-25.png",
    };
    private static readonly string[] ImageShisyashoseiList = {"shisyashosei.png"};
    private static readonly string[] ImageBothHandBothFeetList = {"dsdb.jpg"};
    private static readonly string[] ImageRecycleList = {"ecyc_e.mp4"};
    private static readonly string[] ImageDeliciousMiuList = {"delicious_miu.png"};
    private static readonly string[] ImageTiredList = {"tired.jpg"};
    private static readonly string[] ImageNanddongsList =
    {
        "nanddongs_1.jpg", "nanddongs_2.jpg",
    };
    private static readonly string[] ImageYaList = {"ya.mp4"};
    private static readonly string[] ImageGotSteamList = {"yeol_bat_ne.jpg"};
    private static readonly string[] ImageVoteList = {"vote.mp4"};
    private static readonly string[] ImageGaepanList = {"gaepan.jpg"};
    private static readonly string[] ImageAreYouLaughList = {"are_you_laugh.jpg"};
    private static readonly string[] ImageGotoBackList = {"go_to_back.jpg"};
    private static readonly string[] ImageShutTheMouthList = {"shut_the_mouth.jpg"};
    private static readonly string[] ImageLifePatternList = {"saenghwalpattern.jpg"};
    private static readonly string[] ImageDieList =
    {
        "death_cake_when_saw_3_times.jpg", "death_cake_when_saw_3_times_renewal.jpg",
        "otagai_korose.jpg", "just_die.jpg"
    };
    private static readonly string[] ImageWillCrashYouList = {"i_will_crash_you.jpg"};
    private static readonly string[] ImageSabangList =
    {
        "pig_1.png", "pig_2.png",
    };
    private static readonly string[] ImagePigList =
    {
        "png_1.png", "pig_2.png", "pig_3.png", "pig_4.png", "pig_5.png", "pig_6.gif"
    };
    private static readonly string[] ImageLifeList =
    {
        "life_1.png", "life_2.png", "life_3.png",
    };
    private static readonly string[] ImageExodiaList = {"exodia.jpg"};
    private static readonly string[] ImageGiveUpList =
    {
        "giveup.png", "giveup_2.jpg"
    };

    private static async Task MagicalConch(SocketSlashCommand command) =>
        await command.RespondAsync(RandomUtil.PickOne(MagicalConchList));

    private static async Task ImageHome(SocketSlashCommand command) => await SendImageAsync(command, ImageHomeList);
    private static async Task ImageSunkist(SocketSlashCommand command) => await SendImageAsync(command, ImageSunkistList);
    private static async Task ImageCyphers(SocketSlashCommand command) => await SendImageAsync(command, ImageCyphersList);
    private static async Task ImageReimusNunna(SocketSlashCommand command) => await SendImageAsync(command, ImageReimusNunnaList);
    private static async Task ImageXylophone(SocketSlashCommand command) => await SendImageAsync(command, ImageXylophoneList);
    private static async Task ImageSaidNo(SocketSlashCommand command) => await SendImageAsync(command, ImageSaidNoList);
    private static async Task ImageRuinman(SocketSlashCommand command) => await SendImageAsync(command, ImageRuinmanList);
    private static async Task ImageBeskin31(SocketSlashCommand command) => await SendImageAsync(command, ImageBeskin31List);
    private static async Task ImagePoo(SocketSlashCommand command) => await SendImageAsync(command, ImagePooList);
    private static async Task ImageLicking(SocketSlashCommand command) => await SendImageAsync(command, ImageLickingList);
    private static async Task ImageBambooSpear(SocketSlashCommand command) => await SendImageAsync(command, ImageBambooSpearList);
    private static async Task ImageKnewIt(SocketSlashCommand command) => await SendImageAsync(command, ImageKnewItList);
    private static async Task ImageGoSleep(SocketSlashCommand command) => await SendImageAsync(command, ImageGoSleepList);
    private static async Task ImageSolution(SocketSlashCommand command) => await SendImageAsync(command, ImageSolutionList);
    private static async Task ImageDeadmans(SocketSlashCommand command) => await SendImageAsync(command, ImageDeadmansList);
    private static async Task ImageChimus(SocketSlashCommand command) => await SendImageAsync(command, ImageChimusList);
    private static async Task ImageKkang(SocketSlashCommand command) => await SendImageAsync(command, ImageKkangList);
    private static async Task ImageCockroach(SocketSlashCommand command) => await SendImageAsync(command, ImageCockroachList);
    private static async Task ImageZzol(SocketSlashCommand command) => await SendImageAsync(command, ImageZzolList);
    private static async Task ImageYouToo(SocketSlashCommand command) => await SendImageAsync(command, ImageYouTooList);
    private static async Task ImageMeToo(SocketSlashCommand command) => await SendImageAsync(command, ImageMeTooList);
    private static async Task ImageFunCoolSexy(SocketSlashCommand command) => await SendImageAsync(command, ImageFunCoolSexyList);
    private static async Task ImageWhat(SocketSlashCommand command) => await SendImageAsync(command, ImageWhatList);
    private static async Task ImageDoesTheBest(SocketSlashCommand command) => await SendImageAsync(command, ImageDoesTheBestList);
    private static async Task ImageEva(SocketSlashCommand command) => await SendImageAsync(command, ImageEvaList);
    private static async Task ImageShisyashosei(SocketSlashCommand command) => await SendImageAsync(command, ImageShisyashoseiList);
    private static async Task ImageBothHandBothFeet(SocketSlashCommand command) => await SendImageAsync(command, ImageBothHandBothFeetList);
    private static async Task ImageRecycle(SocketSlashCommand command) => await SendImageAsync(command, ImageRecycleList);
    private static async Task ImageDeliciousMiu(SocketSlashCommand command) => await SendImageAsync(command, ImageDeliciousMiuList);
    private static async Task ImageTired(SocketSlashCommand command) => await SendImageAsync(command, ImageTiredList);
    private static async Task ImageNanddongs(SocketSlashCommand command) => await SendImageAsync(command, ImageNanddongsList);
    private static async Task ImageYa(SocketSlashCommand command) => await SendImageAsync(command, ImageYaList);
    private static async Task ImageGotSteam(SocketSlashCommand command) => await SendImageAsync(command, ImageGotSteamList);
    private static async Task ImageVote(SocketSlashCommand command) => await SendImageAsync(command, ImageVoteList);
    private static async Task ImageGaepan(SocketSlashCommand command) => await SendImageAsync(command, ImageGaepanList);
    private static async Task ImageAreYouLaugh(SocketSlashCommand command) => await SendImageAsync(command, ImageAreYouLaughList);
    private static async Task ImageGotoBack(SocketSlashCommand command) => await SendImageAsync(command, ImageGotoBackList);
    private static async Task ImageShutTheMouth(SocketSlashCommand command) => await SendImageAsync(command, ImageShutTheMouthList);
    private static async Task ImageLifePattern(SocketSlashCommand command) => await SendImageAsync(command, ImageLifePatternList);
    private static async Task ImageDie(SocketSlashCommand command) => await SendImageAsync(command, ImageDieList);
    private static async Task ImageWillCrashYou(SocketSlashCommand command) => await SendImageAsync(command, ImageWillCrashYouList);
    private static async Task ImageSabang(SocketSlashCommand command) => await SendImageAsync(command, ImageSabangList);
    private static async Task ImagePig(SocketSlashCommand command) => await SendImageAsync(command, ImagePigList);
    private static async Task ImageLife(SocketSlashCommand command) => await SendImageAsync(command, ImageLifeList);
    private static async Task ImageExodia(SocketSlashCommand command) => await SendImageAsync(command, ImageExodiaList);
    private static async Task ImageGiveUp(SocketSlashCommand command) => await SendImageAsync(command, ImageGiveUpList);

    private static async Task SendImageAsync(SocketInteraction command, string[] imageList)
    {
        var attachment = EmbedResourceUtil.GetAttachment(RandomUtil.PickOne(imageList));
        if (attachment == null)
        {
            await command.RespondAsync("문제가 발생했음!");
            return;
        }
        await command.RespondWithFileAsync(attachment.Value);
    }
}