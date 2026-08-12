using NetCord;
using NetCord.Rest;

namespace ChimusBot.Bots;

public partial class MainBot
{
    private sealed record CommandDefinition(
        SlashCommandProperties Properties,
        Func<SlashCommandInteraction, Task> Handler);

    private static readonly IReadOnlyDictionary<string, CommandDefinition> Commands = CreateCommands();

    private static IReadOnlyDictionary<string, CommandDefinition> CreateCommands()
    {
        var commands = new Dictionary<string, CommandDefinition>();

        Add("소개", "짭무를 소개합니다.", IntroduceChimus);
        AddRestricted("생일추가", "생일 목록에 생일을 추가합니다.", AddBirthday,
            Permissions.Administrator | Permissions.ManageGuild,
            Option(ApplicationCommandOptionType.User, "생일자", "생일자를 선택", true),
            Option(ApplicationCommandOptionType.String, "생일", "월월-일일", true),
            Option(ApplicationCommandOptionType.Channel, "채널", "생일을 표시할 채널", true));
        Add("생일목록", "생일 목록을 보여줍니다.", ShowBirthdays);
        Add("랜덤픽", "레인보우식스 시즈 랜덤픽", RandomPickR6S,
            Option(ApplicationCommandOptionType.Boolean, "공격", "공격이면 true, 수비면 false", true),
            Option(ApplicationCommandOptionType.Boolean, "예비병력", "예비병력 포함이면 true"));
        Add("골라줘", "골라준다.", PickOne,
            Enumerable.Range(1, 10)
                .Select(index => Option(ApplicationCommandOptionType.String, $"항목{index}", index <= 2 ? "필수" : "선택", index <= 2))
                .ToArray());
        Add("으", "으;", Eue);
        Add("스케줄추가", "스케줄 추가", AddSchedule,
            Option(ApplicationCommandOptionType.String, "메시지", "스케쥴 실행 시 표시되는 메시지", true),
            Option(ApplicationCommandOptionType.Channel, "채널", "스케쥴 실행 시 표시할 채널", true),
            Option(ApplicationCommandOptionType.String, "일시", "연-월-일 시:분 24시간 표기법으로", true));
        Add("스케줄제거", "스케줄 제거", RemoveSchedule,
            Option(ApplicationCommandOptionType.Integer, "번호", "스케줄 번호", true));
        Add("스케줄목록", "스케줄 목록", ListupSchedules);
        Add("소라고둥", "마법의 소라고둥님", MagicalConch);

        Add("집", "랜덤 이미지", ImageHome);
        Add("누나", "랜덤 이미지", ImageReimusNunna);
        Add("띵똥땡똥", "랜덤 이미지", ImageXylophone);
        Add("아니", "랜덤 이미지", ImageSaidNo);
        Add("파멸맨", "랜덤 이미지", ImageRuinman);
        Add("31", "랜덤 이미지", ImageBeskin31);
        Add("할짝", "랜덤 이미지", ImageLicking);
        Add("죽창", "랜덤 이미지", ImageBambooSpear);
        Add("이랄줄", "랜덤 이미지", ImageKnewIt);
        Add("자라", "랜덤 이미지", ImageGoSleep);
        Add("해결책", "랜덤 이미지", ImageSolution);
        Add("망자", "랜덤 이미지", ImageDeadmans);
        Add("짭무", "랜덤 이미지", ImageChimus);
        Add("깡", "랜덤 이미지", ImageKkang);
        Add("고키부리", "랜덤 이미지", ImageCockroach);
        Add("쫄", "랜덤 이미지", ImageZzol);
        Add("야너두", "랜덤 이미지", ImageYouToo);
        Add("야나두", "랜덤 이미지", ImageMeToo);
        Add("펀쿨섹", "랜덤 이미지", ImageFunCoolSexy);
        Add("에바", "랜덤 이미지", ImageEva);
        Add("죽은자의소생", "랜덤 이미지", ImageShisyashosei);
        Add("이끼끼", "랜덤 이미지", ImageRecycle);
        Add("미우", "랜덤 이미지", ImageDeliciousMiu);
        Add("야", "랜덤 이미지", ImageYa);
        Add("열받네", "랜덤 이미지", ImageGotSteam);
        Add("투표", "랜덤 이미지", ImageVote);
        Add("개판", "랜덤 이미지", ImageGaepan);
        Add("웃어", "랜덤 이미지", ImageAreYouLaugh);
        Add("돌아가", "랜덤 이미지", ImageGotoBack);
        Add("입닫아", "랜덤 이미지", ImageShutTheMouth);
        Add("죽어", "랜덤 이미지", ImageDie);
        Add("돼지", "랜덤 이미지", ImagePig);
        Add("엑조디아", "랜덤 이미지", ImageExodia);
        Add("포기해", "랜덤 이미지", ImageGiveUp);

        return commands;

        void Add(
            string name,
            string description,
            Func<SlashCommandInteraction, Task> handler,
            params ApplicationCommandOptionProperties[] options)
        {
            var properties = new SlashCommandProperties(name, description);
            if (options is { Length: > 0 })
                properties.WithOptions(options);

            commands.Add(name, new CommandDefinition(properties, handler));
        }

        void AddRestricted(
            string name,
            string description,
            Func<SlashCommandInteraction, Task> handler,
            Permissions permissions,
            params ApplicationCommandOptionProperties[] options)
        {
            var properties = new SlashCommandProperties(name, description)
                .WithDefaultGuildPermissions(permissions);
            if (options.Length > 0)
                properties.WithOptions(options);

            commands.Add(name, new CommandDefinition(properties, handler));
        }
    }

    private static ApplicationCommandOptionProperties Option(
        ApplicationCommandOptionType type,
        string name,
        string description,
        bool required = false) =>
        new ApplicationCommandOptionProperties(type, name, description).WithRequired(required);
}
