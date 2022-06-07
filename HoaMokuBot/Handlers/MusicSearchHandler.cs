namespace HoaMokuBot.Handlers
{
    using System.Collections.Concurrent;
    using Contracts;
    using Discord.WebSocket;
    using Domain.Contracts;
    using Parsers.Contracts;
    using Victoria;
    using Victoria.Enums;
    using Victoria.Responses.Search;

    public class MusicSearchHandler : IMusicSearchHandler
    {
        private readonly ISpotifyParser spotifyParser;
        private readonly IYoutubeParser youtubeParser;
        private readonly IMusicLavaPlayerHandler playerHandler;
        private readonly IMusicStatusHandler statusHandler;
        private readonly LavaNode lavaNode;
        private readonly IMusicMessageComponentHandler messageComponentHandler;
        private readonly IPlaylist playlist;
        private readonly IMusicTrackHandler trackHandler;

        public MusicSearchHandler(
            ISpotifyParser spotifyParser, 
            IYoutubeParser youtubeParser, 
            IMusicLavaPlayerHandler playerHandler, 
            IMusicStatusHandler statusHandler, 
            LavaNode lavaNode, 
            IMusicMessageComponentHandler messageComponentHandler, 
            IPlaylist playlist, 
            IMusicTrackHandler trackHandler)
        {
            this.spotifyParser = spotifyParser;
            this.youtubeParser = youtubeParser;
            this.playerHandler = playerHandler;
            this.statusHandler = statusHandler;
            this.lavaNode = lavaNode;
            this.messageComponentHandler = messageComponentHandler;
            this.playlist = playlist;
            this.trackHandler = trackHandler;
        }

        private async Task SearchTrack(ConcurrentDictionary<int, SearchResponse> trackDictionary, int id, string track)
        {
            var trackResponse = await this.lavaNode.SearchAsync(SearchType.YouTube, track);
            trackDictionary.TryAdd(id, trackResponse);
        }

        public async Task<string> Search(SocketGuild guild, ISocketMessageChannel channel, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return "Please provide search terms.";
            }

            var player = this.playerHandler.GetPlayer(guild);

            var tracks = new List<string>();

            if (searchQuery.ToLower().Contains("open.spotify.com"))
            {
                tracks = this.spotifyParser.Parse(searchQuery);
            }

            if (searchQuery.ToLower().Contains("youtube.com") && searchQuery.ToLower().Contains("list="))
            {
                tracks = await this.youtubeParser.Parse(searchQuery);
            }

            if (tracks.Any())
            {
                var failedTracks = 0;

                var trackCollection = new List<LavaTrack>();

                await this.statusHandler.Reply(channel, $"Attempting to search {tracks.Count} songs, one-by-one on YouTube. This might take a while...");

                var trackDictionary = new ConcurrentDictionary<int, SearchResponse>();

                var searchTasks = new List<Task>();
                var trackId = 0;
                foreach (var track in tracks)
                {
                    trackId++;
                    searchTasks.Add(this.SearchTrack(trackDictionary, trackId, track));
                }

                await Task.WhenAll(searchTasks);

                for (int i = 0; i < trackId; i++)
                {
                    var trackResponse = trackDictionary[i + 1];
                    if (trackResponse.Status == SearchStatus.LoadFailed || trackResponse.Status == SearchStatus.NoMatches || !trackResponse.Tracks.Any())
                    {
                        failedTracks++;
                        continue;
                    }

                    trackCollection.Add(trackResponse.Tracks.First());
                }

                if (!trackCollection.Any())
                {
                    return $"I wasn't able to find anything for `{searchQuery}`.";
                }

                await this.EnqueuePlaylist(player, trackCollection);

                if (failedTracks > 0)
                {
                    return $"Failed to load {failedTracks} tracks out of {tracks.Count}";
                }

                return $"Loaded {tracks.Count} tracks";
            }

            var searchParameters = this.GetSearchParameters(searchQuery);

            var searchResponse = await this.lavaNode.SearchAsync(searchParameters.SearchType, searchParameters.SearchText);
            if (searchResponse.Status == SearchStatus.LoadFailed ||
                searchResponse.Status == SearchStatus.NoMatches)
            {
                return $"I wasn't able to find anything for `{searchQuery}`.";
            }


            if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
            {
                await this.EnqueuePlaylist(player, searchResponse.Tracks);
                return String.Empty;
            }

            if (searchResponse.Tracks.Count == 1)
            {
                return await this.EnqueueTrack(player, searchResponse.Tracks.First());
            }

            await this.statusHandler.Reply(channel, "Search results:", this.messageComponentHandler.DisplayTrackChoices(searchResponse.Tracks.Take(10).ToList()));
            return string.Empty;
        }

        public async Task<LavaTrack?> SearchDirect(string url)
        {
            var searchResponse = await this.lavaNode.SearchAsync(SearchType.Direct, url);
            if (searchResponse.Status == SearchStatus.LoadFailed ||
                searchResponse.Status == SearchStatus.NoMatches)
            {
                return default;
            }

            return searchResponse.Tracks.First();
        }

        private SearchParameters GetSearchParameters(string searchQuery)
        {
            var lowerSearchQuery = searchQuery.ToLower();

            if (lowerSearchQuery.StartsWith("youtubemusic "))
            {
                return new SearchParameters(SearchType.YouTubeMusic, searchQuery[13..]);
            }

            if (lowerSearchQuery.StartsWith("youtube "))
            {
                return new SearchParameters(SearchType.YouTube, searchQuery[8..]);
            }

            if (lowerSearchQuery.StartsWith("soundcloud "))
            {
                return new SearchParameters(SearchType.YouTube, searchQuery[11..]);
            }

            if (lowerSearchQuery.StartsWith("direct "))
            {
                return new SearchParameters(SearchType.YouTube, searchQuery[7..]);
            }

            return new SearchParameters(SearchType.YouTube, searchQuery);
        }

        private async Task EnqueuePlaylist(LavaPlayer player, IReadOnlyCollection<LavaTrack> tracks)
        {
            foreach (var track in tracks)
            {
                this.playlist.Add(track);
            }

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                return;
            }

            await this.trackHandler.NextTrack(player);
        }

        private async Task<string> EnqueueTrack(LavaPlayer player, LavaTrack track)
        {
            this.playlist.Add(track);

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                return $"Added: {track.Title}";
            }

            await this.trackHandler.NextTrack(player);
            return String.Empty;
        }
    }
}
