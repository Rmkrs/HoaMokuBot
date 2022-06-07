namespace HoaMokuBot.Handlers.Contracts
{
    using Discord;
    using Victoria;

    public interface IMusicMessageComponentHandler
    {
        MessageComponent CreateTrackButtons();
        
        MessageComponent CreatePlaylistInfoButtons(int index, int playlistCount, int songsOnPage);
        
        MessageComponent DisplayTrackChoices(List<LavaTrack> firstTenResults);

        List<MessageComponent> DisplayPlaylists(List<string> playlists);
    }
}
