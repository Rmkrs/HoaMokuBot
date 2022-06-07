namespace HoaMokuBot.Handlers
{
    using System.Text;
    using Discord;
    using Domain;
    using HoaMokuBot.Handlers.Contracts;
    using Victoria;

    public class MusicEmbedHandler : IMusicEmbedHandler
    {
        private readonly IPlaylist playlist;

        public MusicEmbedHandler(IPlaylist playlist)
        {
            this.playlist = playlist;
        }

        public async Task<Embed?> CreateTrackInfo(LavaPlayer player, string status)
        {
            var musicTrack = this.playlist.Current();
            if (musicTrack == null || musicTrack.Track == null)
            {
                return new EmbedBuilder { Title = String.Empty }.Build();
            }

            var artwork = await musicTrack.Track.FetchArtworkAsync();

            var embed = new EmbedBuilder
                {
                    Title = $"{musicTrack.Track.Author} - {musicTrack.Track.Title}",
                    ThumbnailUrl = artwork,
                    Url = musicTrack.Track.Url
                }
                .AddField("Queue", $"{musicTrack.Id} / {this.playlist.Count()}")
                .AddField("Duration", musicTrack.Track.Duration.ToString(@"hh\:mm\:ss"))
                .AddField("Position", player.Track?.Position.ToString(@"hh\:mm\:ss") ?? "00:00:00")
                .AddField("Status", status);

            return embed.Build();
        }

        public Embed CreatePlaylistInfo(List<MusicTrack> songs, int total)
        {
            var description = new StringBuilder();

            foreach (var song in songs)
            {
                description.AppendLine($"{song.Id} - {song.Track?.Author} - {song.Track?.Title}{Environment.NewLine}");
            }

            var embed = new EmbedBuilder
            {
                Title = $"Songs in playlist {this.playlist.Name} (total {total})",
                Description = description.ToString(),
            };

            return embed.Build();
        }
    }
}