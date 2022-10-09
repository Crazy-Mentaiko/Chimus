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
            new SlashCommandBuilder().WithName("스케줄추가").WithDescription("스케줄 추가")
                .AddOption("메시지", ApplicationCommandOptionType.String, "스케쥴 실행 시 표시되는 메시지")
                .AddOption("채널", ApplicationCommandOptionType.Channel, "스케쥴 실행 시 표시할 채널")
                .AddOption("일시", ApplicationCommandOptionType.String, "연-월-일 시:분 24시간 표기법으로"),
            AddSchedule
        },
        {
            new SlashCommandBuilder().WithName("스케줄제거").WithDescription("스케줄 제거")
                .AddOption("번호", ApplicationCommandOptionType.Integer, "스케줄 번호"),
            RemoveSchedule
        },
        {
            new SlashCommandBuilder().WithName("스케줄목록").WithDescription("스케줄 목록"),
            ListupSchedules
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
}