namespace HoaMokuBot.Handlers
{
    using Domain.Contracts;
    using HoaMokuBot.Handlers.Contracts;

    public class MusicPlaylistHandler : IMusicPlaylistHandler
    {
        private readonly IPlaylist playlist;

        public MusicPlaylistHandler(IPlaylist playlist)
        {
            this.playlist = playlist;
        }

        public string Clear()
        {
            this.playlist.Clear();
            return "Cleared the playlist.";
        }

        public string Load(string playlistName)
        {
            var success = this.playlist.Load(playlistName);
            return $"{(success ? "Loaded" : "Could not load")} Playlist: {playlistName}";
        }

        public string Save(string playlistName)
        {
            var success = this.playlist.Save(playlistName);
            return $"{(success ? "Saved" : "Could not save")} Playlist: {playlistName}";
        }

        public string HandleRepeat(RepeatMode newRepeatMode)
        {
            switch (this.playlist.Repeat)
            {
                case RepeatMode.Off:
                    this.playlist.Repeat = newRepeatMode;
                    return $"Repeat set to {newRepeatMode}";
                
                case RepeatMode.On:
                    if (newRepeatMode == RepeatMode.On)
                    {
                        this.playlist.Repeat = RepeatMode.Off;
                        return "Repeat set to Off ";
                    }

                    this.playlist.Repeat = RepeatMode.SingleSong;
                    return "Repeat set to Single Song ";
                
                case RepeatMode.SingleSong:
                    if (newRepeatMode == RepeatMode.SingleSong)
                    {
                        this.playlist.Repeat = RepeatMode.Off;
                        return "Repeat set to Off ";
                    }

                    this.playlist.Repeat = RepeatMode.On;
                    return "Repeat set to On ";
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public List<string> GetPlaylistNames()
        {
            var files = Directory.GetFiles("Resources/", "*.json").ToList();
            files.Remove("Resources/Config.json");

            return files.Select(s => s.Replace("Resources/", "").Replace(".json", "")).ToList();
        }

        public string HandleShuffle()
        {
            if (this.playlist.IsShuffled)
            {
                this.playlist.Unshuffle();
                return "Playlist un-shuffled";
            }

            this.playlist.Shuffle();
            return "Playlist shuffled";
        }

        public string Delete(string playlistName)
        {
            return this.playlist.DeletePlaylist(playlistName) ? "Playlist has been deleted." : "Failed to delete Playlist";
        }
    }
}
