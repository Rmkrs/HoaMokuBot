namespace HoaMokuBot.Config.Contracts
{
    public interface IConfig
    {
        string GetBotPrefix(ulong guildId);

        void SetBotPrefix(ulong guildId, string prefix);

        ConfigAutoJoin? GetAutoJoin(ulong guildId);

        void SetAutoJoin(ulong guildId, ulong voiceChannelId, ulong textChannelId);

        void DeleteAutoJoin(ulong guildId);
    }
}

        