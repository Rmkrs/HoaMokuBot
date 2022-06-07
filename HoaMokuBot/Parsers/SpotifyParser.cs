namespace HoaMokuBot.Parsers
{
    using Contracts;
    using HtmlAgilityPack;

    public class SpotifyParser : ISpotifyParser
    {
        public List<string> Parse(string spotifyUrl)
        {
            var web = new HtmlWeb();
            var doc = web.Load(spotifyUrl);

            var tracks = doc.DocumentNode.SelectNodes("//button[starts-with(@aria-label, 'track ')]").ToList();
            var currentTrack = 0;

            var results = new List<string>();
            foreach (var track in tracks)
            {
                currentTrack++;

                var currentTrackString = currentTrack.ToString();
                if (track.InnerText.StartsWith(currentTrackString))
                {
                    results.Add(track.InnerText.Substring(currentTrackString.Length));
                }
            }

            return results;
        }
    }
}