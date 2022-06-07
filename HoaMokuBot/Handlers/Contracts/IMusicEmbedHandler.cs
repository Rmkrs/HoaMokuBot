namespace HoaMokuBot.Handlers.Contracts
{
    using Discord;
    using Domain;
    using Victoria;

    public interface IMusicEmbedHandler
    {
        Task<Embed?> CreateTrackInfo(LavaPlayer player, string status);
        
        Embed CreatePlaylistInfo(List<MusicTrack> songs, int total);
    }
}
