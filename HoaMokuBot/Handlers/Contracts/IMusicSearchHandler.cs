namespace HoaMokuBot.Handlers.Contracts
{
    using Discord.WebSocket;
    using Victoria;

    public interface IMusicSearchHandler
    {
        Task<string> Search(SocketGuild guild, ISocketMessageChannel channel, string searchQuery);
        
        Task<LavaTrack?> SearchDirect(string url);
    }
}