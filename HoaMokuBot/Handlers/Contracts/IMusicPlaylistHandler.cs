namespace HoaMokuBot.Handlers.Contracts
{
    using Domain.Contracts;

    public interface IMusicPlaylistHandler
    {
        string Clear();
        
        string Load(string playlistName);
        
        string Save(string playlistName);
        
        string HandleRepeat(RepeatMode newRepeatMode);
        
        List<string> GetPlaylistNames();
        
        string HandleShuffle();

        string Delete(string playlistName);
    }
}
