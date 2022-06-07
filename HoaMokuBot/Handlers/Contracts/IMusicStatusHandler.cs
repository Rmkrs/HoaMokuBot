namespace HoaMokuBot.Handlers.Contracts
{
    using Discord;
    using Discord.WebSocket;

    public interface IMusicStatusHandler
    {
        Task UpdateStatus(SocketGuild socketGuild, IMessageChannel channel, string status);
        
        Task UpdatePlaylistInfo(IMessageChannel channel, int index);
        
        Task Reply(IMessageChannel channel, string status);
        
        Task Reply(IMessageChannel channel, string status, MessageComponent messageComponent);

        Task Reply(IMessageChannel channel, string status, List<string> playlists);
    }
}
