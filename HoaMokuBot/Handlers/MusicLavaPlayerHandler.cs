namespace HoaMokuBot.Handlers
{
    using Contracts;
    using Discord.WebSocket;
    using Victoria;

    public class MusicLavaPlayerHandler : IMusicLavaPlayerHandler
    {
        private readonly LavaNode lavaNode;

        public MusicLavaPlayerHandler(LavaNode lavaNode)
        {
            this.lavaNode = lavaNode;
        }

        public string? VerifyPlayerVoiceChannel(SocketGuild guild)
        {
            return this.lavaNode.TryGetPlayer(guild, out _) ? default : "I'm not connected to a voice channel.";
        }

        public LavaPlayer GetPlayer(SocketGuild guild)
        {
            return this.lavaNode.GetPlayer(guild);
        }

        public LavaPlayer? TryGetPlayer(SocketGuild guild)
        {
            return this.lavaNode.TryGetPlayer(guild, out var player) ? player : default;
        }

        public async Task<string> Volume(SocketGuild guild, ushort? volume = null)
        {
            var player = this.lavaNode.GetPlayer(guild);

            if (volume == null)
            {
                return $"The current volume is: {player.Volume}";
            }

            try
            {
                await player.UpdateVolumeAsync(volume.Value);
                return $"I've changed the player volume to {volume}.";
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }
    }
}