namespace HoaMokuBot.Parsers
{
    using Contracts;
    using Google.Apis.Services;
    using Google.Apis.YouTube.v3;

    public class YoutubeParser : IYoutubeParser
    {
        public async Task<List<string>> Parse(string youtubePlaylistUrl)
        {
            var results = new List<string>();
            var service = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyAtOtY0gLQYTnuRzTlKCU59vRoXHikCP2o" });
            
            var uri = new Uri(youtubePlaylistUrl);
            var parameters = uri.Query.TrimStart('?')
                .Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(parameter => parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                .GroupBy(parts => parts[0], parts => parts.Length > 2 ? string.Join("=", parts, 1, parts.Length - 1) : (parts.Length > 1 ? parts[1] : ""))
                .ToDictionary(grouping => grouping.Key,
                    grouping => string.Join(",", grouping));

            var playlistId = parameters["list"];

            var pageToken = "";
            while (pageToken != null)
            {
                var request = service.PlaylistItems.List("snippet");
                request.PlaylistId = playlistId;
                request.PageToken = pageToken;
                request.MaxResults = 500;
                
                var response = await request.ExecuteAsync();
                foreach (var item in response.Items)
                {
                    results.Add($"https://www.youtube.com/watch?v={item.Snippet.ResourceId.VideoId}");
                }

                pageToken = response.NextPageToken;
            }

            return results;
        }
    }
}