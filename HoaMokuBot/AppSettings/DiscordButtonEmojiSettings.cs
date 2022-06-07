// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace HoaMokuBot.AppSettings
{
    using Discord;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class DiscordButtonEmojiSettings
    {
        private DiscordButtonEmojiSettingsParsed? parsed = null;

        public string UnknownCommand { get; set; }

        public string Stop { get; set; }

        public string Previous { get; set; }

        public string Pause { get; set; }

        public string Next { get; set; }

        public string Repeat { get; set; }

        public string RepeatOne { get; set; }

        public string Shuffle { get; set; }

        public string Refresh { get; set; }

        public string PlaylistFirst { get; set; }

        public string PlaylistPrevious { get; set; }

        public string PlaylistNext { get; set; }

        public string PlaylistLast { get; set; }

        public string PlaylistDelete { get; set; }

        public DiscordButtonEmojiSettingsParsed Parsed =>
            this.parsed ??= new DiscordButtonEmojiSettingsParsed
            {
                UnknownCommand = Emoji.Parse(this.UnknownCommand),
                Stop = Emoji.Parse(this.Stop),
                Previous = Emoji.Parse(this.Previous),
                Pause = Emoji.Parse(this.Pause),
                Next = Emoji.Parse(this.Next),
                Repeat = Emoji.Parse(this.Repeat),
                RepeatOne = Emoji.Parse(this.RepeatOne),
                Shuffle = Emoji.Parse(this.Shuffle),
                Refresh = Emoji.Parse(this.Refresh),
                PlaylistFirst = Emoji.Parse(this.PlaylistFirst),
                PlaylistPrevious = Emoji.Parse(this.PlaylistPrevious),
                PlaylistNext = Emoji.Parse(this.PlaylistNext),
                PlaylistLast = Emoji.Parse(this.PlaylistLast),
                PlaylistDelete = Emoji.Parse(this.PlaylistDelete),
            };
    }
}
