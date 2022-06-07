// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace HoaMokuBot.AppSettings
{
    using Discord;

    public class DiscordSettings
    {
        public string BotToken { get; set; }

        public ActivityType ActivityType { get; set; }

        public string ActivityName { get; set; }

        public DiscordButtonEmojiSettings ButtonEmojis { get; set; }
    }
}
