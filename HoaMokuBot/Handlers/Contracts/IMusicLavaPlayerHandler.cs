namespace HoaMokuBot.Handlers.Contracts
{
    using Discord.WebSocket;
    using Victoria;

    public interface IMusicLavaPlayerHandler
    {
        string? VerifyPlayerVoiceChannel(SocketGuild guild);
        
        LavaPlayer GetPlayer(SocketGuild guild);
        
        LavaPlayer? TryGetPlayer(SocketGuild guild);
        
        Task<string> Volume(SocketGuild guild, ushort? volume = null);
    }
}
