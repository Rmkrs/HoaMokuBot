namespace HoaMokuBot.Handlers.Contracts
{
    using Discord.WebSocket;
    using Victoria;

    public interface IMusicTrackHandler
    {
        Task<string> HandlePauseResume(SocketGuild socketGuild);
        
        Task<string> HandleStop(SocketGuild socketGuild);
        
        Task<string> NextTrack(LavaPlayer player);
        
        Task<string> PreviousTrack(SocketGuild guild);
        
        Task<string> Seek(SocketGuild guild, TimeSpan timeSpan);
        
        string NowPlaying(SocketGuild guild);
        
        Task<string> AddSong(SocketGuild guild, string url, LavaTrack? track);
    }
}