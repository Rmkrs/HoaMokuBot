namespace HoaMokuBot.Handlers
{
    using Contracts;
    using Discord;
    using Discord.WebSocket;

    public class MusicActionHandler : IMusicActionHandler
    {
        private readonly IMusicLavaPlayerHandler playerHandler;
        private readonly IMusicStatusHandler statusHandler;

        public MusicActionHandler(IMusicLavaPlayerHandler playerHandler, IMusicStatusHandler statusHandler)
        {
            this.playerHandler = playerHandler;
            this.statusHandler = statusHandler;
        }

        public async Task VerifyAndExecuteAndUpdateStatus(SocketGuild guild, IMessageChannel channel, Func<string> action)
        {
            var status = this.playerHandler.VerifyPlayerVoiceChannel(guild) ?? action.Invoke();
            await this.statusHandler.UpdateStatus(guild, channel, status);
        }

        public async Task VerifyAndExecuteAndUpdateStatus(SocketGuild guild, IMessageChannel channel, Func<Task<string>> action)
        {
            var status = this.playerHandler.VerifyPlayerVoiceChannel(guild) ?? await action.Invoke();
            await this.statusHandler.UpdateStatus(guild, channel, status);
        }

    }
}
