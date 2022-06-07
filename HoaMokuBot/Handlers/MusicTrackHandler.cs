namespace HoaMokuBot.Handlers
{
    using Discord.WebSocket;
    using HoaMokuBot.Handlers.Contracts;
    using Victoria;
    using Victoria.Enums;

    public class MusicTrackHandler : IMusicTrackHandler
    {
        private readonly IPlaylist playlist;
        private readonly IMusicLavaPlayerHandler playerHandler;

        public MusicTrackHandler(IPlaylist playlist, IMusicLavaPlayerHandler playerHandler)
        {
            this.playlist = playlist;
            this.playerHandler = playerHandler;
        }

        public async Task<string> HandlePauseResume(SocketGuild socketGuild)
        {
            var player = this.playerHandler.GetPlayer(socketGuild);

            if (player.PlayerState == PlayerState.Playing)
            {
                await player.PauseAsync();
                return "Paused player.";
            }

            if (player.PlayerState == PlayerState.Paused && player.Track != null)
            {
                await player.ResumeAsync();
                return "Resuming current song.";
            }

            var track = this.playlist.Current()?.Track ?? this.playlist.MoveNext();

            if (track != null)
            {
                await player.PlayAsync(track);
                return "Playing next track.";
            }

            return "No tracks to play.";
        }

        public async Task<string> HandleStop(SocketGuild socketGuild)
        {
            var player = this.playerHandler.GetPlayer(socketGuild);

            if (player.PlayerState == PlayerState.Stopped)
            {
                return String.Empty;
            }

            try
            {
                await player.StopAsync();
                this.playlist.MoveFirst();
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

            return "Stopped playing.";
        }

        public async Task<string> NextTrack(LavaPlayer player)
        {
            var queueItem = this.playlist.MoveNext();

            if (queueItem == null)
            {
                return String.Empty;
            }

            await player.PlayAsync(queueItem);
            return "Playing next track.";
        }

        public async Task<string> PreviousTrack(SocketGuild guild)
        {
            var player = this.playerHandler.GetPlayer(guild);

            if (playlist.PeekPrevious() == null)
            {
                return String.Empty;
            }

            var previousTrack = this.playlist.MovePrevious();
            if (previousTrack == null)
            {
                return String.Empty;
            }

            await player.PlayAsync(previousTrack);
            return "Playing previous song.";
        }

        public async Task<string> Seek(SocketGuild guild, TimeSpan timeSpan)
        {
            var player = this.playerHandler.GetPlayer(guild);

            if (player.PlayerState != PlayerState.Playing && player.PlayerState != PlayerState.Paused)
            {
                return String.Empty;
            }

            try
            {
                await player.SeekAsync(timeSpan);
                return $"I've sought to {timeSpan}.";
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string NowPlaying(SocketGuild guild)
        {
            var player = this.playerHandler.GetPlayer(guild);

            return player.PlayerState != PlayerState.Playing ? "I'm not playing any tracks." : "Refreshed now playing info.";
        }

        public async Task<string> AddSong(SocketGuild guild, string url, LavaTrack? track)
        {
            if (track == null)
            {
                return $"I wasn't able to find anything for `{url}`.";
            }

            var player = this.playerHandler.GetPlayer(guild);

            this.playlist.Add(track);

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                return $"Added: {track.Title}";
            }

            var trackToPlay = this.playlist.MoveNext();
            await player.PlayAsync(trackToPlay);

            return "Playing added song.";
        }
    }
}