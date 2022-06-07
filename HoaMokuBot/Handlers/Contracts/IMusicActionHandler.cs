namespace HoaMokuBot.Handlers.Contracts
{
    using Discord;
    using Discord.WebSocket;

    public interface IMusicActionHandler
    {
        Task VerifyAndExecuteAndUpdateStatus(SocketGuild guild, IMessageChannel channel, Func<string> action);
        
        Task VerifyAndExecuteAndUpdateStatus(SocketGuild guild, IMessageChannel channel, Func<Task<string>> action);
    }
}
