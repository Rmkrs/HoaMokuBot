// ReSharper disable UnusedMember.Global
namespace HoaMokuBot.Modules
{
    using Discord;
    using Discord.Commands;
    using HoaMokuBot.Preconditions;
    using System.Globalization;
    using Handlers.Contracts;

    [MustBeInVoiceChannel]
    // ReSharper disable once UnusedType.Global
    public class Music : ModuleBase<SocketCommandContext>
    {
        private readonly IMusicLavaPlayerHandler playerHandler;
        private readonly IMusicStatusHandler statusHandler;
        private readonly IMusicPlaylistHandler playlistHandler;
        private readonly IMusicTrackHandler trackHandler;
        private readonly IMusicChannelHandler channelHandler;
        private readonly IMusicSearchHandler searchHandler;
        private readonly IMusicActionHandler actionHandler;

        public Music(
            IMusicLavaPlayerHandler playerHandler, 
            IMusicStatusHandler statusHandler, 
            IMusicPlaylistHandler playlistHandler, 
            IMusicTrackHandler trackHandler,
            IMusicChannelHandler channelHandler,
            IMusicSearchHandler searchHandler,
            IMusicActionHandler actionHandler)
        {
            this.playerHandler = playerHandler;
            this.statusHandler = statusHandler;
            this.playlistHandler = playlistHandler;
            this.trackHandler = trackHandler;
            this.channelHandler = channelHandler;
            this.searchHandler = searchHandler;
            this.actionHandler = actionHandler;

        }

        [Command("Join")]
        [Summary("Make the MusicBot join your current voice channel")]
        public async Task JoinAsync()
        {
            var status = await this.channelHandler.HandleJoin(Context.Guild, (IVoiceState)Context.User, (ITextChannel)Context.Channel);
            await this.statusHandler.UpdateStatus(this.Context.Guild, this.Context.Channel, status);
        }

        [Command("Leave")]
        [Summary("Make the MusicBot leave your current voice channel")]
        public async Task LeaveAsync()
        {
            await this.actionHandler.VerifyAndExecuteAndUpdateStatus(this.Context.Guild, this.Context.Channel, () => this.channelHandler.HandleLeave(this.Context.Guild));
        }

        [Command("Play")]
        [Summary("Start playing from the queue")]
        public async Task PlayAsync()
        {
            await this.actionHandler.VerifyAndExecuteAndUpdateStatus(this.Context.Guild, this.Context.Channel, () => this.trackHandler.HandlePauseResume(this.Context.Guild));
        }

        [Command("Search"), Alias("Add", "Find")]
        [Summary("Search for a song")]
        public async Task SearchAsync([Remainder] string searchQuery)
        {
            await this.actionHandler.VerifyAndExecuteAndUpdateStatus(this.Context.Guild, this.Context.Channel, () => this.searchHandler.Search(this.Context.Guild, this.Context.Channel, searchQuery));
        }

        [Command("Pause")]
        [Summary("Pause the currently playing song")]
        public async Task PauseAsync()
        {
            await this.actionHandler.VerifyAndExecuteAndUpdateStatus(this.Context.Guild, this.Context.Channel, () => this.trackHandler.HandlePauseResume(this.Context.Guild));
        }

        [Command("Resume")]
        [Summary("Resume the currently paused song")]
        public async Task ResumeAsync()
        {
            await this.actionHandler.VerifyAndExecuteAndUpdateStatus(this.Context.Guild, this.Context.Channel, () => this.trackHandler.HandlePauseResume(this.Context.Guild));
        }

        [Command("Stop")]
        [Summary("Stop playing music")]
        public async Task StopAsync()
        {
            await this.actionHandler.VerifyAndExecuteAndUpdateStatus(this.Context.Guild, this.Context.Channel, () => this.trackHandler.HandleStop(this.Context.Guild));
        }

        [Command("Seek")]
        [Summary("Seeks the currently playing song to the specified position")]
        public async Task SeekAsync(string seekTime)
        {
            var timeSpan = TimeSpan.ParseExact(seekTime, @"hh\:mm\:ss", CultureInfo.InvariantCulture);
            await this.actionHandler.VerifyAndExecuteAndUpdateStatus(this.Context.Guild, this.Context.Channel, () => this.trackHandler.Seek(this.Context.Guild, timeSpan));
        }

        [Command("Volume")]
        [Summary("Sets the volume of the MusicBot")]
        public async Task VolumeAsync(ushort? volume = null)
        {
            await this.actionHandler.VerifyAndExecuteAndUpdateStatus(this.Context.Guild, this.Context.Channel, () => this.playerHandler.Volume(this.Context.Guild, volume));
        }

        [Command("NowPlaying"), Alias("Np")]
        [Summary("Display information about the currently playing song")]
        public async Task NowPlayingAsync()
        {
            await this.actionHandler.VerifyAndExecuteAndUpdateStatus(Context.Guild, Context.Channel, () => this.trackHandler.NowPlaying(Context.Guild));
        }

        [Command("Skip"), Alias("Next")]
        [Summary("Skips the currently playing song")]
        public async Task Skip()
        {
            var player = this.playerHandler.GetPlayer(this.Context.Guild);
            await this.actionHandler.VerifyAndExecuteAndUpdateStatus(this.Context.Guild, this.Context.Channel, () => this.trackHandler.NextTrack(player));
        }

        [Command("Load")]
        [Summary("Loads a previously saved playlist")]
        public async Task Load(string playlistName)
        {
            await this.actionHandler.VerifyAndExecuteAndUpdateStatus(this.Context.Guild, this.Context.Channel, () =>
            {
                this.trackHandler.HandleStop(this.Context.Guild);
                var result = this.playlistHandler.Load(playlistName);
                this.trackHandler.HandlePauseResume(this.Context.Guild);
                return result;
            });
        }

        [Command("Save")]
        [Summary("Saves a playlist")]
        public async Task Save(string playlistName)
        {
            await this.actionHandler.VerifyAndExecuteAndUpdateStatus(this.Context.Guild, this.Context.Channel, () => this.playlistHandler.Save(playlistName));
        }

        [Command("Clear")]
        [Summary("Clear all tracks from the playlist")]
        public async Task Clear()
        {
            await this.actionHandler.VerifyAndExecuteAndUpdateStatus(this.Context.Guild, this.Context.Channel, () =>
            {
                this.trackHandler.HandleStop(this.Context.Guild);
                return this.playlistHandler.Clear();
            });
        }

        [Command("Playlists")]
        [Summary("Shows all the saved playlists")]
        public async Task Playlists()
        {
            await this.statusHandler.Reply(this.Context.Channel, "Available playlists:", this.playlistHandler.GetPlaylistNames());
        }

        [Command("Songs")]
        [Summary("Displays all songs in current playlist")]
        public async Task Songs()
        {
            await this.statusHandler.UpdatePlaylistInfo(Context.Channel, 1);
        }

        [Command("AutoJoin")]
        [Summary("Auto Join's the MusicBot to a voice channel.")]
        public async Task AutoJoin()
        {
            await this.actionHandler.VerifyAndExecuteAndUpdateStatus(
                this.Context.Guild, 
                this.Context.Channel,
                () => this.channelHandler.HandleAutoJoin(Context.Guild, (IVoiceState)Context.User, (ITextChannel)Context.Channel));
        }
    }
}