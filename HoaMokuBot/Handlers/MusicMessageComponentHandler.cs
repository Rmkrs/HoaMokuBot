namespace HoaMokuBot.Handlers
{
    using AppSettings;
    using Discord;
    using HoaMokuBot.Handlers.Contracts;
    using Microsoft.Extensions.Options;
    using Victoria;

    public class MusicMessageComponentHandler : IMusicMessageComponentHandler
    {
        private readonly DiscordButtonEmojiSettingsParsed emoji;

        public MusicMessageComponentHandler(IOptions<Settings> settings)
        {
            this.emoji = settings.Value.Discord.ButtonEmojis.Parsed;
        }

        public MessageComponent CreateTrackButtons()
        {
            var builder = new ComponentBuilder();
            var firstRow = new ActionRowBuilder();
            firstRow.AddComponent(new ButtonBuilder(null, "stop", ButtonStyle.Secondary, null, emoji.Stop).Build());
            firstRow.AddComponent(new ButtonBuilder(null, "previous", ButtonStyle.Secondary, null, this.emoji.Previous).Build());
            firstRow.AddComponent(new ButtonBuilder(null, "pause-resume", ButtonStyle.Secondary, null, this.emoji.Pause).Build());
            firstRow.AddComponent(new ButtonBuilder(null, "next", ButtonStyle.Secondary, null, this.emoji.Next).Build());

            var secondRow = new ActionRowBuilder();
            secondRow.AddComponent(new ButtonBuilder(null, "repeat", ButtonStyle.Secondary, null, this.emoji.Repeat).Build());
            secondRow.AddComponent(new ButtonBuilder(null, "repeat-one", ButtonStyle.Secondary, null, this.emoji.RepeatOne).Build());
            secondRow.AddComponent(new ButtonBuilder(null, "shuffle", ButtonStyle.Secondary, null, this.emoji.Shuffle).Build());
            secondRow.AddComponent(new ButtonBuilder(null, "refresh", ButtonStyle.Secondary, null, this.emoji.Refresh).Build());

            builder.AddRow(firstRow);
            builder.AddRow(secondRow);
            return builder.Build();
        }

        public MessageComponent CreatePlaylistInfoButtons(int index, int playlistCount, int songsOnPage)
        {
            var isNextDisabled = index + 9 == playlistCount || songsOnPage < 10;

            var builder = new ComponentBuilder();
            var firstRow = new ActionRowBuilder();
            firstRow.AddComponent(new ButtonBuilder(null, "playlist-first", ButtonStyle.Secondary, null, this.emoji.PlaylistFirst, index == 1).Build());
            firstRow.AddComponent(new ButtonBuilder(null, $"playlist-previous-{index - 10}", ButtonStyle.Secondary, null, this.emoji.PlaylistPrevious, index == 1).Build());
            firstRow.AddComponent(new ButtonBuilder(null, $"playlist-next-{index + 10}", ButtonStyle.Secondary, null, this.emoji.PlaylistNext, isNextDisabled).Build());
            firstRow.AddComponent(new ButtonBuilder(null, "playlist-last", ButtonStyle.Secondary, null, this.emoji.PlaylistLast, isNextDisabled).Build());
            builder.AddRow(firstRow);
            return builder.Build();
        }

        public MessageComponent DisplayTrackChoices(List<LavaTrack> firstTenResults)
        {
            var builder = new ComponentBuilder();
            foreach (var track in firstTenResults)
            {
                var title = $"{track.Author} - {track.Title}";
                if (title.Length > 80)
                {
                    title = title.Substring(0, 80);
                }

                builder.WithButton(title, $"add-song-{track.Url}");
            }

            return builder.Build();
        }

        public List<MessageComponent> DisplayPlaylists(List<string> playlists)
        {
            var result = new List<MessageComponent>();
            var currentRow = 0;

            ComponentBuilder? builder = default;

            foreach (var playlist in playlists)
            {
                currentRow++;
                builder ??= new ComponentBuilder();

                var rowBuilder = new ActionRowBuilder();
                rowBuilder.AddComponent(new ButtonBuilder(null, $"playlist-delete-{playlist}", ButtonStyle.Secondary, null, this.emoji.PlaylistDelete).Build());
                rowBuilder.AddComponent(new ButtonBuilder(playlist, $"playlist-play-{playlist}").Build());
                builder.AddRow(rowBuilder);

                if (currentRow == 5)
                {
                    currentRow = 0;
                    result.Add(builder.Build());
                    builder = default;
                }
            }

            if (builder != null)
            {
                result.Add(builder.Build());
            }

            return result;
        }
    }
}