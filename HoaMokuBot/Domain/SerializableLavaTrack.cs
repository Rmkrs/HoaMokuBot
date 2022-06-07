// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace HoaMokuBot.Domain
{
    using Victoria;

    public class SerializableLavaTrack
    {
        public string Hash { get; set; }

        public string Id { get; set; }

        public string Author { get; set; }

        public string Title { get; set; }

        public bool CanSeek { get; set; }

        public TimeSpan Duration { get; set; }

        public bool IsStream { get; set; }

        public TimeSpan Position { get; set; }

        public string Url { get; set; }

        public string Source { get; set; }

        public LavaTrack AsLavaTrack()
        {
            return new LavaTrack(
                this.Hash,
                this.Id,
                this.Title,
                this.Author,
                this.Url,
                this.Position,
                (long)this.Duration.TotalMilliseconds,
                this.CanSeek,
                this.IsStream,
                this.Source);

        }

        public static SerializableLavaTrack FromLavaTrack(LavaTrack lavaTrack)
        {
            return new SerializableLavaTrack
            {
                Hash = lavaTrack.Hash,
                Id = lavaTrack.Id,
                Author = lavaTrack.Author,
                Title = lavaTrack.Title,
                CanSeek = lavaTrack.CanSeek,
                Duration = lavaTrack.Duration,
                IsStream = lavaTrack.IsStream,
                Position = lavaTrack.Position,
                Url = lavaTrack.Url,
                Source = lavaTrack.Source,
            };
        }
    }
}
