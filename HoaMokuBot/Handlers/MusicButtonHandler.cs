namespace HoaMokuBot.Handlers
{
    using Contracts;
    using Discord;
    using Discord.WebSocket;
    using Domain.Contracts;

    public class MusicButtonHandler : IMusicButtonHandler
    {
        private readonly IMusicActionHandler actionHandler;
        private readonly IMusicTrackHandler trackHandler;
        private readonly IMusicSearchHandler searchHandler;
        private readonly IMusicLavaPlayerHandler playerHandler;
        private readonly IMusicPlaylistHandler playlistHandler;
        private readonly IMusicStatusHandler statusHandler;
        private readonly IPlaylist playlist;

        public MusicButtonHandler(
            IMusicActionHandler actionHandler, 
            IMusicTrackHandler trackHandler, 
            IMusicSearchHandler searchHandler,
            IMusicLavaPlayerHandler playerHandler, 
            IMusicPlaylistHandler playlistHandler, 
            IMusicStatusHandler statusHandler, 
            IPlaylist playlist)
        {
            this.actionHandler = actionHandler;
            this.trackHandler = trackHandler;
            this.searchHandler = searchHandler;
            this.playerHandler = playerHandler;
            this.playlistHandler = playlistHandler;
            this.statusHandler = statusHandler;
            this.playlist = playlist;
        }

        public async Task OnButtonExecuted(SocketGuild guild, IMessageChannel channel, string customId)
        {

            if (customId.StartsWith("add-song-"))
            {
                var url = customId[9..];
                var track = await this.searchHandler.SearchDirect(url);
                await this.actionHandler.VerifyAndExecuteAndUpdateStatus(guild, channel, () => this.trackHandler.AddSong(guild, url, track));
                return;
            }

            if (customId.StartsWith("stop"))
            {
                await this.actionHandler.VerifyAndExecuteAndUpdateStatus(guild, channel, () => this.trackHandler.HandleStop(guild));
                return;
            }

            if (customId.StartsWith("previous"))
            {
                await this.actionHandler.VerifyAndExecuteAndUpdateStatus(guild, channel, () => this.trackHandler.PreviousTrack(guild));
                return;
            }

            if (customId.StartsWith("pause-resume"))
            {
                await this.actionHandler.VerifyAndExecuteAndUpdateStatus(guild, channel, () => this.trackHandler.HandlePauseResume(guild));
                return;
            }

            if (customId.StartsWith("next"))
            {
                await this.actionHandler.VerifyAndExecuteAndUpdateStatus(guild, channel, () => this.trackHandler.NextTrack(this.playerHandler.GetPlayer(guild)));
                return;
            }

            if (customId.StartsWith("refresh"))
            {
                await this.actionHandler.VerifyAndExecuteAndUpdateStatus(guild, channel, () => this.trackHandler.NowPlaying(guild));
                return;
            }

            if (customId.StartsWith("repeat-one"))
            {
                await this.actionHandler.VerifyAndExecuteAndUpdateStatus(guild, channel, () => this.playlistHandler.HandleRepeat(RepeatMode.SingleSong));
                return;
            }

            if (customId.StartsWith("repeat"))
            {
                await this.actionHandler.VerifyAndExecuteAndUpdateStatus(guild, channel, () => this.playlistHandler.HandleRepeat(RepeatMode.On));
                return;
            }

            if (customId.StartsWith("shuffle"))
            {
                await this.actionHandler.VerifyAndExecuteAndUpdateStatus(guild, channel, () => this.playlistHandler.HandleShuffle());
                return;
            }

            if (customId.StartsWith("playlist-first"))
            {
                await this.statusHandler.UpdatePlaylistInfo(channel, 1);
                return;
            }

            if (customId.StartsWith("playlist-delete-"))
            {
                await this.actionHandler.VerifyAndExecuteAndUpdateStatus(guild, channel, () =>
                {
                    this.trackHandler.HandleStop(guild);
                    var status = this.playlistHandler.Delete(customId[16..]);
                    this.trackHandler.HandlePauseResume(guild);
                    return status;
                });

                return;
            }

            if (customId.StartsWith("playlist-play-"))
            {
                await this.actionHandler.VerifyAndExecuteAndUpdateStatus(guild, channel, () =>
                {
                    this.trackHandler.HandleStop(guild);
                    var status = this.playlistHandler.Load(customId[14..]);
                    this.trackHandler.HandlePauseResume(guild);
                    return status;
                });

                return;
            }

            if (customId.StartsWith("playlist-previous-"))
            {
                await this.statusHandler.UpdatePlaylistInfo(channel, Convert.ToInt32(customId[18..]));
                return;
            }

            if (customId.StartsWith("playlist-next-"))
            {
                await this.statusHandler.UpdatePlaylistInfo(channel, Convert.ToInt32(customId[14..]));
                return;
            }

            if (customId.StartsWith("playlist-last"))
            {
                var total = this.playlist.Count();
                var index = 1;

                while (index < total)
                {
                    index += 10;
                }

                if (index > total)
                {
                    index -= 10;
                }

                await this.statusHandler.UpdatePlaylistInfo(channel, index);
            }
        }
    }
}
