namespace HoaMokuBot.Handlers.Contracts
{
    using Discord;
    using Discord.WebSocket;

    public interface IMusicButtonHandler
    {
        Task OnButtonExecuted(SocketGuild component, IMessageChannel channel, string dataCustomId);
    }
}