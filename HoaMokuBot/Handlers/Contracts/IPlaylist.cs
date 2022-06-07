namespace HoaMokuBot.Handlers.Contracts
{
    using Domain;
    using Domain.Contracts;
    using Victoria;

    public interface IPlaylist
    {
        RepeatMode Repeat { get; set; }

        bool IsShuffled { get; }

        string Name { get; }

        LavaTrack? MoveNext();
        
        LavaTrack? MovePrevious();
        
        MusicTrack? Current();

        int Count();
        
        LavaTrack? PeekNext();
        
        LavaTrack? PeekPrevious();
        
        void Shuffle();
        
        void Unshuffle();
        
        bool Load(string playlistName);
        
        bool Save(string playlistName);
        
        void Add(LavaTrack lavaTrack);
        
        void Clear();
        
        void MoveFirst();

        List<MusicTrack> GetMusicTracks(int startId, int maxCount);

        bool DeletePlaylist(string playlistName);
    }
}
