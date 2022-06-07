namespace HoaMokuBot.Handlers.Contracts
{
    using Discord;
    using Discord.WebSocket;

    public interface IMusicChannelHandler
    {
        Task<string> HandleJoin(SocketGuild guild, IVoiceState voiceState, ITextChannel textChannel);
        
        Task<string> HandleLeave(SocketGuild guild);
        
        string HandleAutoJoin(SocketGuild guild, IVoiceState voiceState, ITextChannel textChannel);
        
        Task CheckAutoJoin(SocketGuild guild, IVoiceState voiceState);
    }
}
