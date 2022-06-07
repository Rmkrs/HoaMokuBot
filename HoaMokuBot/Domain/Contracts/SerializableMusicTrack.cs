namespace HoaMokuBot.Domain.Contracts
{
    public class SerializableMusicTrack
    {
        public int Id { get; set; }

        public int OriginalId { get; set; }

        public SerializableLavaTrack? Track { get; set; }
    }
}
