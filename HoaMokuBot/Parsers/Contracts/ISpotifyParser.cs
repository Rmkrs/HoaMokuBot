namespace HoaMokuBot.Parsers.Contracts
{
    public interface ISpotifyParser
    {
        List<string> Parse(string spotifyUrl);
    }
}
