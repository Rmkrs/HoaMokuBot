namespace HoaMokuBot.Config.Contracts
{
    public class ConfigGuild
    {
        public string BotPrefix { get; set; } = String.Empty;

        public ConfigAutoJoin? AutoJoin { get; set; }
    }
}