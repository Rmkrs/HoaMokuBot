# HoaMokuBot
Discord bot for the HoaMoku guild that plays music in a voice channel

Fully Configurable:
- AppSettings 
	Discord (Bot Token, Activity type/Name, which Emoji's to use on Buttons)
	LavaNode (Bot deaf yes/no, Connection/Authorization details)
	Youtube (Api Key)

Low CPU/Mem usage:
- Bot runs on average on ~0-2% CPU with ~30 MB memory on an old Core i5 2500K processor.


In channel Interaction Buttons:
- Repeat
- Repeat One
- Shuffle/Unshuffle
- Playlist Delete
- Previous/Next
- First/Last


Available Parsers/Import Playlist support:
- Youtube
- Spotify


Features a Custom Playlist handler:
- Supports Storing/Loading Playlist on Disk (in a folder named Resources under the bot folder)
- Supports Shuffling/Unshufflings the Playlist
- Supports Previous/Next/First/Last
- And of course Pause/Resume/Stop


Extra Features:
- Can be configured to automatically Join a voice channel when the first user connects
- And automatically Leave a voice channel when the last user leaves the channel


Available Chat Commands:
- !Join - Make the MusicBot join your current voice channel
- !Leave - Make the MusicBot leave your current voice channel
- !Play - Start playing from the queue
- !Search, Add, Find - Search for a song
- !Pause - Pause the currently playing song
- !Resume - Resume the currently paused song
- !Stop - Stop playing music
- !Seek - Seeks the currently playing song to the specified position
- !Volume - Sets the volume of the MusicBot
- !NowPlaying, Np - Display information about the currently playing song
- !Skip, Next - Skips the currently playing song
- !Load - Loads a previously saved playlist
- !Save - Saves a playlist
- !Clear - Clear all tracks from the playlist
- !Playlists - Shows all the saved playlists
- !Songs - Displays all songs in current playlist
- !AutoJoin - Auto Join's the MusicBot to a voice channel.

Makes use of:
- .NET 6.0
- Discord.Net
- Victoria
- Google Youtube Api's
- HtmlAgilityPack

How to use:
- Fill in the relevant settings in appSettings.json (for the bot) and application.yaml (for the Lavalink server)
- Start a LavaLink Server (using java and the LavaLink.jar file, settings in application.yaml)
- Start the Bot
