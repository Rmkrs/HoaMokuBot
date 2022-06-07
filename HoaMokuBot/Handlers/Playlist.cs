namespace HoaMokuBot.Handlers
{
    using System.Text.Json;
    using Contracts;
    using Domain;
    using Domain.Contracts;
    using Victoria;

    public class Playlist : IPlaylist
    {
        private List<MusicTrack> tracks = new();

        private MusicTrack? currentTrack;

        public RepeatMode Repeat { get; set; } = RepeatMode.Off;

        public bool IsShuffled { get; private set; }

        public string Name { get; private set; } = String.Empty;

        public LavaTrack? MoveNext()
        {
            if (this.Repeat == RepeatMode.SingleSong && this.currentTrack != null)
            {
                return this.currentTrack.Track;
            }

            var nextId = 1;

            if (this.currentTrack != null)
            {
                nextId = this.currentTrack.Id + 1;
            }

            if (nextId > this.tracks.Count)
            {
                if (this.Repeat == RepeatMode.On)
                {
                    this.currentTrack = this.tracks.Single(t => t.Id == 1);
                    return this.currentTrack.Track;
                }

                return null;
            }

            this.currentTrack =  this.tracks.Single(t => t.Id == nextId);
            return this.currentTrack.Track;    
        }

        public LavaTrack? MovePrevious()
        {
            if (this.Repeat == RepeatMode.SingleSong && this.currentTrack != null)
            {
                return this.currentTrack.Track;
            }

            var previousId = this.tracks.Count;

            if (this.currentTrack != null)
            {
                previousId = this.currentTrack.Id - 1;
            }

            if (previousId == 0)
            {
                if (this.Repeat == RepeatMode.On)
                {
                    this.currentTrack = this.tracks.Single(t => t.Id == this.tracks.Count);
                    return this.currentTrack.Track;
                }

                return null;
            }

            this.currentTrack = this.tracks.Single(t => t.Id == previousId);
            return currentTrack.Track;
        }

        public MusicTrack? Current()
        {
            if (this.currentTrack != null)
            {
                return this.currentTrack;
            }

            return null;
        }

        public int Count()
        {
            return this.tracks.Count;
        }

        public LavaTrack? PeekNext()
        {
            if (this.Repeat == RepeatMode.SingleSong && this.currentTrack != null)
            {
                return this.currentTrack.Track;
            }

            var nextId = 1;

            if (this.currentTrack != null)
            {
                nextId = this.currentTrack.Id + 1;
            }

            if (nextId > this.tracks.Count)
            {
                if (this.Repeat == RepeatMode.On)
                {
                    return this.tracks.Single(t => t.Id == 1).Track;
                }

                return null;
            }

            return this.tracks.Single(t => t.Id == nextId).Track;
        }

        public LavaTrack? PeekPrevious()
        {
            if (this.Repeat == RepeatMode.SingleSong && this.currentTrack != null)
            {
                return this.currentTrack.Track;
            }

            var previousId = this.tracks.Count;

            if (this.currentTrack != null)
            {
                previousId = this.currentTrack.Id - 1;
            }

            if (previousId == 0)
            {
                if (this.Repeat == RepeatMode.On)
                {
                    return this.tracks.Single(t => t.Id == this.tracks.Count).Track;
                }

                return null;
            }

            return this.tracks.Single(t => t.Id == previousId).Track;
        }

        public void Shuffle()
        {
            var ids = Enumerable.Range(1, this.tracks.Count);
            var random = new Random();
            var shuffled = ids.OrderBy(_ => random.Next()).ToList();

            foreach (var musicTrack in tracks.OrderBy(t => t.OriginalId))
            {
                musicTrack.Id = shuffled[musicTrack.OriginalId - 1];
            }

            this.IsShuffled = true;
        }

        public void Unshuffle()
        {
            foreach (var musicTrack in tracks)
            {
                musicTrack.Id = musicTrack.OriginalId;
            }

            this.IsShuffled = false;
        }

        public bool Load(string playlistName)
        {
            if (playlistName.Any(p => Path.GetInvalidFileNameChars().Contains(p)))
            {
                return false;
            }

            if (!File.Exists($"Resources/{playlistName}.json"))
            {
                return false;
            }

            var playlistFileContents = File.ReadAllText($"Resources/{playlistName}.json");

            if (!String.IsNullOrWhiteSpace(playlistFileContents))
            {
                var deserializedPlaylist = JsonSerializer.Deserialize<List<SerializableMusicTrack>>(playlistFileContents);

                if (deserializedPlaylist != null)
                {
                    var convertedTracks = new List<MusicTrack>();

                    foreach (var serializableMusicTrack in deserializedPlaylist)
                    {
                        convertedTracks.Add(MusicTrack.FromSerializableMusicTrack(serializableMusicTrack));
                    }

                    this.tracks.Clear();
                    this.tracks = convertedTracks;
                    this.IsShuffled = false;
                    this.Name = playlistName;
                    this.currentTrack = this.tracks.FirstOrDefault();
                    return true;
                }
            }

            return false;
        }

        public bool Save(string playlistName)
        {
            if (playlistName.Any(p => Path.GetInvalidFileNameChars().Contains(p)))
            {
                return false;
            }

            var jsonConfig = JsonSerializer.Serialize(this.tracks.Select(t => t.AsSerializableMusicTrack()));
            File.WriteAllText($"Resources/{playlistName}.json", jsonConfig);
            this.Name = playlistName;
            return true;
        }

        public void Add(LavaTrack lavaTrack)
        {
            var nextId = this.tracks.Count + 1;
            this.tracks.Add(new MusicTrack { Id = nextId, OriginalId = nextId, Track = lavaTrack });
        }

        public void Clear()
        {
            this.tracks.Clear();
            this.currentTrack = null;
            this.Name = String.Empty;
        }

        public void MoveFirst()
        {
            this.currentTrack = this.tracks.FirstOrDefault();
        }

        public List<MusicTrack> GetMusicTracks(int startId, int maxCount)
        {
            return startId > this.tracks.Count 
                ? new List<MusicTrack>() 
                : this.tracks.Where(t => t.Id >= startId && t.Id < startId + maxCount).OrderBy(t => t.Id).ToList();
        }

        public bool DeletePlaylist(string playlistName)
        {
            if (playlistName.Any(p => Path.GetInvalidFileNameChars().Contains(p)))
            {
                return false;
            }

            if (!File.Exists($"Resources/{playlistName}.json"))
            {
                return false;
            }

            File.Delete($"Resources/{playlistName}.json");
            return true;
        }
    }
}
