namespace HoaMokuBot.Parsers.Contracts
{
    public interface IYoutubeParser
    {
        Task<List<string>> Parse(string youtubePlaylistUrl);
    }
}
