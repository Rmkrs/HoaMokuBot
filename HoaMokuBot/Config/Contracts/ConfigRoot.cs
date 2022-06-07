namespace HoaMokuBot.Config.Contracts
{
    public class ConfigRoot
    {
        public Dictionary<ulong, ConfigGuild> Guilds { get; set; } = new();
    }
}
