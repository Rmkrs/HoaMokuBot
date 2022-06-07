namespace HoaMokuBot.Domain
{
    using Contracts;
    using Victoria;

    public class MusicTrack
    {
        public int Id { get; set; }

        public int OriginalId { get; set; }

        public LavaTrack? Track { get; set; }

        public SerializableMusicTrack AsSerializableMusicTrack()
        {
            return new SerializableMusicTrack
            {
                Id = this.Id,
                OriginalId = this.OriginalId,
                Track = this.Track != null ? SerializableLavaTrack.FromLavaTrack(this.Track) : null,
            };
        }

        public static MusicTrack FromSerializableMusicTrack(SerializableMusicTrack serializableMusicTrack)
        {
            return new MusicTrack
            {
                Id = serializableMusicTrack.Id,
                OriginalId = serializableMusicTrack.OriginalId,
                Track = serializableMusicTrack.Track?.AsLavaTrack(),
            };
        }
    }
}
