using NetCord;
using NetCord.Rest;

namespace ChimusBot.Utils;

internal static class NetCordInteractionExtensions
{
    public static Task RespondAsync(
        this SlashCommandInteraction interaction,
        string? text = null,
        EmbedProperties? embed = null)
    {
        var message = new InteractionMessageProperties { Content = text };
        if (embed is not null)
            message.AddEmbeds(embed);

        return interaction.SendResponseAsync(InteractionCallback.Message(message));
    }

    public static Task RespondWithFileAsync(
        this SlashCommandInteraction interaction,
        Stream stream,
        string fileName)
    {
        var message = new InteractionMessageProperties()
            .AddAttachments(new AttachmentProperties(fileName, stream));
        return interaction.SendResponseAsync(InteractionCallback.Message(message));
    }
}
