namespace HoaMokuBot.Handlers
{
    using Config.Contracts;
    using Discord;
    using Discord.WebSocket;
    using HoaMokuBot.Handlers.Contracts;
    using Victoria;

    public class MusicChannelHandler : IMusicChannelHandler
    {
        private readonly IMusicLavaPlayerHandler playerHandler;
        private readonly LavaNode lavaNode;
        private readonly IConfig config;

        public MusicChannelHandler(IMusicLavaPlayerHandler playerHandler, LavaNode lavaNode, IConfig config)
        {
            this.playerHandler = playerHandler;
            this.lavaNode = lavaNode;
            this.config = config;
        }

        public async Task<string> HandleJoin(SocketGuild guild, IVoiceState voiceState, ITextChannel textChannel)
        {
            var player = this.playerHandler.TryGetPlayer(guild);
            if (player != null)
            {
                return $"I'm already connected to {voiceState.VoiceChannel ?? player.VoiceChannel}";
            }

            try
            {
                await this.lavaNode.JoinAsync(voiceState.VoiceChannel, textChannel);
                if (this.lavaNode.TryGetPlayer(guild, out var volumePlayer))
                {
                    await volumePlayer.UpdateVolumeAsync(10);
                }

                var status = $"Joined {voiceState.VoiceChannel.Name}";
                
                Console.WriteLine(status);
                return status;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public async Task<string> HandleLeave(SocketGuild guild)
        {
            var player = this.playerHandler.TryGetPlayer(guild);
            if (player == null)
            {
                return "I'm not connected to any voice channel";
            }

            if (player.VoiceChannel == null)
            {
                return "I'm not connected to any voice channel";
            }

            try
            {
                await this.lavaNode.LeaveAsync(player.VoiceChannel);
                return $"Left {player.VoiceChannel}";
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string HandleAutoJoin(SocketGuild guild, IVoiceState voiceState, ITextChannel textChannel)
        {
            var currentAutoJoin = this.config.GetAutoJoin(guild.Id);

            if (currentAutoJoin != null && currentAutoJoin.VoiceChannelId == voiceState.VoiceChannel.Id && currentAutoJoin.TextChannelId == textChannel.Id)
            {
                this.config.DeleteAutoJoin(guild.Id);
                return "AutoJoin turned off";
            }

            this.config.SetAutoJoin(guild.Id, voiceState.VoiceChannel.Id, textChannel.Id);
            return $"AutoJoin turned on for {voiceState.VoiceChannel.Name}";
        }

        public async Task CheckAutoJoin(SocketGuild guild, IVoiceState voiceState)
        {
            var currentAutoJoin = this.config.GetAutoJoin(guild.Id);

            if (currentAutoJoin == null)
            {
                return;
            }

            if (currentAutoJoin.VoiceChannelId != voiceState.VoiceChannel.Id)
            {
                return;
            }

            var voiceUsers = await voiceState.VoiceChannel.GetUsersAsync().FlattenAsync();
            var userPresent = voiceUsers.Any(v => !v.IsBot);
            
            var player = this.playerHandler.TryGetPlayer(guild);

            if (userPresent && player != null)
            {
                // User is present, MusicBot is also present
                return;
            }

            if (!userPresent && player == null)
            {
                // No User is present, MusicBot is also not present
                return;
            }

            if (!userPresent && player != null)
            {
                // No user is present, MusicBot is present, AutoLeave
                await this.lavaNode.LeaveAsync(player.VoiceChannel);
                return;
            }

            // User is present, MusicBot is not present, AutoJoin
            var textChannel = guild.GetTextChannel(currentAutoJoin.TextChannelId);
            await this.lavaNode.JoinAsync(voiceState.VoiceChannel, textChannel);
        }
    }
}